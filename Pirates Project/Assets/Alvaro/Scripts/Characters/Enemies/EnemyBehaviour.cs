using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DefinitiveScript;

enum EnemyState {
    Patrolling,
    Searching,
    Following,
    Staring,
    Attacking,
    Blocking
}

public class EnemyBehaviour : CharacterBehaviour
{
    public Transform PatrolPathObj;
    public float movingSpeed;
    public float runningSpeed;

    public float patrolDetectionRadius;
    public float patrolDetectionAngle;
    public float seekingDetectionRadius;
    public float seekingDetectionAngle;

    public float minDistanceFromPlayer;
    public float maxDistanceFromPlayer;

    public LayerMask visionObstacles;

    public float seekingTime = 3f;

    private bool running;

    private bool patrolling;
    private bool following;
    private bool searching;
    private bool staring;
    
    private bool blocking;
    private bool attacking;
    private bool aroundPlayer;

    private EnemyState enemyState;

    private float detectionRadius;
    private float detectionAngle;

    private NavMeshAgent NavMeshAgent;
    private SphereCollider SphereCollider;
    private Animator Animator;

    private int currentPatrolPoint;
    private Transform[] patrolPoints;

    private Transform playerTransform;

    private AIEnemyController m_AIEnemyController;
    public AIEnemyController AIEnemyController {
        get {
            if(m_AIEnemyController == null) m_AIEnemyController = GameManager.Instance.AIEnemyController;
            return m_AIEnemyController;
        }
    }

    private EnemySableController m_EnemySableController;
    public EnemySableController EnemySableController
    {
        get {
            if(m_EnemySableController == null) m_EnemySableController = GetComponent<EnemySableController>();
            return m_EnemySableController;
        }
    }

    public bool sableEnemy; //true = lleva sable; false = lleva gun

