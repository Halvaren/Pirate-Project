using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class AIEnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private bool playerDetect;

    private bool enemyAttacking;

    // Start is called before the first frame update
    void Start()
    {
        enemyAttacking = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Transform GetPlayerTransform()
    {
        return GameManager.Instance.LocalPlayer.transform;
    }

    public bool GetEnemyAttacking()
    {
        return enemyAttacking;
    }

    public void SetEnemyAttacking(bool value)
    {
        enemyAttacking = value;
    }
}
