using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Policy;
using UnityEngine.UI;

public class LevelMap
{
    private List<Cell> _map;

    public List<Cell> Map
    {
        get { return _map; }
    }

    public int Height { get; set; }
    public int Width { get; set; }
    public int Depth { get; set; }

    public LevelMap(int depth, int width, int height)
    {
        Height = height * 2;
        Width = width;
        Depth = depth;

        _map = new List<Cell>(Height * Width * Depth);
    }

    public LevelMap(int[,] rawMap, int height)
    {
        Height = height;
        Width = rawMap.GetLength(0);
        Depth = rawMap.GetLength(1);

        _map = new List<Cell>(Height * Width * Depth);

        for (int x = 0; x < rawMap.GetLength(0); x++)
            for (int z = 0; z < rawMap.GetLength(1); z++)
                Map.Add(new Cell(x, Height / 2, z, rawMap[x, z]));
    }

    public Cell this[int x, int y, int z]
    {
        get
        { return _map.FirstOrDefault( c => c.Position.tileX == x && c.Position.tileY == y && c.Position.tileZ == z ); }
    }
}

public class Coord : IEquatable<Coord>
{
    public override int GetHashCode()
    {
        unchecked
        {
            var hashCode = tileX;
            hashCode = (hashCode * 397) ^ tileY;
            hashCode = (hashCode * 397) ^ tileZ;
            return hashCode;
        }
    }

    public int tileX;
    public int tileY;
    public int tileZ;

    public Coord(int x = 0, int y = 0, int z = 0)
    {
        tileX = x;
        tileY = y;
        tileZ = z;
    }

    public override bool Equals(object other)
    {
        return (this.Equals(other as Coord));
    }

    public bool Equals(Coord other)
    {
        return (tileX == other.tileX &&
                 tileY == other.tileY &&
                 tileZ == other.tileZ);
    }

    public static bool operator ==(Coord left, Coord right)
    {
        return ((left != null && right != null) && (left.tileX == right.tileX &&
                                                        left.tileY == right.tileY &&
                                                        left.tileZ == right.tileZ));
    }

    public static bool operator !=(Coord left, Coord right)
    {
        return !(left == right);
    }
}

public class Cell : IEquatable<Cell>
{
    public Coord Position { get; set; }
    public int Type { get; set; }

    public Cell(Coord position, int type = 0)
    {
        Position = position;
        Type = type;
    }

    public Cell(int x = 0, int y = 0, int z = 0, int type = 0)
    {
        Position = new Coord(x, y, z);
        Type = type;
    }

    public Cell(Cell other)
    {
        Position = other.Position;
        Type = other.Type;
    }

    public void GrowTo(Cell other)
    {
        other.Type = this.Type;
    }

    public override bool Equals(object other)
    {
        return (this.Equals(other as Cell));
    }

    public override int GetHashCode()
    {
        return (Position.GetHashCode() * 397) ^ Type;
    }

    public bool Equals(Cell other)
    {
        return (Position.tileX == other.Position.tileX &&
                 Position.tileY == other.Position.tileY &&
                 Position.tileZ == other.Position.tileZ);
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

    public Room() { }

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