    void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        EnemyPlayerDetection[] enemyPlayerDetectors = GetComponentsInChildren<EnemyPlayerDetection>();
        for(int i = 0; i < enemyPlayerDetectors.Length; i++) 
        {
            enemyPlayerDetectors[i].enemyScript = this;
            
            if(enemyPlayerDetectors[i].GetComponent<SphereCollider>() != null) {
                SphereCollider = enemyPlayerDetectors[i].GetComponent<SphereCollider>();
            }
        }
    }

    void Start()
    {
        StartCoroutine(ObtainPlayerTransform());

        CreatePathPoints();
        SetPatrolling();
    }

    IEnumerator ObtainPlayerTransform()
    {
        while(AIEnemyController.GetPlayerTransform() == null)
        {
            yield return null;
        }
        playerTransform = AIEnemyController.GetPlayerTransform();
    }

    void Update()
    {
        Animator.SetBool("Patrolling", IsPatrolling());
        Animator.SetBool("Following", IsFollowing());
        Animator.SetBool("Searching", IsSearching());
        Animator.SetBool("Staring", IsStaring());
        Animator.SetBool("Blocking", IsBlocking());
        Animator.SetBool("Attacking", IsAttacking());
        Animator.SetBool("AroundPlayer", aroundPlayer);

        ChangeVisionField(IsFollowing() || IsSearching() || (IsPatrolling() && running));
        ImAttacking(IsAttacking());
    }

    void CreatePathPoints()
    {
        patrolPoints = new Transform[PatrolPathObj.childCount];

        for(int i = 0; i < PatrolPathObj.childCount; i++)
        {
            patrolPoints[i] = PatrolPathObj.GetChild(i);
        }
    }

    private void ChangeVisionField(bool param)
    {
        SphereCollider.radius = detectionRadius = param ? seekingDetectionRadius : patrolDetectionRadius;
        detectionAngle = param ? seekingDetectionAngle : patrolDetectionAngle;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return NavMeshAgent;
    }

    public Transform GetPlayerTransform()
    {
        return playerTransform;
    }
    
    public int GetCurrentPatrolPoint()
    {
        return currentPatrolPoint;
    }

    public Vector3 GetNextPatrolPoint(bool firstPoint)
    {
        if(firstPoint)
        {
            currentPatrolPoint = 0;
            float minMagnitude = (patrolPoints[currentPatrolPoint].position - transform.position).magnitude;

            for(int i = 1; i < patrolPoints.Length; i++)
            {
                if((patrolPoints[i].position - transform.position).magnitude < minMagnitude)
                {
                    minMagnitude = (patrolPoints[i].position - transform.position).magnitude;
                    currentPatrolPoint = i;
                }
            }

            return patrolPoints[currentPatrolPoint].position;
        }
        else
        {
            currentPatrolPoint += 1;
            if(currentPatrolPoint == patrolPoints.Length) currentPatrolPoint = 0;

            return patrolPoints[currentPatrolPoint].position;
        }
    }


    public void CharacterDetection(bool collidingSphere, bool collidingEnemy)
    {
        bool result = false;
        if(collidingSphere) //El personaje está dentro de la esfera de colisión
        {
            Vector3 relativePositionToPlayer = playerTransform.position - transform.position; //Posición relativa del jugador con respecto al enemigo
            Vector3 globalForwardFromEnemy = transform.TransformDirection(Vector3.forward); //Vector dirección del frente del enemigo

            float angleBetweenVectors = Vector3.Angle(relativePositionToPlayer, globalForwardFromEnemy);

            if(angleBetweenVectors < detectionAngle) //El personaje está dentro del campo de visión del enemigo
            {                    
                Transform playerCenter = playerTransform.GetComponent<PlayerBehaviour>().characterCenter;
                Vector3 vectorBetweenCharacters = playerCenter.position - characterCenter.position;
                if(!Physics.Raycast(characterCenter.position, vectorBetweenCharacters, vectorBetweenCharacters.magnitude, visionObstacles)) //No existen obstáculos para la visión del enemigo
                {
                    result = true;
                }
            }
        }

        if(collidingEnemy) result = true;

        if(IsFollowing() && !result) SetSearching();
        if(!IsStaring() && !IsAttacking() && !IsBlocking() && result) SetFollowing();
    }

    public void Disarm()
    {
        Animator.SetTrigger("SwordKnockback");
    }

    public void HitOnSword()
    {
        Animator.SetTrigger("HitOnSword");
    }

    public void HitOnBody()
    {
        Animator.SetTrigger("HitOnBody");

        if(IsPatrolling())
        {
            SetSearching();
        }
        else if(IsBlocking() || IsAttacking())
        {
            SetStaring();
        }
    }

    public void ComboAttack()
    {
        EnemySableController.ComboAttack();
    }

    public void Attack()
    {
        Animator.SetTrigger("Attack");
    }

    public void Die()
    {
        Animator.SetTrigger("Die");
    }

    public void Disappear()
    {
        StartCoroutine(FadeOut(1.0f));
    }

    private IEnumerator FadeOut(float time)
    {
        float elapsedTime = 0.0f;
        Vector3 newPosition = transform.position;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            newPosition.y -= Time.deltaTime;
            transform.position = newPosition;

            yield return null;
        }

        Destroy(gameObject); //?
    }
    
    public bool IsRunning()
    {
        return running;
    }

    public bool IsPatrolling()
    {
        return enemyState == EnemyState.Patrolling;
    }

    public bool IsFollowing()
    {
        return enemyState == EnemyState.Following;
    }

    public bool IsSearching()
    {
        return enemyState == EnemyState.Searching;
    }

    public bool IsStaring()
    {
        return enemyState == EnemyState.Staring;
    }

    public bool IsBlocking()
    {
        return enemyState == EnemyState.Blocking;
    }

    public bool IsAttacking()
    {
        return enemyState == EnemyState.Attacking;
    }

    public bool IsMovingAroundPlayer()
    {
        return aroundPlayer;
    }

    public bool IsExpectingAttack()
    {
        return EnemySableController.GetChaining();
    }

    public bool HasHit()
    {
        return EnemySableController.GetHit();
    }

    public bool CanAttack()
    {
        return !AIEnemyController.GetEnemyAttacking();
    }

    public void ImAttacking(bool value)
    {
        AIEnemyController.SetEnemyAttacking(value);
    }

    public void StartRunningPatrol()
    {
        StartCoroutine(RunningPatrol());
    }

    IEnumerator RunningPatrol()
    {
        running = true;

        yield return new WaitForSeconds(seekingTime);

        running = false;
    }

    public void SetPatrolling()
    {
        enemyState = EnemyState.Patrolling;
    }

    public void SetFollowing()
    {
        enemyState = EnemyState.Following;
    }

    public void SetSearching()
    {
        enemyState = EnemyState.Searching;
    }

    public void SetStaring()
    {
        enemyState = EnemyState.Staring;
    }

    public void SetBlocking()
    {
        enemyState = EnemyState.Blocking;
    }

    public void SetAttacking()
    {
        enemyState = EnemyState.Attacking;
    }

    public void SetMovingAroundPlayer(bool value)
    {
        aroundPlayer = value;
    }
}
