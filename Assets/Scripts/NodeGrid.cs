using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeGrid : MonoBehaviour
{
    public bool displayGridGizmos;
    public LayerMask unwalkableMask;
    public LayerMask halfCoverMask;
    public LayerMask fullCoverMask;
    public LayerMask allyMask;
    public LayerMask enemyMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    public Vector3 worldBottomLeft;

    public Transform gridPrefabPassable;

    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;
    List<Unit> units;
    MouseController mouseController;

    void Awake()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        units = new List<Unit>();

        mouseController = GetComponent<MouseController>();

        CreateGrid();
    }

    public int MaxSize
    {
        get
        {
            return gridSizeX * gridSizeY;
        }
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);
                if(grid[x, y].IsWalkable)
                {
                    Instantiate(gridPrefabPassable, new Vector3(x - 5, 0, y - 4.5f), Quaternion.Euler(0, 0, 0));
                }
                if (Physics.CheckSphere(worldPoint, nodeRadius, halfCoverMask))
                {
                    grid[x, y].coverValue = 1;
                }
                if (Physics.CheckSphere(worldPoint, nodeRadius, fullCoverMask))
                {
                    grid[x, y].coverValue = 2;
                }
            }
        }
    }

    public void CheckPassability(bool ally)
    {
        var passabilities = GameObject.FindGameObjectsWithTag("pass");
        foreach (var item in passabilities)
        {
            Destroy(item);
        }
        if (ally)
        {
            UpdateGrid(enemyMask);
        }
        else
        {
            UpdateGrid(allyMask);
        }
        foreach (Node node in grid)
        {
            if (node.IsWalkable)
            {
                Instantiate(gridPrefabPassable, new Vector3(node.gridX - 5, 0, node.gridY - 4.5f), Quaternion.Euler(0, 0, 0));
            }
        }
    }

    public void UpdateGrid(LayerMask notMask)
    {
        foreach (Node node in grid)
        {
            if (!Physics.CheckSphere(node.WorldPosition, nodeRadius, notMask))
            {
                if (!Physics.CheckSphere(node.WorldPosition, nodeRadius, unwalkableMask))
                {
                    node.IsWalkable = true;
                }
            }
            else
            {
                node.IsWalkable = false;
            }
        }
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }


    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {
        Vector3 localPosition = worldPosition - worldBottomLeft;

        float percentX = (localPosition.x) / gridWorldSize.x;
        float percentY = (localPosition.z) / gridWorldSize.y;
        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }


    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
        if (grid != null && displayGridGizmos)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.IsWalkable) ? Color.green : Color.red;
                Gizmos.DrawWireCube(n.WorldPosition, new Vector3(1, 0, 1) * nodeDiameter);
            }
        }
    }
}
