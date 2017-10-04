using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool IsWalkable;
    public Vector3 WorldPosition;

    public Node parent;

    public int gCost;
    public int hCost;

    public int gridX;
    public int gridY;

    int heapIndex;

    public Node(bool isWalkable, Vector3 worldPosition, int _gridX, int _gridY)
    {
        IsWalkable = isWalkable;
        WorldPosition = worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }
    public int fCost
    {
        get { return gCost + hCost; }
    }

    public int HeapIndex
    {
        get
        {
            return heapIndex;
        }

        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
        {
            hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
