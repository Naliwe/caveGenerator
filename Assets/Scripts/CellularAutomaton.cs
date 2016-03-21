using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CellularAutomaton
{
    #region DefaultSelectors

    public static readonly Predicate<Cell> All = cell => true;
    public static readonly Predicate<Cell> Black = cell => cell.Type == 1;

    #endregion

    private Predicate<Cell> _selector, _canGrow;

    public delegate Cell PickTarget( Cell origin );

    private PickTarget _pickTarget;

    public CellularAutomaton( Predicate<Cell> selector, Predicate<Cell> canGrow, PickTarget pickTarget )
    {
        _selector = selector;
        _pickTarget = pickTarget;
        _canGrow = canGrow;
    }

    public void Process( List<Cell> map )
    {
        var selectedCells = map.Where( e => _selector( e ) );

        foreach ( var cell in selectedCells )
        {
            if ( !_canGrow( cell ) )
                continue;
            var target = _pickTarget( cell );

            if ( map.Contains( target ) )
                cell.GrowTo( map.Find( t => t.Equals( target ) ) );
        }
    }

    public List<Cell> GetNeighbors( List<Cell> map, Cell cell )
    {
        return ( from c in map
                 where ( c.Position.tileX == cell.Position.tileX - 1 && c.Position.tileY == cell.Position.tileY
                         || c.Position.tileX == cell.Position.tileX + 1 && c.Position.tileY == cell.Position.tileY
                         || c.Position.tileX == cell.Position.tileX && c.Position.tileY == cell.Position.tileY - 1
                         || c.Position.tileX == cell.Position.tileX && c.Position.tileY == cell.Position.tileY + 1 )
                 select c
               ) as List<Cell>;

    }
}