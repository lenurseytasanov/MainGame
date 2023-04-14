﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class MovingSolidObject : IGameObject, IDamaging
    {
        public Vector2 Position { get; set; }

        public Rectangle Size { get; set; }

        public int SpriteId { get; set; }

        public StateCharacter State { get; set; }

        public Vector2 Speed { get; set; }

        public Vector2 Forces { get; set; }

        public float Acceleration { get; set; } = 4.0f;

        public float Mass { get; set; } = 1.0f;

        public Direction Direction { get; set; }

        public Rectangle PhysicalBound { get; set; }

        public Rectangle HitBox { get; set; }

        public int Damage { get; set; } = 2;

        public bool IsTouchLeft(StaticSolidObject other)
        {
            return PhysicalBound.Right + Speed.X > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Left &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchRight(StaticSolidObject other)
        {
            return PhysicalBound.Left + Speed.X < other.PhysicalBound.Right &&
                   PhysicalBound.Right > other.PhysicalBound.Right &&
                   PhysicalBound.Bottom > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Bottom;
        }

        public bool IsTouchTop(StaticSolidObject other)
        {
            return PhysicalBound.Bottom + Speed.Y > other.PhysicalBound.Top &&
                   PhysicalBound.Top < other.PhysicalBound.Top &&
                   PhysicalBound.Right > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Right;
        }

        public bool IsTouchBottom(StaticSolidObject other)
        {
            return PhysicalBound.Top + Speed.Y < other.PhysicalBound.Bottom &&
                   PhysicalBound.Bottom > other.PhysicalBound.Bottom &&
                   PhysicalBound.Right > other.PhysicalBound.Left &&
                   PhysicalBound.Left < other.PhysicalBound.Right;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (Math.Abs(Speed.X) < 1 &&
                ((State & StateCharacter.Dead) == 0))
                State |= StateCharacter.Standing;
            else 
                State &= ~StateCharacter.Standing;

            Position += new Vector2((int)Speed.X, (int)Speed.Y);
            Direction = Speed.X switch
            {
                > 0 => Direction.Right,
                < 0 => Direction.Left,
                _ => Direction
            };
            PhysicalBound = new Rectangle(PhysicalBound.X + (int)Speed.X, PhysicalBound.Y + (int)Speed.Y, PhysicalBound.Width, PhysicalBound.Height);
            HitBox = PhysicalBound;
            Speed += Forces / Mass;
            Forces = Vector2.Zero;
            Forces += new Vector2(0, 1f) * Mass; // gravity
            Forces += new Vector2(-Speed.X * 0.6f, 0); // resistance
        }
    }
}
