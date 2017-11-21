using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : PlayerController {

    public Transform cameraTransform;
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

            if (previousTileCoord != currentTileCoord && setUnit != null)
            {
                selectionCube.position = currentTileCoord;
                PathRequestManager.RequestPath(setUnit.transform.position, selectionCube.position, setUnit.walkingDist, OnPathFound);
                setUnit.transform.LookAt(selectionCube);
                setUnit.SetTarget(selectionCube);
            }
            previousTileCoord = currentTileCoord;
        }
        MoveCamera();
    }
    public void MouseDown()
    {
        GameController.instance.wait = true;
        StartCoroutine(setUnit.MoveUnit());
    }

    public void Attack()
    {
        setUnit.Attack(GetTarget());
    }

    void MoveCamera()
    {
        float mousePosX = Input.mousePosition.x;
        float mousePosY = Input.mousePosition.y;
        int scrollDistance = 5;
        float scrollSpeed = 20;
        if (mousePosX < scrollDistance)
        {
            cameraTransform.Translate(new Vector3(-1, 0, 1) * scrollSpeed * Time.deltaTime);
            
        }

        if (mousePosX >= Screen.width - scrollDistance)
        {
            cameraTransform.Translate(new Vector3(-1, 0, 1) * -scrollSpeed * Time.deltaTime);
        }

        if (mousePosY < scrollDistance)
        {
            cameraTransform.Translate(new Vector3(1, 0, 1) * -scrollSpeed * Time.deltaTime);
        }

        if (mousePosY >= Screen.height - scrollDistance)
        {
            cameraTransform.Translate(new Vector3(1, 0, 1) * scrollSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            cameraTransform.Translate(new Vector3(1, -0.75f, 1) * scrollSpeed * Time.deltaTime);
        }

        if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            cameraTransform.Translate(new Vector3(-1, 0.75f, -1) * scrollSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            cameraTransform.position = Vector3.zero;
        }
    }
}

