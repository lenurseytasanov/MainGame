using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame
{
    public class PhysicalModel
    {
        private Dictionary<int, IGameObject> _objects;
        private int _playerId;

        private int FieldWidth;
        private int FieldHeight;

        public void Initialize()
        {
            FieldHeight = 500;
            FieldWidth = 500;

            _objects = new Dictionary<int, IGameObject>();
            _objects.Add(1, new Knight() { Position = new Vector2(250, 250), Speed = Vector2.Zero });
            _playerId = 1;
        }

        public void MovePlayer(ControlEventArgs.Direction direction)
        {
            var player = _objects[_playerId] as Knight;
            switch (direction)
            {
                case ControlEventArgs.Direction.Left:
                    player.Speed += new Vector2(-2, 0);
                    break;
                case ControlEventArgs.Direction.Right:
                    player.Speed += new Vector2(2, 0);
                    break;
                case ControlEventArgs.Direction.Up:
                    player.Speed += new Vector2(0, -2);
                    break;
                case ControlEventArgs.Direction.Down:
                    player.Speed += new Vector2(0, 2);
                    break;
            }

        }

        public void Update()
        {
            foreach (var gameObject in _objects)
            {
                gameObject.Value.Update();
            }

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = _objects });
        }

        public event EventHandler<ObjectsEventArgs> Updated;
    }
}
