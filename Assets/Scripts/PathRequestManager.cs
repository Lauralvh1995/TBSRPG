﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PathRequestManager : MonoBehaviour {

    Queue<PathRequest> PathRequestQueue = new Queue<PathRequest>();
    PathRequest currentPathRequest;

    static PathRequestManager instance;
    Pathfinding pathfinding;

    bool isProcessingPath;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    public static bool RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
        instance.PathRequestQueue.Enqueue(newRequest);
        if(instance.TryProcessNext())
        {
            return true;
        }
        return false;
    }

    bool TryProcessNext()
    {
        if (!isProcessingPath && PathRequestQueue.Count > 0)
        {
            currentPathRequest = PathRequestQueue.Dequeue();
            isProcessingPath = true;
            pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd);
            return true;
        }
        return false;
    }

    public bool FinishedProcessingPath(Vector3[] path, bool success)
    {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            if (TryProcessNext())
            {
                return true;
            }
        return false;
    }

    struct PathRequest
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }

    }
}
