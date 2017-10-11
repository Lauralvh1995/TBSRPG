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
                setUnit.transform.LookAt(selectionCube);
            }
            setUnit.SetTarget(selectionCube);
            previousTileCoord = currentTileCoord;
        }
    }
    public bool MouseDown()
    {
        return setUnit.MouseDown();
    }
}
