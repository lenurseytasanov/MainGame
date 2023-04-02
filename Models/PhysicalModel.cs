using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class PhysicalModel
    {
        private Dictionary<int, IGameObject> _objects;
        private int _playerId;

        private int FieldWidth;
        private int FieldHeight;

        public void Initialize()
        {
            FieldHeight = 300;
            FieldWidth = 300;

            _objects = new Dictionary<int, IGameObject>
            {
                { 1, new Knight() { Position = new Vector2(250, 250), Speed = Vector2.Zero } }
            };
            _playerId = 1; //danger
        }

        public void MovePlayer(Direction direction)
        {
            var player = _objects[_playerId];
            switch (direction)
            {
                case Direction.Left:
                    player.Speed += new Vector2(-1, 0);
                    break;
                case Direction.Right:
                    player.Speed += new Vector2(1, 0);
                    break;
                case Direction.Up:
                    player.Speed += new Vector2(0, -1);
                    break;
                case Direction.Down:
                    player.Speed += new Vector2(0, 1);
                    break;
            }

        }

        public void Update()
        {
            foreach (var gameObject in _objects.Values)
            {
                if (gameObject.Position.X + gameObject.Speed.X < 0 ||
                    gameObject.Position.X + gameObject.Speed.X > FieldWidth)
                    gameObject.Speed = new Vector2(0, gameObject.Speed.Y);
                if (gameObject.Position.Y + gameObject.Speed.Y < 0 ||
                    gameObject.Position.Y + gameObject.Speed.Y > FieldHeight)
                    gameObject.Speed = new Vector2(gameObject.Speed.X, 0);

                gameObject.Update();
            }

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = _objects });
        }

        public event EventHandler<ObjectsEventArgs> Updated;
    }
}
