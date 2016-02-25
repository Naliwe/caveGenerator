using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

public class MeshGenerator : MonoBehaviour
{
    public MeshFilter walls;
    public SquareGrid squareGrid;
    public MeshFilter cave;

    public bool is2d;

    private List<Vector3> vertices;
    private List<int> triangles;

    private Dictionary<int, List<Triangle>> triangleDictionary = new Dictionary<int, List<Triangle>>();
    List<List<int>> outlines = new List<List<int>>();
    HashSet<int> checkedVertices = new HashSet<int>();

    public void GenerateMesh(int[,] map, float squareSize)
    {
        triangleDictionary.Clear();
        outlines.Clear();
        checkedVertices.Clear();

        squareGrid = new SquareGrid(map, squareSize);

        vertices = new List<Vector3>();
        triangles = new List<int>();

        for (int x = 0; x < squareGrid.squares.GetLength(0); x++)
            for (int y = 0; y < squareGrid.squares.GetLength(1); y++)
                TriangulateSquare(squareGrid.squares[x, y]);

        Mesh mesh = new Mesh();
        cave.mesh = mesh;

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        int tileAmount = 10;
        Vector2[] uvs = new Vector2[vertices.Count];

        for ( int i = 0; i < vertices.Count; i++ )
        {
            float percentX = Mathf.InverseLerp( -map.GetLength( 0 ) / 2 * squareSize,
                                                map.GetLength( 0 ) / 2 * squareSize, vertices[i].x ) * tileAmount;
            float percentY = Mathf.InverseLerp(-map.GetLength(0) / 2 * squareSize,
                                                map.GetLength(0) / 2 * squareSize, vertices[i].z) * tileAmount;

            uvs[i] = new Vector2(percentX, percentY);
        }

        mesh.uv = uvs;

        if ( is2d )
            Generate2DColliders();
        else
            CreateWallMesh();
    }

    private void Generate2DColliders()
    {
        EdgeCollider2D[] currentColliders = gameObject.GetComponents<EdgeCollider2D>();

        for ( int i = 0; i < currentColliders.Length; i++ )
        {
            Destroy( currentColliders[i] );
        }

        CalculateMeshOutlines();

        foreach ( List<int> outline in outlines )
        {
            EdgeCollider2D edgeCollider2D = gameObject.AddComponent < EdgeCollider2D>();
            Vector2[] edgePoints = new Vector2[outline.Count];

            for ( int i = 0; i < outline.Count; i++ )
                edgePoints[i] = new Vector2(vertices[outline[i]].x, vertices[outline[i]].z);
            edgeCollider2D.points = edgePoints;
        }
    }

    private void CreateWallMesh()
    {

        CalculateMeshOutlines();

        List<Vector3> wallVertices = new List<Vector3>();
        List<int> wallTriangles = new List<int>();
        Mesh wallMesh = new Mesh();
        float wallHeight = 5;

        foreach (List<int> outline in outlines)
        {
            for (int i = 0; i < outline.Count - 1; i++)
            {
                int startIndex = wallVertices.Count;
                wallVertices.Add(vertices[outline[i]]); // Left
                wallVertices.Add(vertices[outline[i + 1]]); // Right
                wallVertices.Add(vertices[outline[i]] - Vector3.up * wallHeight); // Bottom left
                wallVertices.Add(vertices[outline[i + 1]] - Vector3.up * wallHeight); // Bottom right

                wallTriangles.Add(startIndex + 0);
                wallTriangles.Add(startIndex + 2);
                wallTriangles.Add(startIndex + 3);

                wallTriangles.Add(startIndex + 3);
                wallTriangles.Add(startIndex + 1);
                wallTriangles.Add(startIndex + 0);
            }
        }

        wallMesh.vertices = wallVertices.ToArray();
        wallMesh.triangles = wallTriangles.ToArray();
        walls.mesh = wallMesh;

        MeshCollider wallCollider = walls.gameObject.AddComponent<MeshCollider>();
        wallCollider.sharedMesh = wallMesh;
    }

    struct Triangle
    {
        public int vertexIndexA;
        public int vertexIndexB;
        public int vertexIndexC;
        private int[] vertices;

