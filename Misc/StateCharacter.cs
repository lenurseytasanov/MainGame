using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainGame.Misc
{
    [Flags]
    public enum StateCharacter
    {
        None = 0,
        Dead = 1 << 1,
        Hurt = 1 << 2,
        OnGround = 1 << 3,
        Attacking = 1 << 4,
        Standing = 1 << 5,
    }
}
