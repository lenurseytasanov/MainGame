using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class Knight : MovingSolidObject
    {
        public byte HealthPoints { get; set; } = 10;

        public bool IsHurt { get; set; }

        public Attack Attack { get; set; }

        public HashSet<MovingSolidObject> DamagedObjects { get; set; } = new HashSet<MovingSolidObject>();

        private TimeSpan _elapsedTime;

        public override void Update(GameTime gameTime)
        {
            if (IsHurt)
            {
                _elapsedTime += gameTime.ElapsedGameTime;
                if (_elapsedTime > TimeSpan.FromMilliseconds(500))
                {
                    IsHurt = false;
                    _elapsedTime = TimeSpan.Zero;
                }
            }
            if (Attack != Attack.None)
            {
                _elapsedTime += gameTime.ElapsedGameTime;
                if (_elapsedTime > TimeSpan.FromMilliseconds(500))
                {
                    Attack = Attack.None;
                    DamagedObjects.Clear();
                    _elapsedTime = TimeSpan.Zero;
                }
            }

            base.Update(gameTime);
        }
    }
}