        public Triangle(int a, int b, int c)
        {
            vertexIndexA = a;
            vertexIndexB = b;
            vertexIndexC = c;

            vertices = new int[3]
                       {
                           a, b, c
                       };
        }

        public int this[int i]
        {
            get { return vertices[i]; }
        }

        public bool Contains(int vertexIndex)
        {
            return (vertexIndex == vertexIndexA || vertexIndex == vertexIndexB || vertexIndex == vertexIndexC);
        }
    }

    void TriangulateSquare(Square square)
    {
        switch (square.configuration)
        {
            case 0:
                break;
            case 1:
                MeshFromPoints(square.centerLeft, square.centerBottom, square.bottomLeft);
                break;
            case 2:
                MeshFromPoints(square.bottomRight, square.centerBottom, square.centerRight);
                break;
            case 4:
                MeshFromPoints(square.topRight, square.centerRight, square.centerTop);
                break;
            case 8:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerLeft);
                break;
            case 3:
                MeshFromPoints(square.centerRight, square.bottomRight, square.bottomLeft, square.centerLeft);
                break;
            case 6:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.centerBottom);
                break;
            case 9:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerBottom, square.bottomLeft);
                break;
            case 12:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerLeft);
                break;
            case 5:
                MeshFromPoints(square.centerTop, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft, square.centerLeft);
                break;
            case 10:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;
            case 7:
                MeshFromPoints(square.centerTop, square.topRight, square.bottomRight, square.bottomLeft,
                                square.centerLeft);
                break;
            case 11:
                MeshFromPoints(square.topLeft, square.centerTop, square.centerRight, square.bottomRight, square.bottomLeft);
                break;
            case 13:
                MeshFromPoints(square.topLeft, square.topRight, square.centerRight, square.centerBottom, square.bottomLeft);
                break;
            case 14:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.centerBottom, square.centerLeft);
                break;
            case 15:
                MeshFromPoints(square.topLeft, square.topRight, square.bottomRight, square.bottomLeft);
                checkedVertices.Add(square.topLeft.vertexIndex);
                checkedVertices.Add(square.topRight.vertexIndex);
                checkedVertices.Add(square.bottomRight.vertexIndex);
                checkedVertices.Add(square.bottomLeft.vertexIndex);
                break;
        }
    }

    void MeshFromPoints(params Node[] points)
    {
        AssignVertices(points);

        if (points.Length >= 3)
            CreateTriangle(points[0], points[1], points[2]);
        if (points.Length >= 4)
            CreateTriangle(points[0], points[2], points[3]);
        if (points.Length >= 5)
            CreateTriangle(points[0], points[3], points[4]);
        if (points.Length >= 6)
            CreateTriangle(points[0], points[4], points[5]);
    }

    private void AssignVertices(Node[] points)
    {
        for (int i = 0; i < points.Length; i++)
        {
            if (points[i].vertexIndex == -1)
            {
                points[i].vertexIndex = vertices.Count;
                vertices.Add(points[i].position);
            }
        }
    }

    void CreateTriangle(Node a, Node b, Node c)
    {
        triangles.Add(a.vertexIndex);
        triangles.Add(b.vertexIndex);
        triangles.Add(c.vertexIndex);

        Triangle triangle = new Triangle(a.vertexIndex, b.vertexIndex, c.vertexIndex);
        AddTriangleToDictionary(triangle.vertexIndexA, triangle);
        AddTriangleToDictionary(triangle.vertexIndexB, triangle);
        AddTriangleToDictionary(triangle.vertexIndexC, triangle);
    }

    void AddTriangleToDictionary(int vertexIndexKey, Triangle t)
    {
        if (triangleDictionary.ContainsKey(vertexIndexKey))
            triangleDictionary[vertexIndexKey].Add(t);
        else
        {
            triangleDictionary.Add(vertexIndexKey, new List<Triangle> { t });
        }
    }

    void CalculateMeshOutlines()
    {
        for (int vIndex = 0; vIndex < vertices.Count; vIndex++)
        {
            if (!checkedVertices.Contains(vIndex))
            {
                int newOutlineVertex = GetConnectedOutlineVertex(vIndex);
                if (newOutlineVertex != -1)
                {
                    checkedVertices.Add(vIndex);

                    List<int> newOutline = new List<int>();
                    newOutline.Add(vIndex);
                    outlines.Add(newOutline);
                    FollowOutline(newOutlineVertex, outlines.Count - 1);
                    outlines[outlines.Count - 1].Add(vIndex);
                }
            }
        }
    }

    private void FollowOutline(int newOutlineVertex, int i)
    {
        outlines[i].Add(newOutlineVertex);
        checkedVertices.Add(newOutlineVertex);

        int nextVertexIndex = GetConnectedOutlineVertex(newOutlineVertex);

        if (nextVertexIndex != -1)
            FollowOutline(nextVertexIndex, i);
    }

    int GetConnectedOutlineVertex(int vertexIndex)
    {
        List<Triangle> trianglesContainingVertex = triangleDictionary[vertexIndex];

        for (int i = 0; i < trianglesContainingVertex.Count; i++)
        {
            Triangle t = trianglesContainingVertex[i];

            for (int j = 0; j < 3; j++)
            {
                int vertexB = t[j];

                if (vertexB != vertexIndex && !checkedVertices.Contains(vertexB) && IsOutlineEdge(vertexIndex, vertexB))
                    return (vertexB);
            }
        }

        return (-1);
    }

    bool IsOutlineEdge(int vA, int vB)
    {
        List<Triangle> trianglesContainingVA = triangleDictionary[vA];
        int sharedTriangles = 0;

        for (int i = 0; i < trianglesContainingVA.Count; i++)
        {
            if (trianglesContainingVA[i].Contains(vB))
            {
                sharedTriangles++;
                if (sharedTriangles > 1)
                    break;
            }
        }

        return (sharedTriangles == 1);
    }

    public class SquareGrid
    {
        public Square[,] squares;

        public SquareGrid(int[,] map, float squareSize)
        {
            int nodeCountX = map.GetLength(0);
            int nodeCountY = map.GetLength(1);
            float mapWidth = nodeCountX * squareSize;
            float mapHeight = nodeCountY * squareSize;

            ControlNode[,] controlNodes = new ControlNode[nodeCountX, nodeCountY];

            for (int x = 0; x < nodeCountX; x++)
                for (int y = 0; y < nodeCountY; y++)
                {
                    Vector3 pos = new Vector3(-mapWidth / 2 + x * squareSize + squareSize / 2, 0,
                        -mapHeight / 2 + y * squareSize + squareSize / 2);
                    controlNodes[x, y] = new ControlNode(pos, map[x, y] == 1, squareSize);
                }
            squares = new Square[nodeCountX - 1, nodeCountY - 1];

            for (int x = 0; x < nodeCountX - 1; x++)
                for (int y = 0; y < nodeCountY - 1; y++)
                    squares[x, y] = new Square(controlNodes[x, y + 1], controlNodes[x + 1, y + 1], controlNodes[x + 1, y], controlNodes[x, y]);
        }
    }

    public class Square
    {
        public ControlNode topLeft, topRight, bottomRight, bottomLeft;
        public Node centerTop, centerRight, centerBottom, centerLeft;
        public int configuration = 0;

        public Square(ControlNode topleft, ControlNode topright, ControlNode bottomright, ControlNode bottomleft)
        {
            topLeft = topleft;
            topRight = topright;
            bottomLeft = bottomleft;
            bottomRight = bottomright;

            centerTop = topLeft.right;
            centerRight = bottomRight.above;
            centerBottom = bottomLeft.right;
            centerLeft = bottomLeft.above;

            if (topLeft.active)
                configuration += 8;
            if (topRight.active)
                configuration += 4;
            if (bottomRight.active)
                configuration += 2;
            if (bottomLeft.active)
                configuration += 1;
        }
    }

    public class Node
    {
        public Vector3 position;
        public int vertexIndex = -1;

        public Node(Vector3 pos)
        {
            position = pos;
        }
    }

    public class ControlNode : Node
    {
        public bool active;
        public Node above, right;

        public ControlNode(Vector3 pos, bool acti, float squareSize) : base(pos)
        {
            active = acti;
            above = new Node(position + Vector3.forward * squareSize / 2f);
            right = new Node(position + Vector3.right * squareSize / 2f);
        }
    }
}
