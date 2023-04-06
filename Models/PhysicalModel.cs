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

        //private int FieldWidth;
        private int FieldHeight;

        public void Initialize()
        {
            FieldHeight = 300;

            _objects = new Dictionary<int, IGameObject>
            {
                { 1, new Knight() { Position = new Vector2(250, 100), Speed = Vector2.Zero, SpriteId = 1 } },
                { 3, new Knight() { Position = new Vector2(400, 100), Speed = Vector2.Zero, SpriteId = 1 } },
                { 2, new Ground() { Position = new Vector2( -10000, FieldHeight), SpriteId = 2 }}
            };
            _playerId = 1;

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = _objects, PlayerId = _playerId });
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

        public void Attack()
        {
            (_objects[_playerId] as Knight).IsAttacking = true;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var o in _objects)
            {
                if (o.Value is Knight { HealthPoints: <= 0 })
                    _objects.Remove(o.Key);

                o.Value.Update(gameTime);
                if (o.Value is not Solid solid) continue;

                foreach (var otherSolid in _objects.Values.OfType<Solid>().Where(other => !other.Equals(solid)))
                {
                    if (solid is Knight knight)
                        knight.OnGround = knight.IsTouchTop(otherSolid);

                    if (solid is Knight knight1 && otherSolid is Knight knight2)
                    {
                        //TODO: replace to IsIntersects
                        if (knight1.IsAttacking && knight1.PhysicalBound.Intersects(knight2.PhysicalBound) && !knight1.DamagedObjects.Contains(knight2))
                        {
                            knight2.HealthPoints -= 2;
                            knight2.Forces += new Vector2(knight1.Position.X < knight2.Position.X ? 20 : -20, -20);
                            knight1.DamagedObjects.Add(knight2);
                        }
                        continue;
                    }
                    if (solid.IsTouchTop(otherSolid))
                    {
                        solid.Speed = new Vector2(solid.Speed.X, 0);
                    }
                    if (solid.IsTouchBottom(otherSolid))
                    {
                        while (solid.IsTouchBottom(otherSolid))
                            solid.Speed = new Vector2(solid.Speed.X, solid.Speed.Y + 0.5f);
                        solid.Speed = new Vector2(solid.Speed.X, 0);
                    }
                    if (solid.IsTouchLeft(otherSolid))
                    {
                        while (solid.IsTouchLeft(otherSolid))
                            solid.Speed = new Vector2(solid.Speed.X - 0.5f, solid.Speed.Y);
                        solid.Speed = new Vector2(0, solid.Speed.Y);
                    }
                    if (solid.IsTouchRight(otherSolid))
                    {
                        while (solid.IsTouchRight(otherSolid))
                            solid.Speed = new Vector2(solid.Speed.X + 0.5f, solid.Speed.Y);
                        solid.Speed = new Vector2(0, solid.Speed.Y);
                    }

                }
            }

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = _objects, PlayerId = _playerId });
        }

        public event EventHandler<ObjectsEventArgs> Updated;
    }
}
