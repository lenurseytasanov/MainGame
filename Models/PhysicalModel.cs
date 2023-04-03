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
            FieldWidth = 700;

            _objects = new Dictionary<int, IGameObject>
            {
                { 1, new Knight() { Position = new Vector2(250, 100), Speed = Vector2.Zero, SpriteId = 1 } },
                { 2, new Ground() { Position = new Vector2( -1000, FieldHeight), SpriteId = 2 }}
            };
            _playerId = 1; //danger
        }

        public void MovePlayer(Direction direction)
        {
            var player = _objects[_playerId] as Knight;
            switch (direction)
            {
                case Direction.Left:
                    player.Forces += new Vector2(-2, 0);
                    break;
                case Direction.Right:
                    player.Forces += new Vector2(2, 0);
                    break;
                case Direction.Up:
                    if (player.OnGround) 
                        player.Forces += new Vector2(0, -20);
                    break;
            }
        }

        public void Update()
        {
            foreach (var gameObject in _objects.Values)
            {
                if (gameObject.Speed.X + gameObject.Position.X < 0 ||
                    gameObject.Speed.X + gameObject.Position.X > FieldWidth)
                    gameObject.Speed = new Vector2(0, gameObject.Speed.Y);
                if (gameObject is Solid solid)
                {
                    foreach (var other in _objects.Values.OfType<Solid>().Where(other => !other.Equals(gameObject)))
                    {
                        if (solid.IsTouchTop(other))
                        {
                            solid.Speed = new Vector2(solid.Speed.X, 0);
                        }
                        if (solid.IsTouchBottom(other))
                        {
                            while (solid.IsTouchBottom(other))
                                gameObject.Speed = new Vector2(gameObject.Speed.X, gameObject.Speed.Y + 0.5f);
                            gameObject.Speed = new Vector2(gameObject.Speed.X, 0);
                        }
                        if (solid.IsTouchLeft(other))
                        {
                            while (solid.IsTouchLeft(other))
                                gameObject.Speed = new Vector2(gameObject.Speed.X - 0.5f, gameObject.Speed.Y);
                            gameObject.Speed = new Vector2(0, gameObject.Speed.Y);
                        }
                        if (solid.IsTouchRight(other))
                        {
                            while (solid.IsTouchRight(other))
                                gameObject.Speed = new Vector2(gameObject.Speed.X + 0.5f, gameObject.Speed.Y);
                            gameObject.Speed = new Vector2(0, gameObject.Speed.Y);
                        }

                    }   
                }

                gameObject.Update();
            }

            (_objects[_playerId] as Knight)!.OnGround = (_objects[_playerId] as Knight)!.IsTouchTop(_objects[2] as Solid);

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = _objects });
        }

        public event EventHandler<ObjectsEventArgs> Updated;
    }
}
