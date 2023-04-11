using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MainGame.Sprites
{
    public class CharacterSprite : AnimatedSprite
    {
        public Direction Direction { get; set; }

        public int AttackNumber { get; set; }

        public StateCharacter State { get; set; }

        public void SetAnimations(int spriteId)
        {
            if ((State & StateCharacter.Dead) != 0)
                SetAnimation(Direction == Direction.Right ? "DeadRight" : "DeadLeft");
            if ((State & StateCharacter.Dead) == 0 && (State & StateCharacter.Hurt) != 0)
                SetAnimation(Direction == Direction.Right ? "HurtRight" : "HurtLeft");
            if ((State & StateCharacter.Dead) == 0 && (State & StateCharacter.Flying) != 0)
                SetAnimation(Direction == Direction.Right ? "JumpRight" : "JumpLeft");
            if ((State & StateCharacter.Dead) == 0 && (State & StateCharacter.Flying) == 0 && (State & StateCharacter.Standing) == 0)
                SetAnimation(Direction == Direction.Right ? "RunRight" : "RunLeft");
            if ((State & StateCharacter.Dead) == 0 && (State & StateCharacter.Flying) == 0 && (State & StateCharacter.Standing) != 0)
                SetAnimation(Direction == Direction.Right ? "IdleRight" : "IdleLeft");
            switch (spriteId)
            {
                case 1:
                    if ((State & StateCharacter.Standing) != 0 && (State & StateCharacter.Attacking) != 0)
                        switch (AttackNumber)
                        {
                            case 0:
                                SetAnimation(Direction == Direction.Right ? "AttackSlash1Right" : "AttackSlash1Left");
                                break;
                            case 1:
                                SetAnimation(Direction == Direction.Right ? "AttackSlash2Right" : "AttackSlash2Left");
                                break;
                            case 2:
                                SetAnimation(Direction == Direction.Right ? "AttackPierceRight" : "AttackPierceLeft");
                                break;
                        }
                    else if ((State & StateCharacter.Attacking) != 0)
                        SetAnimation(Direction == Direction.Right ? "AttackRunRight" : "AttackRunLeft");
                    break;
                case 2:
                    if ((State & StateCharacter.Standing) != 0 && (State & StateCharacter.Attacking) != 0)
                        switch (AttackNumber)
                        {
                            case 0:
                                SetAnimation(Direction == Direction.Right ? "Attack1Right" : "Attack1Left");
                                break;
                        }
                    else if ((State & StateCharacter.Attacking) != 0)
                        SetAnimation(Direction == Direction.Right ? "AttackRunRight" : "AttackRunLeft");
                    break;
            }
        }
    }
}
