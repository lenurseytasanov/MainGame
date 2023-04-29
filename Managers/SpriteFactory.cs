using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Screens;
using MainGame.Sprites;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace MainGame.Managers
{
    public abstract class SpriteFactory
    {
        protected ContentManager Content;

        protected SpriteFactory(ContentManager content)
        {
            Content = content;
        }

        public abstract Sprite CreateSprite(int spriteId);
    }

    public class CharacterSpriteFactory : SpriteFactory
    {
        public CharacterSpriteFactory(ContentManager content) : base(content) { }

        public override Sprite CreateSprite(int spriteId)
        {
            var sprite = new CharacterSprite()
            {
                ShiftWhenFlipped = spriteId == 1 ? new Vector2(50, 0) : Vector2.Zero,
                LayerDepth = 0.5f,
                AttackCount = spriteId switch
                {
                    1 => 3,
                    2 => 1,
                    3 => 1
                },
                Animations = spriteId switch
                {
                    1 => new Dictionary<string, Animation>()
                    {
                        {
                            "IdleRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4,
                            }
                        },
                        {
                            "IdleLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Idle"), FrameCount = 4,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "RunRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7,
                            }
                        },
                        {
                            "RunLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Run"), FrameCount = 7,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "JumpRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6,
                            }
                        },
                        {
                            "JumpLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Jump"), FrameCount = 6,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "AttackSlash1Right",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                            }
                        },
                        {
                            "AttackSlash1Left",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Attack 1"), FrameCount = 5,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "AttackSlash2Right",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Attack 2"), FrameCount = 4
                            }
                        },
                        {
                            "AttackSlash2Left",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Attack 2"), FrameCount = 4,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "AttackPierceRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Attack 3"), FrameCount = 4,
                            }
                        },
                        {
                            "AttackPierceLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Attack 3"), FrameCount = 4,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "AttackRunRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Run+Attack"), FrameCount = 6,
                            }
                        },
                        {
                            "AttackRunLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Run+Attack"), FrameCount = 6,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "HurtRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Hurt"), FrameCount = 2,
                            }
                        },
                        {
                            "HurtLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Hurt"), FrameCount = 2,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "DeadRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Dead"), FrameCount = 6,
                                IsLooping = false
                            }
                        },
                        {
                            "DeadLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Knight_1/Dead"), FrameCount = 6,
                                IsLooping = false,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                    },
                    2 => new Dictionary<string, Animation>()
                    {
                        {
                            "IdleRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Idle"), FrameCount = 5
                            }
                        },
                        {
                            "IdleLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Idle"), FrameCount = 5,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "RunRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Run"), FrameCount = 6
                            }
                        },
                        {
                            "RunLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Run"), FrameCount = 6,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "JumpRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Jump"), FrameCount = 5
                            }
                        },
                        {
                            "JumpLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Jump"), FrameCount = 5,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "HurtRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Hurt"), FrameCount = 2
                            }
                        },
                        {
                            "HurtLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Hurt"), FrameCount = 2,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "DeadRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Dead"), FrameCount = 4,
                                IsLooping = false
                            }
                        },
                        {
                            "DeadLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Dead"), FrameCount = 4,
                                IsLooping = false,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "AttackRunRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Run+Attack"), FrameCount = 5,
                            }
                        },
                        {
                            "AttackRunLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Run+Attack"), FrameCount = 5,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "Attack1Right",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Attack_1"), FrameCount = 4,
                            }
                        },
                        {
                            "Attack1Left",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Berserk/Attack_1"), FrameCount = 4,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                    },
                    3 => new Dictionary<string, Animation>()
                    {
                        {
                            "IdleRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Idle"), FrameCount = 5
                            }
                        },
                        {
                            "IdleLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Idle"), FrameCount = 5,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "RunRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Run"), FrameCount = 6
                            }
                        },
                        {
                            "RunLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Run"), FrameCount = 6,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "JumpRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Jump"), FrameCount = 6,
                            }
                        },
                        {
                            "JumpLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Jump"), FrameCount = 6,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "HurtRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Hurt"), FrameCount = 2
                            }
                        },
                        {
                            "HurtLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Hurt"), FrameCount = 2,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "DeadRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Dead"), FrameCount = 5,
                                IsLooping = false
                            }
                        },
                        {
                            "DeadLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Dead"), FrameCount = 5,
                                IsLooping = false,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                        {
                            "AttackMagicRight",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Magic_2"), FrameCount = 6,
                            }
                        },
                        {
                            "AttackMagicLeft",
                            new Animation()
                            {
                                SpriteSheet = Content.Load<Texture2D>("Orc/Orc_Shaman/Magic_2"), FrameCount = 6,
                                Effects = SpriteEffects.FlipHorizontally
                            }
                        },
                    },
                }
            };
            return sprite;
        }
    }

    public class StaticSpriteFactory : SpriteFactory
    {
        public StaticSpriteFactory(ContentManager content) : base(content) { }

        public override Sprite CreateSprite(int spriteId)
        {
            return new Sprite()
            {
                LayerDepth = 0.25f,
                Texture = Content.Load<Texture2D>(spriteId switch
                {
                    12 => "dungeon/Tiles_rock/tile2",
                    13 => "dungeon/Tiles_lava/lava_tile3",
                    14 => "dungeon/Tiles_rock/tile5",
                    15 => "dungeon/Tiles_rock/tile1",
                    16 => "dungeon/Tiles_rock/tile3",
                    17 => "dungeon/Tiles_rock/tile4",
                    18 => "dungeon/Tiles_rock/tile6",
                    19 => "dungeon/Tiles_rock/tile12",
                    20 => "dungeon/Tiles_rock/tile13",
                    50 => "Orc/Orc_Shaman/Fireball",
                    _ => throw new Exception("Unknown texture")
                })
            };
        }
    }

    public class BarsFactory : SpriteFactory
    {
        public BarsFactory(ContentManager content) : base(content) { }

        public override Sprite CreateSprite(int spriteId)
        {
            var sprite = new StateSprite()
            {
                Position = new Vector2(20, 20),
                LayerDepth = 0.12f,
                States = new Dictionary<int, Texture2D>()
                {
                    { 0, Content.Load<Texture2D>("bars/healthbar0") },
                    { 1, Content.Load<Texture2D>("bars/healthbar1") },
                    { 2, Content.Load<Texture2D>("bars/healthbar2") },
                    { 3, Content.Load<Texture2D>("bars/healthbar3") },
                    { 4, Content.Load<Texture2D>("bars/healthbar4") },
                    { 5, Content.Load<Texture2D>("bars/healthbar5") },
                    { 6, Content.Load<Texture2D>("bars/healthbar6") },
                    { 7, Content.Load<Texture2D>("bars/healthbar7") },
                    { 8, Content.Load<Texture2D>("bars/healthbar8") },
                    { 9, Content.Load<Texture2D>("bars/healthbar9") },
                    { 10, Content.Load<Texture2D>("bars/healthbar10") }
                }
            };
            return sprite;
        }
    }

    public class BackgroundFactory : SpriteFactory
    {
        public BackgroundFactory(ContentManager content) : base(content) { }

        public override Sprite CreateSprite(int spriteId)
        {
            return new BackgroundSprite()
            {
                Position = Vector2.Zero,
                LayerDepth = 1,
                Texture = Content.Load<Texture2D>("dungeon/Background/Pale/Background")
            };
        }
    }
}
