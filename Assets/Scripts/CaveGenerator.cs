using UnityEngine;
using System.Collections;

public class CaveGenerator
{
  private LevelMap _map;

  public CaveGenerator(int[,] middle)
  {
    _map = new LevelMap(middle, 10);
  }
}