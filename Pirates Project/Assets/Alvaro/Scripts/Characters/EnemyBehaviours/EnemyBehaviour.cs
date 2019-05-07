using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DefinitiveScript;

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

    void Awake()
    {
        NavMeshAgent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        EnemyCharacterDetection[] enemyCharacterDetectors = GetComponentsInChildren<EnemyCharacterDetection>();
        for(int i = 0; i < enemyCharacterDetectors.Length; i++) 
        {
            enemyCharacterDetectors[i].enemyScript = this;
            
            if(enemyCharacterDetectors[i].GetComponent<SphereCollider>() != null) {
                SphereCollider = enemyCharacterDetectors[i].GetComponent<SphereCollider>();
            }
        }
    }

    void Start()
    {
        StartCoroutine(ObtainPlayerTransform());

        CreatePathPoints();
        patrolling = true;
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
        Animator.SetBool("Patrolling", patrolling);
        Animator.SetBool("Following", following);
        Animator.SetBool("Searching", searching);
        Animator.SetBool("Staring", staring);
        Animator.SetBool("Blocking", blocking);
        Animator.SetBool("Attacking", attacking);
        Animator.SetBool("AroundPlayer", aroundPlayer);

        ChangeVisionField(following || searching || (patrolling && running));
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

        if(patrolling) patrolling = !result;
        if(following) searching = !result;
        if(!staring && !attacking && !blocking) following = result;
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

        if(patrolling || searching)
        {
            patrolling = searching = false;
            following = true;
        }
        else if(blocking || attacking)
        {
            blocking = false;
            attacking = false;
            staring = true;
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

    public void StartAttackEvent(int attackId)
    {
        EnemySableController.StartAttack(attackId, Animator.GetNextAnimatorClipInfo(0)[0].clip.length);
    }

    public void EnableSwordColliderEvent()
    {
        EnemySableController.EnableSwordCollider();
    }

    public void DisableSwordColliderEvent()
    {
        EnemySableController.DisableSwordCollider();
    }

    public void FinishAttackEvent(int attackId)
    {
        EnemySableController.FinishAttack(attackId);
    }

    
    public bool IsRunning()
    {
        return running;
    }

    public bool IsPatrolling()
    {
        return patrolling;
    }

    public bool IsFollowing()
    {
        return following;
    }

    public bool IsSearching()
    {
        return searching;
    }

    public bool IsStaring()
    {
        return staring;
    }

    public bool IsBlocking()
    {
        return blocking;
    }

    public bool IsAttacking()
    {
        return attacking;
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

    public void SetPatrolling(bool value)
    {
        patrolling = value;
    }

    public void SetFollowing(bool value)
    {
        following = value;
    }

    public void SetSearching(bool value)
    {
        searching = value;
    }

    public void SetStaring(bool value)
    {
        staring = value;
    }

    public void SetBlocking(bool value)
    {
        blocking = value;
    }

    public void SetAttacking(bool value)
    {
        attacking = value;
        ImAttacking(value);
    }

    public void SetMovingAroundPlayer(bool value)
    {
        aroundPlayer = value;
    }
}
