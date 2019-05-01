using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyVisible : MonoBehaviour
{
    public bool visible;
    private void OnBecameInvisible()
    {
        visible = false;
    }
    private void OnBecameVisible()
    {
        visible = true;
    }
}
