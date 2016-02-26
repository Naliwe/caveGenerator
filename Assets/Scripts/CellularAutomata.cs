using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CellularAutomata
{
    private Predicate<Cell> _selector, _canGrow;

    public delegate Cell PickTarget(Cell origin);

    private PickTarget _pickTarget;

    private List<Cell> _map;

    public CellularAutomata(List<Cell> map, Predicate<Cell> selector, Predicate<Cell> canGrow, PickTarget pickTarget)
    {
        _map = map;
        _selector = selector;
        _pickTarget = pickTarget;
        _canGrow = canGrow;
    }

    public List<Cell> Process()
    {
        var selectedCells = _map.Where(e => _selector(e));

        foreach (Cell cell in selectedCells)
        {
            if (_canGrow(cell))
            {
                Cell target = _pickTarget(cell);

                if (_map.Contains(target))
                    cell.GrowTo(_map.Find(t => t.Equals(target)));
            }
        }

        return (_map);
    }
}
