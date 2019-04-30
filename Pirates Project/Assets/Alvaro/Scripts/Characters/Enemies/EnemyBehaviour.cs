using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace DefinitiveScript
{
    public class EnemyBehaviour : CharacterBehaviour
    {
        private EnemySableController SableController;
        private NavMeshAgent NavMeshAgent;
        private CharacterAnimationController CharacterAnimationController;
        private SphereCollider SphereCollider;

        private Transform[] patrolPoints;
        private float detectionRadius;
        private float detectionAngle;

        public LayerMask visionObstacles;

        private bool inPath;
        private bool seeking;
        private bool following;
        private bool characterNotInRange;
        private bool waitInPoint;
        private bool nextPointPath;
        private bool firstPoint;
        private bool characterDetect;
        private bool toLastPlayerPosition;

        private bool prepareToAttack;

        private bool running;

        private bool stopAI;

        private float waitingTime;

        private float waitingTimer;
        private float untilSeekingTimer;
        private float seekingTimer;

        private int destinationPointIndex;

        private Coroutine enemyCoroutine;
        
        public Transform PatrolPathObj; 
        public float movingSpeed;
        public float runningSpeed;
        public float patrolMinWaitingTime;
        public float patrolMaxWaitingTime;

        public float rotationSpeed = 25f;
        public float blockingRotationSpeed = 1f;

        public float patrolDetectionRadius;
        public float patrolDetectionAngle; //Mitad del ángulo que forma el campo de visión del enemigo mientras patrullea
        public float seekingDetectionRadius;
        public float seekingDetectionAngle; //Mitad del ángulo que forma el campo de visión del enemigo mientras tiene detectado al player o mientras lo busca

        public float minDistanceFromPlayer;
        public float maxDistanceFromPlayer;

        private Transform playerTransform;
        private Vector3 lastPlayerPosition;

        public float timeUntilSeek;
        public float timeSeeking;

        public bool sableEnemy;

        void Awake()
        {
            SableController = GetComponent<EnemySableController>();
            
            NavMeshAgent = GetComponent<NavMeshAgent>();
            
            CharacterAnimationController = GetComponent<CharacterAnimationController>();

            EnemyCharacterDetection[] enemyCharacterDetectors = GetComponentsInChildren<EnemyCharacterDetection>();
            for(int i = 0; i < enemyCharacterDetectors.Length; i++) 
            {
                enemyCharacterDetectors[i].enemyScript = this;
                
                if(enemyCharacterDetectors[i].sphereCollider) {
                    SphereCollider = enemyCharacterDetectors[i].GetComponent<SphereCollider>();
                }
            }

            playerTransform = GameManager.Instance.LocalPlayer.transform;
        }

        void Start()  {
            inPath = true;
            seeking = false;
            following = false;
            characterNotInRange = false;
            waitInPoint = false;
            nextPointPath = false;
            firstPoint = true;
            toLastPlayerPosition = false;

            stopAI = false;

            waitingTimer = untilSeekingTimer = seekingTimer = 0.0f;

            ChangeVisionField(following);

            CreatePathPoints();

            if(!stopAI) SetFirstPointPath();
        }

        void Update() 
        {
            if(!stopAI)
            {
                if(!SableController.GetAttacking())
                {
                    if(inPath && !prepareToAttack)
                    {
                        if(!waitInPoint)
                        {
                            if(nextPointPath)
                            {
                                SetNextPointPath();
                            }
                            if(!nextPointPath && NavMeshAgent.remainingDistance == 0f)
                            {
                                firstPoint = false;

                                waitingTime = Random.Range(patrolMinWaitingTime, patrolMaxWaitingTime);
                                waitInPoint = true;
                            }
                            nextPointPath = false;
                        }
                        else
                        {
                            WaitInPointPath();
                        }
                        if(!firstPoint && seeking) SeekingTimer();
                    }
                    else if(following && !SableController.GetBlocking())
                    {
                        FollowingPlayer();
                    }
                    else if(SableController.GetBlocking())
                    {
                        TurnEnemyWhileBlocking();
                    }

                    if(prepareToAttack)
                    {
                        prepareToAttack = SableController.AIAttack();
                    }
                    else if(!SableController.GetAttacking()) SableController.ResetAIAttack();
                }

                if(characterNotInRange)
                {
                    UntilSeekingTimer();
                }

                if(toLastPlayerPosition)
                {
                    ToLastPositionPlayer();
                }

                ChangeVisionField(following || seeking);
            }
            
            if(!SableController.GetAttacking()) CharacterAnimationController.MovingAnimation(NavMeshAgent.velocity.magnitude, running);
        }

        private void SetRunning(bool param)
        {
            running = param;
            NavMeshAgent.speed = param ? runningSpeed : movingSpeed;
        }

        private void SetFirstPointPath()
        {
            firstPoint = true;
            NavMeshAgent.SetDestination(CalculateNextPointPath());

            SetRunning(true);
        }

        public float DistanceFromPlayer()
        {
            Vector3 enemyToPlayer = playerTransform.position - transform.position;
            enemyToPlayer.y = 0f;
            return enemyToPlayer.magnitude;
        }

        public float GetDetectionRadius()
        {
            return detectionRadius;
        }

        private Vector3 CalculateNextPointPath()
        {
            Transform[] wayPoints;
            if(seeking)
            {
                wayPoints = patrolPoints; //Debería ser seekingPoints
            }
            else 
            {
                wayPoints = patrolPoints;
            }

            if(firstPoint) 
            {
                destinationPointIndex = 0;
                float minMagnitude = (wayPoints[destinationPointIndex].position - transform.position).magnitude;

                for (int i = 1; i < wayPoints.Length; i++) 
                {
                    if ((wayPoints[i].position - transform.position).magnitude < minMagnitude) 
                    {
                        minMagnitude = (wayPoints[i].position - transform.position).magnitude;
                        destinationPointIndex = i;
                    }
                }

                return wayPoints[destinationPointIndex].position;
            }
            else
            {
                destinationPointIndex++;
                if(destinationPointIndex == wayPoints.Length)
                {
                    destinationPointIndex = 0;
                }

                return wayPoints[destinationPointIndex].position;
            }
        }

        private void SetNextPointPath()
        {
            Vector3 nextDestination = CalculateNextPointPath();
            NavMeshAgent.SetDestination(nextDestination);

            SetRunning(seeking);
        }

        private void WaitInPointPath()
        {

            waitingTimer += Time.deltaTime;
            if(waitingTimer >= waitingTime) 
            {
                waitInPoint = false;
                nextPointPath = true;

                waitingTimer = 0f;
            }
        }

        private void ToLastPositionPlayer()
        {
            print("hol");
            NavMeshAgent.SetDestination(lastPlayerPosition);

            SetRunning(true);

            if(NavMeshAgent.remainingDistance == 0f)
            {
                following = false;
                inPath = true;
                seeking = true;

                toLastPlayerPosition = false;

                SetFirstPointPath();
            }
        }

        private void UntilSeekingTimer()
        {
            untilSeekingTimer += Time.deltaTime;

            if(untilSeekingTimer >= timeUntilSeek && !SableController.GetAttacking())
            {
                toLastPlayerPosition = true;
                lastPlayerPosition = playerTransform.position;
                characterNotInRange = false;

                untilSeekingTimer = 0f;
            }
        }

        private void SeekingTimer()
        {
            seekingTimer += Time.deltaTime;

            if(seekingTimer >= timeSeeking)
            {
                seeking = false;

                seekingTimer = 0.0f;
            }
        }

        private void FollowingPlayer()
        {
            float distanceFromPlayer = DistanceFromPlayer();

            if(distanceFromPlayer < maxDistanceFromPlayer)
            {
                SetRunning(false);

                NavMeshAgent.isStopped = true;
                NavMeshAgent.ResetPath();

                Vector3 lookPos = playerTransform.position - transform.position;
                lookPos.y = 0;
                Quaternion rotation = Quaternion.LookRotation(lookPos);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);

                if(distanceFromPlayer < minDistanceFromPlayer && !playerTransform.GetComponent<CharacterSableController>().GetAttacking())
                {
                    prepareToAttack = false;

                    NavMeshAgent.Move(-transform.forward * movingSpeed * Time.deltaTime);
                }
                else
                {
                    prepareToAttack = true;
                }
            }
            else 
            {
                prepareToAttack = false;

                SetRunning(true);

                NavMeshAgent.SetDestination(playerTransform.position);
            }
        }

        public void TurnEnemyWhileBlocking()
        {
            Vector3 lookPos = playerTransform.position - transform.position;
            lookPos.y = 0;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, blockingRotationSpeed * Time.deltaTime);
        }

        public void CharacterDetection(bool collidingSphere, bool collidingEnemy)
        {
            bool result = false;
            if(collidingSphere) //El personaje está dentro de la esfera de colisión
            {
                Vector3 relativePositionToPlayer = playerTransform.position - transform.position; //Posición relativa del jugador con respecto al enemigo
                Vector3 globalForwardFromEnemy = transform.TransformDirection(Vector3.forward); //Vector dirección del frente del enemigo

                float angleBetweenVectors = Vector3.Angle(relativePositionToPlayer, globalForwardFromEnemy);

                if(angleBetweenVectors < detectionAngle || following) //El personaje está dentro del campo de visión del enemigo
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

            if(result && inPath) 
            {
                inPath = false;
                following = true;
            }
            else if(!result && following) 
            {
                characterNotInRange = true;
            }
            else if(result && characterNotInRange)
            {
                characterNotInRange = false;
            }
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

        /*private IEnumerator TimerUntilSeek(float time)
        {
            yield return new WaitForSeconds(time);

            lastPlayerPosition = playerTransform.position;
        }*/

        /*public void CharacterDetection(bool param)
        {
            bool result = false;
            if(param) //El personaje está dentro de la esfera de colisión
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

            //print("result: " + result + " characterDetect: " + characterDetect);
            if(result && !characterDetect) StartFollowPlayer();
            else if(!result && characterDetect) StartTimerUntilSeek();

            ChangeVisionField(result);
        }*/

        /* private void StartPath(bool seeking)
        {
            //print("En recorrido");
            float randomTime = Random.Range(patrolMinWaitingTime, patrolMaxWaitingTime);

            if(enemyCoroutine != null) StopCoroutine(enemyCoroutine);
            enemyCoroutine = StartCoroutine(FollowPath(patrolPoints, randomTime, seeking));
        }

        IEnumerator FollowPath(Transform[] waypoints, float waitingTime, bool seeking) { //recorrido en busqueda y patrulla
            
            //encuentro el punto mas cercano
            int destinationPointIndex =  0;
            float minMagnitude = (waypoints[0].position - transform.position).magnitude;

            for (int i = 1; i < waypoints.Length; i++) 
            {
                if ((waypoints[i].position - transform.position).magnitude < minMagnitude) 
                {
                    minMagnitude = (waypoints[i].position - transform.position).magnitude;
                    destinationPointIndex = i;
                }
            }

            //el enemigo va para el punto mas cercano
            NavMeshAgent.SetDestination(waypoints[destinationPointIndex].position);
            NavMeshAgent.speed = runningSpeed;
            CharacterAnimationController.MovingAnimation(true, true);
            bool firstPoint = true;

            //recorrer el path
            while (true) 
            {
                print("En recorrido");
                if(firstPoint && seeking) {
                    StartCoroutine(SeekingTimer(timeSeeking));
                    firstPoint = false;
                }

                if (NavMeshAgent.remainingDistance == 0) {

                    CharacterAnimationController.MovingAnimation(false, seeking);

                    if (destinationPointIndex != waypoints.Length - 1) 
                    {
                        destinationPointIndex += 1;
                    }
                    else 
                    {
                        destinationPointIndex = 0;
                    }

                    yield return new WaitForSeconds(waitingTime);
                    CharacterAnimationController.MovingAnimation(true, seeking);

                    NavMeshAgent.SetDestination(waypoints[destinationPointIndex].position);

                    if (seeking) NavMeshAgent.speed = runningSpeed;
                    else NavMeshAgent.speed = movingSpeed;
                    
                }

                yield return null;
            }

        }

        IEnumerator SeekingTimer(float time)
        {
            yield return new WaitForSeconds(time);

            StartPath(false);
        }

        public void StartTimerUntilSeek()
        {
            StartCoroutine(TimerUntilSeek(timeUntilSeek));
        }

        IEnumerator TimerUntilSeek(float time)
        {
            print("Puede que empiece a buscar el player");
            yield return new WaitForSeconds(time);

            lastPlayerPosition = playerTransform.position;
            StartSeekPlayer();
        }

        private void StartFollowPlayer()
        {
            characterDetect = true;

            if(enemyCoroutine != null) StopCoroutine(enemyCoroutine);
            enemyCoroutine = StartCoroutine(FollowPlayer());
        }

        IEnumerator FollowPlayer()
        {
            //print("Siguiendo al player");

            while(true)
            {
                print("Siguiendo al player");
                Vector3 enemyToPlayer = playerTransform.position - transform.position;
                enemyToPlayer.y = 0;
                float distanceFromPlayer = enemyToPlayer.magnitude;

                if(distanceFromPlayer < maxDistanceFromPlayer && distanceFromPlayer > minDistanceFromPlayer * 0.9f)
                {
                    CharacterAnimationController.MovingAnimation(false, false);
                    NavMeshAgent.isStopped = true;
                    NavMeshAgent.ResetPath();
                    
                    //yield return new WaitForSeconds(5f);
                    
                    //print("Atacar!");
                }
                else if(distanceFromPlayer < minDistanceFromPlayer)
                {
                    CharacterAnimationController.MovingAnimation(false, false);
                    NavMeshAgent.isStopped = true;
                    NavMeshAgent.ResetPath();
                    
                    Vector3 lookPos = playerTransform.position - transform.position;
                    lookPos.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);

                    NavMeshAgent.Move(-transform.forward * movingSpeed * Time.deltaTime);
                }
                else {
                    
                    CharacterAnimationController.MovingAnimation(true, true);
                    NavMeshAgent.SetDestination(playerTransform.position);
                }
                yield return null;
            }
        }

        private void StartSeekPlayer()
        {
            characterDetect = false;

            if(enemyCoroutine != null) StopCoroutine(enemyCoroutine);
            enemyCoroutine = StartCoroutine(SeekPlayer());
        }

        IEnumerator SeekPlayer()
        {
            //print("Buscando al player");
            NavMeshAgent.SetDestination(lastPlayerPosition);

            while(true)
            {
                print("Buscando al player");
                if(NavMeshAgent.remainingDistance == 0f)
                {
                    StartPath(true);
                }
                yield return null;
            }

            yield return null;
        }*/

        /*public Material InitialMaterial; 1
        public Material KnockbackMaterial; 2
        public float KnockbackTime = 0.5f; 3
        //public float knockbackForce = 10f; 4
        private bool knockback;

        public float InitialHealth = 100f;
        private float health;

        private MeshRenderer meshRenderer;
        private Rigidbody rigidbody;
        private ParticleSystem hitParticles;

        // Start is called before the first frame update
        void Awake()
        {
            meshRenderer = GetComponent<MeshRenderer>();
            hitParticles = GetComponentInChildren<ParticleSystem>();
            InitialMaterial = meshRenderer.material;

            rigidbody = GetComponent<Rigidbody>();
        }

        void Start()
        {
            health = InitialHealth;
            knockback = false;
        }

        public void AttackedBySword(float damage, Vector3 direction)
        {
            if(!knockback)
            {
                health -= damage;
                if(health <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    StartCoroutine(Knockback(KnockbackTime, direction, damage/4));
                }
            }
            
        }

        public void AttackedByGun(float damage, Vector3 direction, Vector3 hitPoint)
        {
            if(!knockback)
            {
                hitParticles.transform.position = hitPoint;

                hitParticles.Play();

                health -= damage;
                if(health <= 0)
                {
                    Destroy(gameObject);
                }
                else
                {
                    StartCoroutine(Knockback(KnockbackTime, direction, damage/4));
                }
            }
            
        }

        IEnumerator Knockback(float time, Vector3 direction, float knockbackForce)
        {
            meshRenderer.material = KnockbackMaterial;
            knockback = true;

            rigidbody.AddForce(direction.normalized * knockbackForce, ForceMode.Impulse);

            yield return new WaitForSeconds(time);

            meshRenderer.material = InitialMaterial;
            knockback = false;
        }*/
    }
}