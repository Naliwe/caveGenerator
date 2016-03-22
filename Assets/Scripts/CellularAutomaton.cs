using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CellularAutomaton
{
    #region DefaultSelectors

    public static readonly Predicate<Cell> Black = cell => cell.Type == 1;

    #endregion

    public delegate Cell PickTarget( Cell origin );

    private Predicate<Cell> _selector, _canGrow;
    private PickTarget _pickTarget;
    private LevelMap _map;

    public CellularAutomaton( LevelMap map, Predicate<Cell> selector, Predicate<Cell> canGrow, PickTarget pickTarget )
    {
        _map = map;
        _selector = selector;
        _pickTarget = pickTarget;
        _canGrow = canGrow;
    }

    public void Process( int height )
    {
        var selectedCells = _map.Map.Where( e => _selector( e ) && e.Position.tileY == height );

        foreach ( var cell in selectedCells )
        {
            if ( !_canGrow( cell ) )
                continue;
            var target = _pickTarget( cell );

            if ( target != null )
                cell.GrowTo( target );
        }
    }

    public IEnumerable<Cell> GetNeighbors( Cell cell )
    {
        return ( from c in _map.Map
                 where
                     ( c.Position.tileX == cell.Position.tileX - 1 && c.Position.tileZ == cell.Position.tileZ &&
                       c.Position.tileY == cell.Position.tileY ||
                       c.Position.tileX == cell.Position.tileX + 1 && c.Position.tileZ == cell.Position.tileZ &&
                       c.Position.tileY == cell.Position.tileY ||
                       c.Position.tileX == cell.Position.tileX && c.Position.tileZ == cell.Position.tileZ - 1 &&
                       c.Position.tileY == cell.Position.tileY ||
                       c.Position.tileX == cell.Position.tileX && c.Position.tileZ == cell.Position.tileZ + 1 &&
                       c.Position.tileY == cell.Position.tileY )
                 select c
               );
    }
}