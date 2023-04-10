using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using MainGame.Models;
using Microsoft.Xna.Framework;

namespace MainGame
{
    public class EnemyAI
    {
        private Dictionary<int, IGameObject> _objects;
        private int _playerId;

        public void Update(GameTime elapsedTime)
        {
            foreach (var o in _objects
                         .Where(o => o.Key != _playerId && o.Value is Character)
                         .Select(o => (o.Key, o.Value as Character)))
            {
                Moved?.Invoke(this, new MoveEventArgs() { Id = o.Key, Speed = 2, Dir = o.Item2.Direction });
                //Attacked?.Invoke(this, new AttackEventArgs() { Id = o.Key });
            }
            CycleFinished?.Invoke(this, new CycleEventArgs() { ElapsedTime = elapsedTime });
        }

        public void LoadParameters(Dictionary<int, IGameObject> objects, int playerId)
        {
            _objects = objects;
            _playerId = playerId;
        }

        public event EventHandler<MoveEventArgs> Moved;
        public event EventHandler<AttackEventArgs> Attacked;
        public event EventHandler<CycleEventArgs> CycleFinished;
    }
}
