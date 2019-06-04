using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class AIEnemyController : MonoBehaviour
{
    private Transform playerTransform;
    private bool playerDetect;

    private GameObject[] enemyObjects;
    private EnemyBehaviour[] enemies;
    private bool[] enemiesDetectedPlayer;

    private bool enemyAttacking;

    private CavernSceneSoundController CavernSceneSoundController;

    void Awake()
    {
        enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        enemies = new EnemyBehaviour[enemyObjects.Length];

        int aux = Random.Range(0, enemies.Length);

        for(int i = 0; i < enemies.Length; i++)
        {
            enemies[i] = enemyObjects[i].GetComponent<EnemyBehaviour>();
            enemies[i].SetEnemyID(i);

            if(i == aux) enemies[i].GetComponent<EnemyLootController>().SetHasKey(true);
        }

        CavernSceneSoundController = GameObject.FindWithTag("SoundController").GetComponent<CavernSceneSoundController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyAttacking = false;
        playerDetect = false;
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

    public void SetPlayerDetected(int enemyID, bool value)
    {
        if(enemiesDetectedPlayer == null) enemiesDetectedPlayer = new bool[enemies.Length];
        enemiesDetectedPlayer[enemyID] = value;

        print(value + " " + playerDetect);

        if(!value && playerDetect)
        {
            bool aux = false;
            for(int i = 0; i < enemiesDetectedPlayer.Length; i++)
            {
                if(enemiesDetectedPlayer[i]) { 
                    aux = true;
                    break;
                }
            }

            if(!aux) CavernSceneSoundController.PlayCavernTheme();

            playerDetect = aux;
        }
        else if(value && !playerDetect)
        {
            CavernSceneSoundController.PlayActionTheme();

            playerDetect = value;
        }
        
    }
}
