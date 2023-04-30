using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using MainGame.Models.GameObjects;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class EnemyAI
    {
        private Dictionary<int, IGameObject> _objects;
        private int _playerId;

        public void Update(GameTime elapsedTime)
        {
            foreach (var pair in _objects
                         .Where(pair => pair.Value is IControllable)
                         .Select(pair => (pair.Key, pair.Value as Character)))
            {
                var obj = pair.Item2;
                var player = _objects[_playerId] as Character;
                switch (obj.AI)
                {
                    case AIType.Warrior:
                        if (Vector2.Distance(obj.HitBox.Center.ToVector2(), player.HitBox.Center.ToVector2()) > obj.HitBox.Width * 5 / 6)
                            Moved?.Invoke(this,
                                new MoveEventArgs()
                                {
                                    Id = pair.Key,
                                    Dir = obj.HitBox.Center.X <= player.HitBox.Center.X ? Direction.Right : Direction.Left
                                });
                        if (obj.HitBox.Intersects(player.HitBox))
                            MeleeAttack?.Invoke(this, new AttackEventArgs() { Id = pair.Key });
                        break;
                    case AIType.Ranger:
                        if (Vector2.Distance(obj.HitBox.Center.ToVector2(), player.HitBox.Center.ToVector2()) < 300)
                            Moved?.Invoke(this,
                                new MoveEventArgs()
                                {
                                    Id = pair.Key,
                                    Dir = obj.HitBox.Center.X <= player.HitBox.Center.X ? Direction.Left : Direction.Right
                                });
                        else
                            RangeAttack?.Invoke(this, new AttackEventArgs() { Id = pair.Key });
                        break;
                }
            }
        }

        public void LoadParameters(LevelModel level)
        {
            _objects = level.Objects;
            _playerId = level.PlayerId;
        }

        public event EventHandler<MoveEventArgs> Moved;
        public event EventHandler<AttackEventArgs> MeleeAttack;
        public event EventHandler<AttackEventArgs> RangeAttack;
    }
}
