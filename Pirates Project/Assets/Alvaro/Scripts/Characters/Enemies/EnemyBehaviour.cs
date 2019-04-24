using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace DefinitiveScript
{
    public class EnemyBehaviour : MonoBehaviour
    {
        private SableController SableController;
        private NavMeshAgent nav;
        private CharacterAnimationController characterAnimationController;
        private SphereCollider sphereCollider;

        private Transform[] patrolPoints;
        private float detectionAngle;

        private bool characterDetect;
        private bool inPath;
        
        public Transform PatrolPathObj; 
        public float movingSpeed;
        public float runningSpeed;
        public float patrolMinWaitingTime;
        public float patrolMaxWaitingTime;

        public float patrolDetectionRadius;
        public float patrolDetectionAngle;
        public float seekingDetectionRadius;
        public float seekingDetectionAngle;

        void Awake()
        {
            SableController = GetComponent<SableController>();
            
            nav = GetComponent<NavMeshAgent>();
            
            characterAnimationController = GetComponent<CharacterAnimationController>();

        }

        void Start() {

            //sphereCollider.radius = patrolDetectionRadius;
            detectionAngle = patrolDetectionAngle;

            characterDetect = false;
            inPath = false;

            //CreatePatrolPoints();
        }

        void CreatePatrolPoints()
        {
            patrolPoints = new Transform[PatrolPathObj.childCount];

            for(int i = 0; i < PatrolPathObj.childCount; i++)
            {
                patrolPoints[i] = PatrolPathObj.GetChild(i);
            }
        }

        void Update()
        {
            if(characterDetect)
            {
                if(!inPath)
                {
                    inPath = true;

                    float randomTime = Random.Range(patrolMinWaitingTime, patrolMaxWaitingTime);
                    StartCoroutine(PatrolPath(patrolPoints, randomTime));
                }
            }

            if(Input.GetKeyDown(KeyCode.E)) SableController.ComboAttack();
            SableController.Block(Input.GetKey(KeyCode.F));
        }

        IEnumerator PatrolPath(Transform[] waypoints, float waitingTime) { //recorrido en busqueda y patrulla
            //encuentro el punto mas cercano
            
            int destinationPointIndex =  0;
            float minMagnitude = (waypoints[0].position - transform.position).magnitude;
            for (int i = 1; i < waypoints.Length; i++) {
                if ((waypoints[i].position - transform.position).magnitude < minMagnitude) {
                    minMagnitude = (waypoints[i].position - transform.position).magnitude;
                    destinationPointIndex = i;
                }
            }
            //el enemigo va para el punto mas cercano
            nav.SetDestination(waypoints[destinationPointIndex].position);
            nav.speed = runningSpeed;

            //recorrer el path
            while (true) {
                characterAnimationController.MovingAnimation(true, !characterDetect);
                if (nav.remainingDistance == 0) {
                    characterAnimationController.MovingAnimation(false, !characterDetect);
                    if (destinationPointIndex != waypoints.Length - 1) {
                        destinationPointIndex += 1;
                    }
                    else {
                        destinationPointIndex = 0;
                    }

                    yield return new WaitForSeconds(waitingTime);

                    nav.SetDestination(waypoints[destinationPointIndex].position);
                    if (!characterDetect) nav.speed = movingSpeed;
                    else nav.speed = runningSpeed;
                    
                }
                yield return null;

            }

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