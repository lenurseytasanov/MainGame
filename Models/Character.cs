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
        public byte HealthPoints { get; set; } = 10;

        public int AttackNumber { get; set; }

        public int AttackCount { get; set; }

        public HashSet<MovingSolidObject> DamagedObjects { get; set; } = new HashSet<MovingSolidObject>();

        private TimeSpan _elapsedTime;

        public override void Update(GameTime gameTime)
        {
            if (HealthPoints <= 0)
                State |= StateCharacter.Dead;

            if ((State & StateCharacter.Hurt) != 0)
            {
                _elapsedTime += gameTime.ElapsedGameTime;
                if (_elapsedTime > TimeSpan.FromMilliseconds(500))
                {
                    State ^= StateCharacter.Hurt;
                    _elapsedTime = TimeSpan.Zero;
                }
            }
            if ((State & StateCharacter.Attacking) != 0)
            {
                _elapsedTime += gameTime.ElapsedGameTime;
                if (_elapsedTime > TimeSpan.FromMilliseconds(500))
                {
                    State &= ~StateCharacter.Attacking;
                    DamagedObjects.Clear();
                    _elapsedTime = TimeSpan.Zero;
                }
            }

            base.Update(gameTime);
        }
    }
}
