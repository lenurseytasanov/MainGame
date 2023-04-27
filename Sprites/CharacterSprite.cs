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

        public int AttackCount { get; set; }

        public StateCharacter State { get; set; }

        public Vector2 ShiftWhenFlipped { get; set; } = Vector2.Zero;

        public override void Draw(SpriteBatch spriteBatch, Vector2 shift)
        {
            shift.X += (AnimationManager.CurrentAnimation.Effects & SpriteEffects.FlipHorizontally) != 0
                ? -(int)ShiftWhenFlipped.X
                : (int)ShiftWhenFlipped.X;
            shift.Y += (AnimationManager.CurrentAnimation.Effects & SpriteEffects.FlipVertically) != 0
                ? -(int)ShiftWhenFlipped.Y
                : (int)ShiftWhenFlipped.Y;

            spriteBatch.Draw(
                AnimationManager.CurrentAnimation.SpriteSheet,
                new Rectangle(
                    (int)shift.X + (int)Position.X,
                    (int)shift.Y + (int)Position.Y,
                    Size.Width, Size.Height),
                AnimationManager.CurrentFrame, Color.White,
                0, Vector2.Zero,
                AnimationManager.CurrentAnimation.Effects, LayerDepth);
        }

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
