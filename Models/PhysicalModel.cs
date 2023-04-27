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

        public void Initialize()
        {
            _levels = new Dictionary<string, LevelModel>()
            {
                { "lobby", new LevelModel() { LoadPath = "../../../Sources/Levels/lobby.txt" } },
                { "level1", new LevelModel() { LoadPath = "../../../Sources/Levels/level1.txt" } },
                { "level2", new LevelModel() { LoadPath = "../../../Sources/Levels/level2.txt" } }
            };
            _levelTransitions = new Dictionary<string, (string toLeft, string toRight)>()
            {
                { "lobby", ("", "level1") },
                { "level1", ("lobby", "level2") },
                { "level2", ("level1", "") }
            };

            _currentLevel = "lobby";
            _levels[_currentLevel].Initialize();

            Updated?.Invoke(this, new ObjectsEventArgs()
            {
                Objects = _levels[_currentLevel].Objects,
                PlayerId = _levels[_currentLevel].PlayerId,
                LevelSize = _levels[_currentLevel].LevelSize,
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
            if ((chr.State & StateCharacter.Attacking) == 0 &&
                (chr.State & StateCharacter.Dead) == 0)
            {
                chr.State |= StateCharacter.Attacking;
                chr.AttackNumber++;
                chr.AttackNumber %= chr.AttackCount;
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (var character in _levels[_currentLevel].Objects.Values.OfType<Character>())
            {
                character.Update(gameTime);

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
                    if (damaging is Character chr1 && 
                        ((chr1.State & StateCharacter.Attacking) == 0 || (chr1.State & StateCharacter.Dead ) != 0))
                        continue;

                    character.HealthPoints -= damaging.Damage;
                    if (character.HealthPoints < 0) character.HealthPoints = 0;

                    character.State |= StateCharacter.Hurt;
                    character.Forces += new Vector2(character.Direction == Direction.Right ? 5 : -10, -10);
                    break;
                }
            }

            Updated?.Invoke(this, new ObjectsEventArgs()
            {
                Objects = _levels[_currentLevel].Objects, 
                PlayerId = _levels[_currentLevel].PlayerId, 
                LevelSize = _levels[_currentLevel].LevelSize
            });
        }

        private bool IsLevelChanged(Character character)
        {
            if (_levels[_currentLevel].LevelSize.Contains(character.PhysicalBound))
                return false;

            character.Forces = new Vector2(0, -1) * character.Mass;
            character.Speed = new Vector2(10, 0) * (character.Speed.X > 0 ? -1 : 1);

            if (character.PhysicalBound.Right > _levels[_currentLevel].LevelSize.Width)
                _currentLevel = _levelTransitions[_currentLevel].toRight;
            if (character.PhysicalBound.Left < 0)
                _currentLevel = _levelTransitions[_currentLevel].toLeft;

            if (_levels[_currentLevel].Objects == null)
                _levels[_currentLevel].Initialize();
            LevelChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public event EventHandler<ObjectsEventArgs> Updated;

        public event EventHandler LevelChanged;
    }
}
