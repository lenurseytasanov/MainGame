using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using MainGame.Models.GameObjects;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class PhysicalModel
    {
        private Dictionary<string, LevelModel> _levels;
        private string _currentLevel;
        private Dictionary<string, (string toLeft, string toRight)> _levelTransitions;

        private readonly Dictionary<int, IGameObject> _temporary = new();

        public void Initialize()
        {
            _levels = new Dictionary<string, LevelModel>()
            {
                { "lobby", new LevelModel() { LoadPath = "../../../Sources/Levels/lobby.txt", } },
                { "level1", new LevelModel() { LoadPath = "../../../Sources/Levels/level1.txt" } },
                { "level2", new LevelModel() { LoadPath = "../../../Sources/Levels/level2.txt" } },
                { "level3", new LevelModel() { LoadPath = "../../../Sources/Levels/level3.txt" } },
                { "boss", new LevelModel() { LoadPath = "../../../Sources/Levels/boss.txt" } },
            };
            _levelTransitions = new Dictionary<string, (string toLeft, string toRight)>()
            {
                { "lobby", ("", "level1") },
                { "level1", ("lobby", "level2") },
                { "level2", ("level1", "level3") },
                { "level3", ("level2", "boss") },
                { "boss", ("level3", "") }
            };

            _currentLevel = "lobby";
            _levels[_currentLevel].Initialize();

            Updated?.Invoke(this, new LoadEventArgs()
            {
                Level = _levels[_currentLevel]
            });
        }

        public void Reset()
        {
            _levels[_currentLevel].Initialize();
            Updated?.Invoke(this, new LoadEventArgs()
            {
                Level = _levels[_currentLevel]
            });
        }

        public void Move(int id, Direction direction)
        {
            var dynamic = _levels[_currentLevel].Objects[id] as DynamicObject;
            if (dynamic is Character { HealthPoints: <= 0 }) return;
            switch (direction)
            {
                case Direction.Left:
                    dynamic.Forces += new Vector2(-dynamic.Acceleration, 0);
                    break;
                case Direction.Right:
                    dynamic.Forces += new Vector2(dynamic.Acceleration, 0);
                    break;
                case Direction.Up:
                    if (dynamic is Character chr && (chr.State & StateCharacter.Flying) == 0)
                        dynamic.Forces += new Vector2(0, -20);
                    break;
            }
        }

        public void Attack(int id)
        {
            var chr = _levels[_currentLevel].Objects[id] as Character;
            if (chr.AttackingTime > chr.Cooldown && (chr.State & StateCharacter.Dead) == 0)
            {
                chr.AttackingTime = TimeSpan.Zero;
                chr.State |= StateCharacter.Attacking;
            }
        }

        public void Shoot(int id)
        {
            var chr = _levels[_currentLevel].Objects[id] as Character;
            var player = _levels[_currentLevel].Objects[_levels[_currentLevel].PlayerId] as Character;
            var previousState = chr.State;
            Attack(id);
            if ((previousState & StateCharacter.Attacking) == 0 &&
                (chr.State & StateCharacter.Attacking) != 0 &&
                (chr.State & StateCharacter.Dead) == 0)
            {
                var dir = player.PhysicalBound.Center.ToVector2() - chr.Position;
                dir /= dir.Length();
                _temporary.Add(
                    GetRandomId(),
                    new Fireball()
                    {
                        Position = chr.Position,
                        Speed = 10 * dir,
                        SpriteId = 50,
                        Damage = 1,
                        Mass = 0,
                        AirResistance = 0,
                        Size = new Rectangle((int)chr.Position.X, (int)chr.Position.Y, 100, 100)
                    });
            }
        }

        private int GetRandomId()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var id = rnd.Next(1, 1000);
            while (_levels[_currentLevel].Objects.ContainsKey(id) ||
                   _temporary.ContainsKey(id))
                id = rnd.Next(1, 1000);
            return id;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var dynamic in _levels[_currentLevel].Objects.Values.OfType<DynamicObject>())
            {
                dynamic.Update(gameTime);

                if (dynamic is not Character character) continue;

                if (IsLevelChanged(character))
                    break;

                var flying = true;
                foreach (var staticObj in _levels[_currentLevel].Objects.Values.OfType<Ground>())
                {
                    if (character.IsTouchTop(staticObj))
                    {
                        flying = false;
                        character.Speed = new Vector2(character.Speed.X, 0);
                    }
                    if (character.IsTouchBottom(staticObj))
                        character.Speed = new Vector2(character.Speed.X, 0);
                    if (character.IsTouchLeft(staticObj))
                        character.Speed = new Vector2(0, character.Speed.Y);
                    if (character.IsTouchRight(staticObj))
                        character.Speed = new Vector2(0, character.Speed.Y);
                }

                if (flying)
                    character.State |= StateCharacter.Flying;
                else
                    character.State &= ~StateCharacter.Flying;

                if ((character.State & StateCharacter.Dead) != 0 ||
                    (character.State & StateCharacter.Hurt) != 0)
                    continue;

                foreach (var damaging in _levels[_currentLevel].Objects.Values
                             .OfType<IDamaging>()
                             .Where(d => !d.Equals(character) && character.HitBox.Intersects(d.HitBox)))
                {
                    if (damaging is Character chr1 && (chr1.State & StateCharacter.Attacking) == 0)
                        continue;
                    var player = _levels[_currentLevel].Objects[_levels[_currentLevel].PlayerId] as Character;
                    if (!(damaging.Equals(player) || !damaging.Equals(player) && character.Equals(player)))
                        continue;

                    character.HealthPoints -= damaging.Damage;
                    if (character.HealthPoints < 0) character.HealthPoints = 0;

                    character.State |= StateCharacter.Hurt;
                    character.Forces += new Vector2(character.Direction == Direction.Right ? 5 : -10, -10);
                    break;
                }
            }

            foreach (var pair in _temporary)
            {
                _levels[_currentLevel].Objects.Add(pair.Key, pair.Value);
            }
            _temporary.Clear();

            Updated?.Invoke(this, new LoadEventArgs()
            {
                Level = _levels[_currentLevel]
            });
        }

        private bool IsLevelChanged(Character character)
        {
            if (_levels[_currentLevel].LevelSize.Contains(character.PhysicalBound))
                return false;

            character.Forces = new Vector2(0, -1) * character.Mass;
            character.Speed = new Vector2(10, 0) * (character.Speed.X > 0 ? -1 : 1);
            
            var health = character.HealthPoints;
            if (character.PhysicalBound.Right > _levels[_currentLevel].LevelSize.Width)
                _currentLevel = _levelTransitions[_currentLevel].toRight;
            if (character.PhysicalBound.Left < 0)
                _currentLevel = _levelTransitions[_currentLevel].toLeft;

            if (_levels[_currentLevel].Objects == null)
                _levels[_currentLevel].Initialize();
            (_levels[_currentLevel].Objects[_levels[_currentLevel].PlayerId] as Character).HealthPoints = health;
            LevelChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public event EventHandler<LoadEventArgs> Updated;

        public event EventHandler LevelChanged;
    }
}
