﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Unit : MonoBehaviour, IComparable<Unit> {

    
    public float speed = 20;
    public float rotationSpeed = 50;
    public int hp;
    public int maxHP;
    public int initiative;
    public int maxAP;
    public int AP;
    public int walkingDist;
    public bool ally;

    public Weapon equipped;

    private Vector3[] path;
    private int targetIndex;
    private Vector3 currentWaypoint;
    private Stopwatch timer = new Stopwatch();
    private Transform target;

    private System.Random rando = new System.Random();

    private void Update()
    {
        if(hp <= 0)
        {
            Dies();
        }
    }

    public IEnumerator MoveUnit()
    {
        yield return StartCoroutine(PathRequestManager.RequestPath(transform.position, target.position, walkingDist, OnPathFound));
    }

    void Dies()
    {
        GameController.instance.PurgeList();
        Destroy(gameObject);
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
        if (path.Length != 0)
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
                        if (transform.position == currentWaypoint && currentWaypoint != path[0])
                        {
                            GameController.instance.Done(true);
                            GameController.instance.wait = false;
                        }
                        else
                        {
                            GameController.instance.Done(false);
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
        else
        {
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

    public bool Attack(Unit unit)
    {
        int accuracy = equipped.GetAccuracy();
        if (unit != null)
        {
            if (IsTargetWithinRange(unit))
            {
                //raycast, if through cover, accuracy penalty. Layer 9 = half cover; Layer 10 = full cover;
                Ray ray = new Ray(transform.position, unit.transform.position - transform.position);
                RaycastHit[] hitInfo = Physics.RaycastAll(ray, Vector3.Distance(transform.position, unit.transform.position), 11);
                foreach (RaycastHit hit in hitInfo)
                {
                    if (hit.transform.gameObject.layer == 9)
                    {
                        accuracy -= 5;
                    }
                    if (hit.transform.gameObject.layer == 10)
                    {
                        accuracy -= 10;
                    }
                }
                int crit = rando.Next(0, 100);
                if (crit < accuracy)
                {
                    int damage = equipped.GetDamage(); // +/- modifiers
                    if (crit < equipped.GetAccuracy() / 10)
                    {
                        //if crit, deal double damage;
                        damage += damage;
                    }
                    unit.hp -= damage;
                }
                else
                {
                    unit.hp -= 0; //miss
                }
            }
            else
            {
                //no attack at all
            }
            UnityEngine.Debug.Log(unit.hp);
            return true;
        }
        else
        {
            return false;
        }
    }

    bool IsTargetWithinRange(Unit unit)
    {
        if(Mathf.Abs(unit.transform.position.x - transform.position.x) <= equipped.GetRange())
        {
            if (Mathf.Abs(unit.transform.position.z - transform.position.z) <= equipped.GetRange())
            {
                return true;
            }
        }
        return false;
    }
}
