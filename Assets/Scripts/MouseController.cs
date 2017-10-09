using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : PlayerController {


    Vector3 currentTileCoord;
    Vector3 previousTileCoord;

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hitInfo;

        if (plane.GetComponent<Collider>().Raycast(ray, out hitInfo, Mathf.Infinity))
        {
            int x = Mathf.FloorToInt(hitInfo.point.x / grid.nodeRadius * 0.5f);
            int y = Mathf.FloorToInt(hitInfo.point.z / grid.nodeRadius * 0.5f);

            currentTileCoord.x = x + 0.5f;
            currentTileCoord.z = y + 0.5f;

            if (previousTileCoord != currentTileCoord)
            {
                selectionCube.position = currentTileCoord;
                PathRequestManager.RequestPath(setUnit.transform.position, selectionCube.position, OnPathFound);
            }
                //Debug.Log(currentTileCoord.x.ToString() + ',' + currentTileCoord.z.ToString());
            setUnit.SetTarget(selectionCube);
            previousTileCoord = currentTileCoord;
        }
    }
    void OnPathFound(Vector3[] newPath, bool pathSuccessful)
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

    public bool MouseDown()
    {
        return setUnit.MouseDown();
    }
}
