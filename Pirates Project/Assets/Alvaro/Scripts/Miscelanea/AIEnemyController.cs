using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIEnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private bool playerDetect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }

    public void SetPlayerTransform(Transform param)
    {
        playerTransform = param;
    }
}
