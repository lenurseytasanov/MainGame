using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;

namespace MainGame.Models.GameObjects;

public interface IControllable : IGameObject
{
    AIType AI { get; set; }
}