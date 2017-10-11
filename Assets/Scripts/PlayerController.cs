using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public abstract class PlayerController : MonoBehaviour
{
    public Transform selectionCube;
    public Transform plane;
    public LineRenderer lineRenderer;

    protected Unit setUnit;
    protected Vector3[] path;
    protected Grid grid;


    void Awake()
    {
        grid = GetComponent<Grid>();
    }

    public void SetUnit(Unit unit)
    {
        setUnit = unit;
    }
    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            if (this != null)
            {
                lineRenderer.positionCount = path.Length;
                lineRenderer.SetPositions(path);
            }
        }
    }
}
    
