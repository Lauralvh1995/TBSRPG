using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IComparable<Unit> {

    
    public float speed = 20;
    public float rotationSpeed = 50;
    public int initiative;
    public int maxAP;
    public int AP;
    public int walkingDist;

    public Weapon equipped;

    Vector3[] path;
    int targetIndex;
    public Transform target;
    Vector3 currentWaypoint;
    Stopwatch timer = new Stopwatch();


    public IEnumerator MoveUnit()
    {
        yield return StartCoroutine(PathRequestManager.RequestPath(transform.position, target.position, OnPathFound));
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            targetIndex = 0;
            if (this != null)
            {
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }
    }

    IEnumerator FollowPath()
    {
        timer.Start();
        currentWaypoint = path[0];
        targetIndex = 0;
        while (true)
        {
            if (transform.position == currentWaypoint)
            {
                targetIndex++;
                if (targetIndex >= path.Length)
                {
                    timer.Stop();
                    UnityEngine.Debug.Log(timer.Elapsed);
                    if (transform.position == currentWaypoint)
                    {
                        GameController.instance.Done(true);
                        GameController.instance.wait = false;
                    }
                    targetIndex = 0;
                    path = new Vector3[0];
                    yield break;                    
                }
                currentWaypoint = path[targetIndex];
            }
            //Rotate Towards Next Waypoint
            Vector3 targetDir = currentWaypoint - transform.position;
            float step = rotationSpeed * Time.deltaTime;
            Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0F);
            transform.rotation = Quaternion.LookRotation(newDir);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (path != null)
        {
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.color = Color.cyan;

                if (i == targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }
                else
                {
                    Gizmos.DrawLine(path[i - 1], path[i]);
                }
            }
        }
    }

    public int CompareTo(Unit other)
    {
        if(initiative > other.initiative)
        {
            return 1;
        }
        else if(initiative == other.initiative)
        {
            return 0;
        }
        else
        {
            return -1;
        }
    }

    public void SetTarget(Transform transform)
    {
        target = transform;
    }

    public void Attack(Unit unit)
    {
        throw new NotImplementedException();
    }
}
