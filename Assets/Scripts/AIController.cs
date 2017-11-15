using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;

public class AIController : PlayerController
{
    System.Random rando = new System.Random();
    public int Move()
    {
        return rando.Next(0, 7);
    }
    public void GetPath()
    {
        GameController.instance.wait = true;
        Vector3 newPos = new Vector3();
        while (true)
        {
            switch (Move())
            {
                case (0):
                    newPos.x = setUnit.transform.position.x + 1;
                    newPos.z = setUnit.transform.position.z;
                    break;
                case (1):
                    newPos.x = setUnit.transform.position.x + 1;
                    newPos.z = setUnit.transform.position.z + 1;
                    break;
                case (2):
                    newPos.x = setUnit.transform.position.x;
                    newPos.z = setUnit.transform.position.z + 1;
                    break;
                case (3):
                    newPos.x = setUnit.transform.position.x - 1;
                    newPos.z = setUnit.transform.position.z + 1;
                    break;
                case (4):
                    newPos.x = setUnit.transform.position.x - 1;
                    newPos.z = setUnit.transform.position.z;
                    break;
                case (5):
                    newPos.x = setUnit.transform.position.x - 1;
                    newPos.z = setUnit.transform.position.z - 1;
                    break;
                case (6):
                    newPos.x = setUnit.transform.position.x;
                    newPos.z = setUnit.transform.position.z - 1;
                    break;
                case (7):
                    newPos.x = setUnit.transform.position.x + 1;
                    newPos.z = setUnit.transform.position.z - 1;
                    break;
            }
            if (grid.NodeFromWorldPoint(newPos).IsWalkable)
            {
                selectionCube.position = newPos;
                PathRequestManager.RequestPath(setUnit.transform.position, selectionCube.position, setUnit.walkingDist, OnPathFound);
                setUnit.SetTarget(selectionCube);
                StartCoroutine(setUnit.MoveUnit());
                return;
            }
        }
    }
}

