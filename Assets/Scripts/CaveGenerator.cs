using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

public class CaveGenerator
{
    private int _caveHeight;

    private LevelMap _map;
    private CellularAutomaton _ca;

    public CaveGenerator(int height, int[,] mapBase)
    {
        _caveHeight = height * 2;
        _map = new LevelMap(mapBase, _caveHeight);

        var random = new Random();

        _ca = new CellularAutomaton(_map, CellularAutomaton.Black,
                                     cell =>
                                     (random.Next(100) >= 50 - (Math.Abs(cell.Position.tileY) / _map.Height) * 50),
                                     origin => _ca.GetNeighbors(origin).FirstOrDefault(cell => cell.Type == 0));
    }

    public LevelMap Generate()
    {
        for (int i = (_caveHeight / 2) + 1, j = (_caveHeight / 2) - 1; i < _caveHeight; ++i, --j)
        {
            for (int x = 0; x < _map.Width; x++)
                for (int z = 0; z < _map.Depth; z++)
                {
                    _map.Map.Add(new Cell(x, i, z, _map[x, i - 1, z].Type));
                    _map.Map.Add(new Cell(x, j, z, _map[x, j + 1, z].Type));
                }
            _ca.Process(i);
            _ca.Process(j);
        }

        return (_map);
    }
}