using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class PhysicalModel
    {
        private Dictionary<int, IGameObject> _objects;
        private int _playerId;

        private LevelModel _level;

        public void Initialize()
        {
            _level = new LevelModel();
            _level.Initialize();
            _objects = _level.Objects;
            _playerId = _level.PlayerId;

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = _objects, PlayerId = _playerId });
        }

        public void MovePlayer(Direction direction)
        {
            var player = _objects[_playerId] as MovingSolidObject;
            switch (direction)
            {
                case Direction.Left:
                    player.Forces += new Vector2(-2, 0);
                    break;
                case Direction.Right:
                    player.Forces += new Vector2(2, 0);
                    break;
                case Direction.Up:
                    if (player.IsOnGround) 
                        player.Forces += new Vector2(0, -20);
                    break;
            }
        }

        public void Attack()
        {
            var rnd = new Random(DateTime.Now.Millisecond);
            var knight = _objects[_playerId] as Knight;
            if (knight.Attack == Misc.Attack.None)
                knight.Attack = (Attack)rnd.Next(1, 4);
        }

        public void Update(GameTime gameTime)
        {
            foreach (var movingObj in _objects.Values.OfType<MovingSolidObject>())
            {
                movingObj.Update(gameTime);

                var onGround = false;
                foreach (var staticObj in _objects.Values.OfType<StaticSolidObject>())
                {
                    if (movingObj.IsTouchTop(staticObj))
                    {
                        onGround = true;
                        movingObj.Speed = new Vector2(movingObj.Speed.X, 0);
                    }
                }
                movingObj.IsOnGround = onGround;

                if (movingObj is not Knight knight || knight.Attack == Misc.Attack.None) continue;

                foreach (var knight1 in _objects.Values.OfType<Knight>().Where(o => !o.Equals(knight)
                             && o.HealthPoints > 0 
                             && knight.PhysicalBound.Intersects(o.PhysicalBound) 
                             && !knight.DamagedObjects.Contains(o)))
                {
                    knight1.HealthPoints -= 2;
                    knight1.IsHurt = true;
                    knight1.Forces += new Vector2(knight.Position.X < knight1.Position.X ? 40 : -40, -20);
                    knight.DamagedObjects.Add(knight1);
                }
            }

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = _objects, PlayerId = _playerId });
        }

        public event EventHandler<ObjectsEventArgs> Updated;
    }
}
