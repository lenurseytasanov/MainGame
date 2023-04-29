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
            foreach (var pair in _objects
                         .Where(pair => pair.Value is IControllable)
                         .Select(pair => (pair.Key, pair.Value as IControllable)))
            {
                var obj = pair.Item2;
                var player = _objects[_playerId] as Character;
                switch (obj.AI)
                {
                    case AIType.Warrior:
                        Moved?.Invoke(this,
                            new MoveEventArgs()
                            {
                                Id = pair.Key,
                                Dir = obj.Position.X <= player.Position.X ? Direction.Right : Direction.Left
                            });
                        if (obj.Size.Intersects(player.PhysicalBound))
                            MeleeAttack?.Invoke(this, new AttackEventArgs() { Id = pair.Key });
                        break;
                    case AIType.Ranger:
                        if (Vector2.Distance(obj.Position, player.Position) < 400)
                            Moved?.Invoke(this,
                                new MoveEventArgs()
                                {
                                    Id = pair.Key,
                                    Dir = obj.Position.X <= player.Position.X ? Direction.Left : Direction.Right
                                });
                        else
                            RangeAttack?.Invoke(this, new AttackEventArgs() { Id = pair.Key });
                        break;
                }
            }
        }

        public void LoadParameters(Dictionary<int, IGameObject> objects, int playerId)
        {
            _objects = objects;
            _playerId = playerId;
        }

        public event EventHandler<MoveEventArgs> Moved;
        public event EventHandler<AttackEventArgs> MeleeAttack;
        public event EventHandler<AttackEventArgs> RangeAttack;
    }
}
