<<<<<<< HEAD
﻿using System;
using UnityEngine;
using System.Collections.Generic;

public class CaveGenerator
{
    private int _caveHeight = 0;

    private LevelMap _map;
    private CellularAutomaton _ca;

    public CaveGenerator( int height, List<Cell> mapBase )
    {
        _caveHeight = height;
        _map = new LevelMap( _caveHeight );
        _map[0] = mapBase;

        // TODO: create CA
    }

    void Generate()
    {
        for ( int i = 0; i < _caveHeight; i++ )
        {
            _map[i] = new List<Cell>( _map[i - 1] );
            _map[-i] = new List<Cell>( _map[i - 1] );


            // TODO: call CA
        }
    }
=======
﻿using UnityEngine;
using System.Collections;

public class CaveGenerator
{
  private LevelMap _map;

  public CaveGenerator(int[,] middle)
  {
    _map = new LevelMap(middle, 10);
  }
>>>>>>> 071722b368a0f1b77fb7b1f16d2fa98e964ae1ab
}