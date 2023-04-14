using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class PhysicalModel
    {
        public Dictionary<int, IGameObject> Objects { get; private set; }
        private int _playerId;

        public LevelModel Level { get; set; }

        public void Initialize()
        {
            Objects = Level.Objects;
            _playerId = Level.PlayerId;

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = Objects, PlayerId = _playerId });
        }

        public void Move(int id, Direction direction)
        {
            var player = Objects[id] as MovingSolidObject;
            if (player is Character { HealthPoints: <= 0 }) return;
            switch (direction)
            {
                case Direction.Left:
                    player.Forces += new Vector2(-player.Acceleration, 0);
                    break;
                case Direction.Right:
                    player.Forces += new Vector2(player.Acceleration, 0);
                    break;
                case Direction.Up:
                    if ((player.State & StateCharacter.Flying) == 0)
                        player.Forces += new Vector2(0, -20);
                    break;
            }
        }

        public void Attack(int id)
        {
            var chr = Objects[id] as Character;
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
            foreach (var movingObj in Objects.Values.OfType<MovingSolidObject>())
            {
                movingObj.Update(gameTime);

                if (movingObj.PhysicalBound.Right > Level.FieldWidth)
                {
                    movingObj.Forces = new Vector2(0, -1) * movingObj.Mass;
                    movingObj.Speed = new Vector2(-5, 0);
                    LevelChanged?.Invoke(this, new LevelChangeArgs() { Direction = Direction.Right, LevelName = Level.Name });
                    return;
                }

                if (movingObj.PhysicalBound.Left < 0)
                {
                    movingObj.Forces = new Vector2(0, -1) * movingObj.Mass;
                    movingObj.Speed = new Vector2(5, 0);
                    LevelChanged?.Invoke(this, new LevelChangeArgs() { Direction = Direction.Left, LevelName = Level.Name });
                    return;
                }

                var flying = true;
                foreach (var staticObj in Objects.Values.OfType<StaticSolidObject>())
                {
                    if (movingObj.IsTouchTop(staticObj))
                    {
                        flying = false;
                        movingObj.Speed = new Vector2(movingObj.Speed.X, 0);
                    }
                    if (movingObj.IsTouchBottom(staticObj))
                        movingObj.Speed = new Vector2(movingObj.Speed.X, 0);
                    if (movingObj.IsTouchLeft(staticObj))
                        movingObj.Speed = new Vector2(-0, movingObj.Speed.Y);
                    if (movingObj.IsTouchRight(staticObj))
                        movingObj.Speed = new Vector2(0, movingObj.Speed.Y);
                }

                if (flying)
                    movingObj.State |= StateCharacter.Flying;
                else
                    movingObj.State &= ~StateCharacter.Flying;

                if (movingObj is not Character chr || 
                    (chr.State & StateCharacter.Dead) != 0 ||
                    (chr.State & StateCharacter.Hurt) != 0)
                    continue;

                foreach (var damaging in Objects.Values
                             .OfType<IDamaging>()
                             .Where(d => !d.Equals(chr) && chr.HitBox.Intersects(d.HitBox)))
                {
                    if (damaging is Character chr1 && 
                        ((chr1.State & StateCharacter.Attacking) == 0 || (chr1.State & StateCharacter.Dead ) != 0))
                        continue;

                    chr.HealthPoints -= damaging.Damage;
                    if (chr.HealthPoints < 0) chr.HealthPoints = 0;

                    chr.State |= StateCharacter.Hurt;
                    chr.Forces += new Vector2(chr.Direction == Direction.Right ? 5 : -10, -10);
                    break;
                }
            }

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = Objects, PlayerId = _playerId });
        }

        public event EventHandler<ObjectsEventArgs> Updated;

        public event EventHandler<LevelChangeArgs> LevelChanged;
    }
}
