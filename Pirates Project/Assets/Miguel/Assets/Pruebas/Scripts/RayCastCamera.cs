using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastCamera : MonoBehaviour
{
    public bool EnemyBehindObstacle()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        int layerMask = 1<<8;
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, fwd, 20, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
