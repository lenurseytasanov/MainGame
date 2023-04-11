using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class Character : MovingSolidObject
    {
        public int HealthPoints { get; set; } = 10;

        public int AttackNumber { get; set; }

        public int AttackCount { get; set; }

        private TimeSpan _elapsedTime;

        public override void Update(GameTime gameTime)
        {
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
            if (HealthPoints <= 0)
                State |= StateCharacter.Dead;

            base.Update(gameTime);
        }
    }
}
