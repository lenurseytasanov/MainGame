using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using MainGame.Models.GameObjects;
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
                         .Select(o => (o.Key, Value: o.Value as Character)))
            {
                var chr = o.Value;
                Moved?.Invoke(this,
                    new MoveEventArgs()
                    {
                        Id = o.Key,
                        Dir = chr.Position.X <= _objects[_playerId].Position.X ? Direction.Right : Direction.Left
                    });
                if (chr.PhysicalBound.Intersects((_objects[_playerId] as Character).PhysicalBound))
                    Attacked?.Invoke(this, new AttackEventArgs() { Id = o.Key });
            }
        }

        public void LoadParameters(Dictionary<int, IGameObject> objects, int playerId)
        {
            _objects = objects;
            _playerId = playerId;
        }

        public event EventHandler<MoveEventArgs> Moved;
        public event EventHandler<AttackEventArgs> Attacked;
    }
}
