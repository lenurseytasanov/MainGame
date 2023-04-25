using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MainGame.Managers;
using Microsoft.Xna.Framework;
using MainGame.Models.GameObjects;

namespace MainGame.Models
{
    public class LevelFactory
    {
        private char[,] _tiles;

        private int _columns;
        private int _rows;

        private readonly int _tileSize = 100;

        private readonly LoadLevelsManager _loadManager;
        
        public string Name { get; set; }
        public string LoadPath { get; set; }

        public int PlayerId { get; private set; }

        public int FieldWidth => _columns * _tileSize;
        public int FieldHeight => _rows * _tileSize;

        public LevelFactory()
        {
            _loadManager = new LoadLevelsManager();
        }

        public void Initialize()
        { }

        public Dictionary<int, IGameObject> CreateGameObjects()
        {
            var objects = new Dictionary<int, IGameObject>();
            _tiles = new char[_rows, _columns];
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

                    objects.Add(currentId, GetObject(_tiles[r, c], c, r));
                    currentId++;
                }
            }
            return objects;
        }

        private IGameObject GetObject(char sign, int i, int j) => sign switch
        {
            'E' => new Character()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                Speed = Vector2.Zero,
                SpriteId = 2,
                HealthPoints = 1,
                Damage = 2,
                AttackCount = 1,
                Mass = 2.0f,
                Acceleration = 2.0f,
                PhysicalBound = new Rectangle(i * _tileSize + _tileSize * 2 / 3, j * _tileSize + _tileSize * 2 / 3, _tileSize * 2 / 3, _tileSize * 4 / 3),
                Size = new Rectangle(i * _tileSize, j * _tileSize, _tileSize * 2, _tileSize * 2)
            },
            'P' => new Character()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                Speed = Vector2.Zero,
                SpriteId = 1,
                HealthPoints = 10,
                Damage = 2,
                AttackCount = 3,
                PhysicalBound = new Rectangle(i * _tileSize + _tileSize * 2 / 3, j * _tileSize + _tileSize * 2 / 3, _tileSize * 2 / 3, _tileSize * 4 / 3),
                Size = new Rectangle(i * _tileSize, j * _tileSize, _tileSize * 2, _tileSize * 2)
            },
            'L' => new Lava()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                SpriteId = 13,
                HitBox = new Rectangle(i * _tileSize, j * _tileSize + _tileSize / 4, _tileSize, _tileSize * 3 / 4),
                Size = new Rectangle(i * _tileSize, j * _tileSize, _tileSize, _tileSize),
            },
            'G' => new Ground()
            {
                Position = new Vector2(i * _tileSize, j * _tileSize),
                SpriteId = GetGroundStateId(sign, i, j),
                PhysicalBound = new Rectangle(i * _tileSize, j * _tileSize + _tileSize / 4, _tileSize, _tileSize * 3 / 4),
                Size = new Rectangle(i * _tileSize, j * _tileSize, _tileSize, _tileSize)
            }
        };

        private int GetGroundStateId(char ground, int c, int r)
        {
            if (r > 0 && _tiles[r - 1, c] == ' ')
            {
                if (c > 0 && _tiles[r, c - 1] == ' ')
                    return 15;
                if (c < _columns - 1 && _tiles[r, c + 1] == ' ')
                    return 16;
                return 12;
            }
            if (r > 0 && c > 0 && _tiles[r, c - 1] != ' ' && _tiles[r - 1, c] != ' ' && _tiles[r - 1, c - 1] == ' ')
                return 20;
            if (r > 0 && c < _columns - 1 && _tiles[r, c + 1] != ' ' && _tiles[r - 1, c] != ' ' && _tiles[r - 1, c + 1] == ' ')
                return 19;
            if (c > 0 && _tiles[r, c - 1] == ' ')
                return 17;
            if (c < _columns - 1 && _tiles[r, c + 1] == ' ')
                return 18;
            return 14;
        }
    }
}
