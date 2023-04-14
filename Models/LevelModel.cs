using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Managers;
using Microsoft.Xna.Framework;

namespace MainGame.Models
{
    public class LevelModel
    {
        private char[,] _tiles;

        private int _columns;
        private int _rows;

        private readonly int _tileSize = 100;

        private readonly LoadLevelsManager _loadManager;
        
        public string Name { get; set; }
        public string LoadPath { get; set; }

        public Dictionary<int, IGameObject> Objects { get; }
        public int PlayerId { get; private set; }

        public int FieldWidth => _columns * _tileSize;
        public int FieldHeight => _rows * _tileSize;

        public LevelModel()
        {
            Objects = new Dictionary<int, IGameObject>();
            _loadManager = new LoadLevelsManager();
            _tiles = new char[_rows, _columns];
        }

        public void Initialize()
        {
            _tiles = _loadManager.LoadLevel(LoadPath);
            _rows = _tiles.GetLength(0);
            _columns = _tiles.GetLength(1);

            var currentId = 1;
            for (var c = 0; c < _columns; c++)
            {
                for (var r = 0; r < _rows; r++)
                {
                    if (_tiles[r, c] == ' ') continue;

                    if (_tiles[r, c] == 'P')
                        PlayerId = currentId;

                    Objects.Add(currentId, CreateGameObject(_tiles[r, c], c, r));
                    currentId++;
                }
            }
        }

        public IGameObject CreateGameObject(char sign, int i, int j) => sign switch
        {
            'E' => new Character()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                Speed = Vector2.Zero,
                SpriteId = 2,
                HealthPoints = 1,
                AttackCount = 1,
                PhysicalBound = new Rectangle(i * _tileSize + _tileSize * 2 / 3, j * _tileSize + _tileSize * 2 / 3, _tileSize * 2 / 3, _tileSize * 4 / 3),
                Size = new Rectangle(i * _tileSize, j * _tileSize, _tileSize * 2, _tileSize * 2)
            },
            'P' => new Character()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                Speed = Vector2.Zero,
                SpriteId = 1,
                AttackCount = 3,
                PhysicalBound = new Rectangle(i * _tileSize + _tileSize * 2 / 3, j * _tileSize + _tileSize * 2 / 3, _tileSize * 2 / 3, _tileSize * 4 / 3),
                Size = new Rectangle(i * _tileSize, j * _tileSize, _tileSize * 2, _tileSize * 2)
            },
            _ => new StaticSolidObject()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                SpriteId = sign switch
                {
                    'U' => 12,
                    'D' => 13,
                    'G' => 14,
                    'L' => 15,
                    'R' => 16,
                    'K' => 17,
                    'O' => 18,
                    'C' => 19,
                    'Y' => 20
                },
                PhysicalBound = new Rectangle(i * _tileSize, j * _tileSize + _tileSize / 4, _tileSize, _tileSize * 3 / 4),
                Size = new Rectangle(i * _tileSize, j * _tileSize, _tileSize, _tileSize)
            }
        };
    }
}
