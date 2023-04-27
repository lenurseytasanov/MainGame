using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models.GameObjects
{
    public class Character : DynamicObject, ISolid, IDamaging
    {
        public int HealthPoints { get; set; } = 10;

        public int Damage { get; set; } = 2;

        public Rectangle HitBox { get; set; }

        public Rectangle PhysicalBound { get; set; }

        public StateCharacter State { get; set; }

        public Direction Direction { get; set; }

        private TimeSpan _elapsedTime;

        public override void Update(GameTime gameTime)
        {
            Size = new Rectangle(Size.X + (int)Speed.X, Size.Y + (int)Speed.Y, Size.Width, Size.Height);
            PhysicalBound = new Rectangle(PhysicalBound.X + (int)Speed.X, PhysicalBound.Y + (int)Speed.Y, PhysicalBound.Width, PhysicalBound.Height);
            HitBox = PhysicalBound;

            if (Math.Abs(Speed.X) < 1 &&
                (State & StateCharacter.Dead) == 0)
                State |= StateCharacter.Standing;
            else
                State &= ~StateCharacter.Standing;

            Direction = Speed.X switch
            {
                > 0 => Direction.Right,
                < 0 => Direction.Left,
                _ => Direction
            };

            if ((State & StateCharacter.Hurt) != 0)
            {
                _elapsedTime += gameTime.ElapsedGameTime;

                if (_elapsedTime > TimeSpan.FromMilliseconds(500))
                {
                    State &= ~StateCharacter.Hurt;
                    _elapsedTime = TimeSpan.Zero;
                }
            }
            if ((State & StateCharacter.Attacking) != 0)
            {
                _elapsedTime += gameTime.ElapsedGameTime;
                if (_elapsedTime > TimeSpan.FromMilliseconds(500))
                {
                    State &= ~StateCharacter.Attacking;
                    _elapsedTime = TimeSpan.Zero;
                }
            }
            if (HealthPoints == 0)
                State |= StateCharacter.Dead;

            base.Update(gameTime);
        }

        public bool IsTouchLeft(ISolid other)
        {
            return PhysicalBound.Right + Speed.X > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Left &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchRight(ISolid other)
        {
            return PhysicalBound.Left + Speed.X < other.PhysicalBound.Right &&
                   PhysicalBound.Right > other.PhysicalBound.Right &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchTop(ISolid other)
        {
            return PhysicalBound.Bottom + Speed.Y > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Top &&
                   PhysicalBound.Right > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Right;
        }

        public bool IsTouchBottom(ISolid other)
        {
            return PhysicalBound.Top + Speed.Y < other.PhysicalBound.Bottom &&
                   PhysicalBound.Bottom > other.PhysicalBound.Bottom &&
                   PhysicalBound.Right > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Right;
        }
    }
}
