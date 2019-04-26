using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace DefinitiveScript
{
    public class EnemyBehaviour : CharacterBehaviour
    {
        private SableController SableController;
        private NavMeshAgent NavMeshAgent;
        private CharacterAnimationController CharacterAnimationController;
        private SphereCollider SphereCollider;

        private Transform[] patrolPoints;
        private float detectionRadius;
        private float detectionAngle;

        public LayerMask visionObstacles;

        private bool characterDetect;
        private bool characterInRange;
        private bool inPath;

        private Coroutine enemyCoroutine;
        
        public Transform PatrolPathObj; 
        public float movingSpeed;
        public float runningSpeed;
        public float patrolMinWaitingTime;
        public float patrolMaxWaitingTime;

        public float patrolDetectionRadius;
        public float patrolDetectionAngle; //Mitad del ángulo que forma el campo de visión del enemigo mientras patrullea
        public float seekingDetectionRadius;
        public float seekingDetectionAngle; //Mitad del ángulo que forma el campo de visión del enemigo mientras tiene detectado al player o mientras lo busca

        public float minDistanceFromPlayer;
        public float maxDistanceFromPlayer;

        private Transform playerTransform;
        private Vector3 lastPlayerPosition;

        public float timeUntilSeek = 3f;
        public float timeSeeking;

        void Awake()
        {
            SableController = GetComponent<SableController>();
            
            NavMeshAgent = GetComponent<NavMeshAgent>();
            
            CharacterAnimationController = GetComponent<CharacterAnimationController>();

            GetComponentInChildren<EnemyCharacterDetection>().enemyScript = this;
            SphereCollider = GetComponentInChildren<EnemyCharacterDetection>().GetComponent<SphereCollider>();

            playerTransform = GameManager.Instance.LocalPlayer.transform;
        }

        void Start()  {
            characterDetect = false;
            inPath = false;

            ChangeVisionField(characterDetect);

            CreatePathPoints();

            StartPath(false);
        }

        void CreatePathPoints()
        {
            patrolPoints = new Transform[PatrolPathObj.childCount];

            for(int i = 0; i < PatrolPathObj.childCount; i++)
            {
                patrolPoints[i] = PatrolPathObj.GetChild(i);
            }
        }

        void Update()
        {

        }

        public void CharacterDetection(bool param)
        {
            bool result = false;
            if(param) //El personaje está dentro de la esfera de colisión
            {
                Vector3 relativePositionToPlayer = playerTransform.position - transform.position; //Posición relativa del jugador con respecto al enemigo
                Vector3 globalForwardFromEnemy = transform.TransformDirection(Vector3.forward); //Vector dirección del frente del enemigo

                float angleBetweenVectors = Vector3.Angle(relativePositionToPlayer, globalForwardFromEnemy);
                //Debug.Log(angleBetweenVectors);
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

            print(result);
            if(result && !characterInRange) StartFollowPlayer();
            else if(!result && characterInRange) StartTimerUntilSeek();
            else if(!result && !characterInRange && characterDetect)
            {
                lastPlayerPosition = playerTransform.position;
                StartSeekPlayer();
            }

            ChangeVisionField(result);
        }

        private void ChangeVisionField(bool param)
        {
            SphereCollider.radius = detectionRadius = param ? seekingDetectionRadius : patrolDetectionRadius;
            detectionAngle = param ? seekingDetectionAngle : patrolDetectionAngle;
        }

        private void StartPath(bool seeking)
        {
            float randomTime = Random.Range(patrolMinWaitingTime, patrolMaxWaitingTime);

            if(enemyCoroutine != null) StopCoroutine(enemyCoroutine);
            enemyCoroutine = StartCoroutine(FollowPath(patrolPoints, randomTime, seeking));

            if(seeking) StartCoroutine(SeekingTimer(timeSeeking));
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


            //recorrer el path
            while (true) 
            {
                
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
            print("Puede que empiece a buscar el player");
            StartCoroutine(TimerUntilSeek(timeUntilSeek));
        }

        IEnumerator TimerUntilSeek(float time)
        {
            yield return new WaitForSeconds(time);

            characterInRange = false;
        }

        private void StartFollowPlayer()
        {
            characterInRange = characterDetect = true;

            if(enemyCoroutine != null) StopCoroutine(enemyCoroutine);
            enemyCoroutine = StartCoroutine(FollowPlayer());
        }

        IEnumerator FollowPlayer()
        {
            print("Siguiendo al player");

            while(true)
            {
                if(NavMeshAgent.remainingDistance < maxDistanceFromPlayer && NavMeshAgent.remainingDistance > minDistanceFromPlayer)
                {
                    CharacterAnimationController.MovingAnimation(false, false);
                    NavMeshAgent.isStopped = true;
                    NavMeshAgent.ResetPath();
                    
                    print("Atacar!");
                }
                else if(NavMeshAgent.remainingDistance < minDistanceFromPlayer)
                {
                    CharacterAnimationController.MovingAnimation(false, false);
                    NavMeshAgent.isStopped = true;
                    NavMeshAgent.ResetPath();
                    
                    Vector3 lookPos = playerTransform.position - transform.position;
                    lookPos.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(lookPos);
                    transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);/*

                    NavMeshAgent.Move(-Vector3.forward*movingSpeed*Time.deltaTime);*/
                    print("Problema");
                }
                else {
                    
                    CharacterAnimationController.MovingAnimation(true, true);
                    NavMeshAgent.SetDestination(playerTransform.position);
                }
                yield return null;
            }

            //yield return null;
        }

        private void StartSeekPlayer()
        {
            if(enemyCoroutine != null) StopCoroutine(enemyCoroutine);
            enemyCoroutine = StartCoroutine(SeekPlayer());
        }

        IEnumerator SeekPlayer()
        {
            print("Buscando al player");
            NavMeshAgent.SetDestination(lastPlayerPosition);

            /*while(true)
            {
                if(NavMeshAgent.remainingDistance == 0f)
                {
                    StartPath(true);
                }
                yield return null;
            }*/

            yield return null;
        }

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