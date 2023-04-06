using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class LevelModel
    {
        private readonly char[,] _tiles;

        private readonly int _columns = 10;
        private readonly int _rows = 4;
        private readonly int _tileSize = 100;

        public Dictionary<int, IGameObject> Objects { get; private set; }
        public int PlayerId { get; private set; }

        public int FieldWidth => _columns * _tileSize;
        public int FieldHeight => _rows * _tileSize;

        public LevelModel()
        {
            Objects = new Dictionary<int, IGameObject>();
            _tiles = new char[_columns, _rows];
        }

        public void Initialize()
        {
            for (var i = 0; i < _columns; i++)
                _tiles[i, _rows - 1] = 'G';

            _tiles[2, 1] = 'P';
            _tiles[3, 1] = 'E';

            var currentId = 1;
            for (var i = 0; i < _columns; i++)
            {
                for (var j = 0; j < _rows; j++)
                {
                    if (_tiles[i, j] == '\0') continue;

                    if (_tiles[i, j] == 'P')
                        PlayerId = currentId;

                    Objects.Add(currentId, CreateGameObject(_tiles[i, j], i, j));
                    currentId++;
                }
            }
        }

        public IGameObject CreateGameObject(char sign, int i, int j) => sign switch
        {
            'P' or 'E' => new Knight()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                Speed = Vector2.Zero,
                SpriteId = 1,
                PhysicalBound = new Rectangle(i * _tileSize, j * _tileSize, 50, 100)
            },
            'G' => new StaticSolidObject()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                SpriteId = 2,
                PhysicalBound = new Rectangle(i * _tileSize, j * _tileSize, 100, 100)
            },
            _ => throw new Exception($"Unknown object: {sign}")
        };
    }
}
