using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class AIEnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private bool playerDetect;

    private EnemyBehaviour[] enemies;

    private bool enemyAttacking;

    // Start is called before the first frame update
    void Start()
    {
        enemyAttacking = false;
<<<<<<< HEAD

        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new EnemyBehaviour[enemyObjects.Length];

        int enemyWithKey = 0; Random.Range(0, enemyObjects.Length);

        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = enemyObjects[i].GetComponent<EnemyBehaviour>();

            if(i == enemyWithKey) enemies[i].GetComponent<EnemyLootController>().SetHasKey(true);
        }
=======
>>>>>>> parent of 8a6f75546... con animacion del cofre
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

    public void PlayerDead()
    {
        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i].PlayerDead();
            enemies[i].SetPatrolling();
        }
    }
}
