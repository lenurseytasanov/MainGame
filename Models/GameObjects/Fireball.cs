using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models.GameObjects;

public class Fireball : DynamicObject, IDamaging
{
    public int Damage { get; set; } = 1;

    public Rectangle HitBox { get; set; }

    public override void Update(GameTime gameTime)
    {
        HitBox = Size;

        base.Update(gameTime);
    }
}