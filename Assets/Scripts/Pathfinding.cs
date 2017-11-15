using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public int maxLength;

    NodeGrid grid;
    PathRequestManager requestManager;

    void Awake()
    {
        grid = GetComponent<NodeGrid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void SetMaxLenght(int length)
    {
        maxLength = length;
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 endPosition)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.NodeFromWorldPoint(startPosition);
        Node targetNode = grid.NodeFromWorldPoint(endPosition);

        if (startNode.IsWalkable && targetNode.IsWalkable)
        {

            Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
            HashSet<Node> closedSet = new HashSet<Node>();
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                Node node = openSet.RemoveFirst();
                closedSet.Add(node);

                if (node == targetNode)
                {
                    stopwatch.Stop();
                    pathSuccess = true;
                    //UnityEngine.Debug.Log(endPosition.x.ToString() + ',' + endPosition.z.ToString());
                    UnityEngine.Debug.Log(stopwatch.ElapsedMilliseconds.ToString() + " ms");
                    break;
                }

                foreach (Node neighbour in grid.GetNeighbours(node))
                {
                    if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }

                    int newCostToNeighbour = node.gCost + GetDistance(node, neighbour);
                    if (newCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                    {
                        neighbour.gCost = newCostToNeighbour;
                        neighbour.hCost = GetDistance(neighbour, targetNode);
                        neighbour.parent = node;

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                        else
                        {
                            openSet.UpdateItem(neighbour);
                        }
                    }
                }
            }
        }
        yield return null;
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
        }
        else
        {
            waypoints = RetracePath(startNode, startNode);
            pathSuccess = true;
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    int GetDistance(Node a, Node b)
    {
        int dstX = Mathf.Abs(a.gridX - b.gridX);
        int dstY = Mathf.Abs(a.gridY - b.gridY);

        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        else
        {
            return 14 * dstX + 10 * (dstY - dstX);
        }

    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        if (currentNode == startNode)
        {
            path.Add(currentNode);
        }
        path.Add(startNode);
        Vector3[] waypoints;
        if (path.Count > maxLength+2)
        {
            waypoints = new Vector3[1] { startNode.WorldPosition };
        }
        else
        {
            waypoints = SimplifyPath(path);
        }
        Array.Reverse(waypoints);
        return waypoints;
    }
    private Vector3[] SimplifyPath(List<Node> path)
    {
        if (path.Count < 1)
        {
            return new Vector3[0];
        }

        List<Vector3> pathWayPoints = new List<Vector3>
        {

            // Don't forget to add last point 
            path[0].WorldPosition
        };
        for (int i = 1; i < path.Count - 1; i++)
        {
            Vector2 furuteDirection = new Vector2(path[i + 1].gridX - path[i].gridX, path[i + 1].gridY - path[i].gridY);
            Vector2 directionPrevious = new Vector2(path[i].gridX - path[i - 1].gridX, path[i].gridY - path[i - 1].gridY);

            if (furuteDirection != directionPrevious)
            {
                // We add worldPosition, but not the actual node
                pathWayPoints.Add(path[i].WorldPosition);
            }
        }

        pathWayPoints.Add(path[path.Count - 1].WorldPosition);

        return pathWayPoints.ToArray();
    }

    public IEnumerator StartFindPath(Vector3 startPos, Vector3 targetPos)
    {
        yield return StartCoroutine(FindPath(startPos, targetPos));
    }
}
