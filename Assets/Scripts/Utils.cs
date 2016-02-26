using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Policy;

public class LevelMap
{
    private Dictionary<int, List<Cell>> _map;

    public int Height { get; set; }

    public LevelMap(int height)
    {
        Height = height;
    }

    public LevelMap(int[,] rawMap, int height)
    {
        Height = height;

        List<Cell> tmp = new List<Cell>();

        for (int x = 0; x < rawMap.GetLength(0); x++)
            for (int y = 0; y < rawMap.GetLength(1); y++)
                tmp.Add(new Cell(new Coord(x, y)));
    }

    public List<Cell> this[int layer]
    {
        get { return _map[layer]; }
        set { _map[layer] = value; }
    }
}

public struct Coord
{
    public int tileX;
    public int tileY;

    public Coord(int x, int y)
    {
        tileX = x;
        tileY = y;
    }
}

public class Cell : IEquatable<Cell>
{
    public Coord Position { get; set; }
    public int Type { get; set; }

    public List<Cell> Neighbors
    {
        get
        {
            List<Cell> ret = new List<Cell>
                             {
                                 new Cell( new Coord( Position.tileX - 1, Position.tileY ) ),
                                 new Cell( new Coord( Position.tileX + 1, Position.tileY ) ),
                                 new Cell( new Coord( Position.tileX, Position.tileY - 1 ) ),
                                 new Cell( new Coord( Position.tileX, Position.tileY + 1 ) ),
                             };

            return (ret);
        }
    }

    public Cell(Coord position)
    {
        Position = position;
    }

    public void GrowTo(Cell other)
    {
        other.Type = this.Type;
    }

    public override bool Equals( object other )
    {
        return ( this.Equals( other as Cell ) );
    }

    public bool Equals( Cell other )
    {
        return ( Position.tileY == other.Position.tileX &&
                 Position.tileY == other.Position.tileY );
    }
}


public class Room : IComparable<Room>
{
    public List<Coord> tiles;
    public List<Coord> edgeTiles;
    public List<Room> connectedRooms;
    public int roomSize;
    public bool isAccessibleFromMainRoom;
    public bool isMainRoom;

    public Room()
    {

    }

    public Room(List<Coord> roomTiles, int[,] map)
    {
        tiles = roomTiles;
        roomSize = tiles.Count;
        connectedRooms = new List<Room>();

        edgeTiles = new List<Coord>();
        foreach (Coord tile in tiles)
        {
            for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++)
            {
                for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++)
                {
                    if (x == tile.tileX || y == tile.tileY)
                    {
                        if (map[x, y] == 1)
                        {
                            edgeTiles.Add(tile);
                        }
                    }
                }
            }
        }
    }

    public void SetAccessibleFromMainRoom()
    {
        if (!isAccessibleFromMainRoom)
        {
            isAccessibleFromMainRoom = true;
            foreach (Room connectedRoom in connectedRooms)
            {
                connectedRoom.SetAccessibleFromMainRoom();
            }
        }
    }

    public static void ConnectRooms(Room roomA, Room roomB)
    {
        if (roomA.isAccessibleFromMainRoom)
            roomB.SetAccessibleFromMainRoom();
        else if (roomB.isAccessibleFromMainRoom)
            roomA.SetAccessibleFromMainRoom();

        roomA.connectedRooms.Add(roomB);
        roomB.connectedRooms.Add(roomA);
    }

    public bool IsConnected(Room otherRoom)
    {
        return connectedRooms.Contains(otherRoom);
    }

    public int CompareTo(Room other)
    {
        return (other.roomSize.CompareTo(roomSize));
    }
}