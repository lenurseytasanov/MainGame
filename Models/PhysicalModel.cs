using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using MainGame.Misc;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class PhysicalModel
    {
        public Dictionary<int, IGameObject> Objects { get; private set; }
        private int _playerId;

        private LevelModel _level;

        public void Initialize(LevelModel level)
        {
            _level = level;
            Objects = _level.Objects;
            _playerId = _level.PlayerId;

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = Objects, PlayerId = _playerId });
        }

        public void Move(int id, float speed, Direction direction)
        {
            var player = Objects[id] as MovingSolidObject;
            if (player is Character { HealthPoints: <= 0 }) return; 
            switch (direction)
            {
                case Direction.Left:
                    if (player.IsOnGround)
                        player.Forces += new Vector2(-speed, 0);
                    else
                        player.Forces += new Vector2(-speed / 10, 0);
                    break;
                case Direction.Right:
                    if (player.IsOnGround)
                        player.Forces += new Vector2(speed, 0);
                    else
                        player.Forces += new Vector2(speed / 10, 0);
                    break;
                case Direction.Up:
                    if (player.IsOnGround)
                        player.Forces += new Vector2(0, -20);
                    break;
            }
        }

        public void Attack(int id)
        {
            var chr = Objects[id] as Character;
            chr.Attack = (Attack)(1 + ((int)chr.PreviousAttack + 1) % (Enum.GetNames(typeof(Attack)).Length - 1));
        }

        public void Update(GameTime gameTime)
        {
            foreach (var movingObj in Objects.Values.OfType<MovingSolidObject>())
            {
                movingObj.Update(gameTime);

                var onGround = false;
                foreach (var staticObj in Objects.Values.OfType<StaticSolidObject>())
                {
                    if (movingObj.IsTouchTop(staticObj))
                    {
                        onGround = true;
                        movingObj.Speed = new Vector2(movingObj.Speed.X, 0);
                    }
                    if (movingObj.IsTouchBottom(staticObj))
                        movingObj.Speed = new Vector2(movingObj.Speed.X, 0);
                    if (movingObj.IsTouchLeft(staticObj))
                        movingObj.Speed = new Vector2(0, movingObj.Speed.Y);
                    if (movingObj.IsTouchRight(staticObj))
                        movingObj.Speed = new Vector2(0, movingObj.Speed.Y);
                }
                movingObj.IsOnGround = onGround;

                if (movingObj is not Character chr || chr.Attack == Misc.Attack.None) continue;

                foreach (var chr1 in Objects.Values.OfType<Character>().Where(o => !o.Equals(chr)
                             && o.HealthPoints > 0 
                             && chr.PhysicalBound.Intersects(o.PhysicalBound) 
                             && !chr.DamagedObjects.Contains(o)))
                {
                    chr1.HealthPoints -= 2;
                    chr1.IsHurt = true;
                    chr1.Forces += new Vector2(chr.Position.X < chr1.Position.X ? 5 : -10, -10);
                    chr.DamagedObjects.Add(chr1);
                }
            }

            Updated?.Invoke(this, new ObjectsEventArgs() { Objects = Objects, PlayerId = _playerId });
        }

        public event EventHandler<ObjectsEventArgs> Updated;
    }
}
