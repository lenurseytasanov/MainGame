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
        public Dictionary<string, LevelModel> Levels { get; set; }
        public LevelModel CurrentLevel { get; set; }

        public void Initialize()
        {
            Levels = new Dictionary<string, LevelModel>()
            {
                { "level1", new LevelModel() { LoadPath = "../../../Sources/Levels/level1.txt" } },
                { "level2", new LevelModel() { LoadPath = "../../../Sources/Levels/level2.txt" } }
            };
            CurrentLevel = Levels["level1"];
            CurrentLevel.Initialize();

            Updated?.Invoke(this, new ObjectsEventArgs()
            {
                Objects = CurrentLevel.Objects,
                PlayerId = CurrentLevel.PlayerId,
                LevelSize = CurrentLevel.LevelSize,
            });
        }

        public void Move(int id, Direction direction)
        {
            var dynamic = CurrentLevel.Objects[id] as DynamicObject;
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
            var chr = CurrentLevel.Objects[id] as Character;
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
            foreach (var character in CurrentLevel.Objects.Values.OfType<Character>())
            {
                character.Update(gameTime);

                if (IsLevelChanged(character))
                    break;

                var flying = true;
                foreach (var staticObj in CurrentLevel.Objects.Values.OfType<Ground>())
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

                foreach (var damaging in CurrentLevel.Objects.Values
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
                Objects = CurrentLevel.Objects, 
                PlayerId = CurrentLevel.PlayerId, 
                LevelSize = CurrentLevel.LevelSize
            });
        }

        private bool IsLevelChanged(Character character)
        {
            if (CurrentLevel.LevelSize.Contains(character.PhysicalBound))
                return false;
            character.Forces = new Vector2(0, -1) * character.Mass;
            character.Speed = new Vector2(10, 0) * (character.Speed.X > 0 ? -1 : 1);
            if (character.PhysicalBound.Right > CurrentLevel.LevelSize.Width)
                CurrentLevel = Levels["level2"];
            if (character.PhysicalBound.Left < 0)
                CurrentLevel = Levels["level1"];
            if (CurrentLevel.Objects == null)
                CurrentLevel.Initialize();
            LevelChanged?.Invoke(this, EventArgs.Empty);
            return true;
        }

        public event EventHandler<ObjectsEventArgs> Updated;

        public event EventHandler LevelChanged;
    }
}
