using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefinitiveScript 
{
    public class PlayerGunController : MonoBehaviour
    {
        public float damage = 50f;
        public float range = 1000f;
        public float timeBetweenShots = 1f;
        public float effectsDisplayTime = 0.1f;

        public GameObject effectObject;
        private ParticleSystem gunParticles;
        private LineRenderer gunLine;
        private Light faceLight;

        public GameObject scopeMarkCamera;
        public GameObject scopeMarkOverlay;
        private Image scopeMarkOverlayRenderer;

        private bool m_GunPrepared;
        public bool gunPrepared {
            get { return m_GunPrepared; }
            set { m_GunPrepared = value; }
        }

        [SerializeField] LayerMask shootableMask;

        private PlayerBehaviour m_LocalPlayer;
        public PlayerBehaviour LocalPlayer {
            get {
                if(m_LocalPlayer == null) m_LocalPlayer = GetComponent<PlayerBehaviour>();
                return m_LocalPlayer;
            }
        }

        private PlayerHealthController m_HealthController;
        public PlayerHealthController HealthController
        {
            get {
                if(m_HealthController == null) m_HealthController = GetComponent<PlayerHealthController>();
                return m_HealthController;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            gunParticles = effectObject.GetComponent<ParticleSystem>();
            gunLine = effectObject.GetComponent<LineRenderer>();
            faceLight = effectObject.GetComponentInChildren<Light>();

            scopeMarkOverlayRenderer = scopeMarkOverlay.GetComponent<Image>();
        }

        void Start()
        {
            gunPrepared = false;

            gunParticles.Stop();
            gunLine.enabled = false;
            faceLight.enabled = false;
            scopeMarkOverlayRenderer.enabled = false;
        }

        void Update()
        {
            EnableScopeMarkRenderer(gunPrepared);
            //Debug.DrawLine(scopeMarkCamera.transform.position, scopeMarkCamera.transform.position + scopeMarkCamera.transform.forward * range, Color.red);
        }

        // Update is called once per frame
        public bool Shoot()
        {
            if(gunPrepared && HealthController.GetAbleToShoot())
            {
                Vector3 shootingPoint;
                Vector3 hitDirection;
                EnemyBehaviour enemy = CalculateShootingPoint(out shootingPoint, out hitDirection);
                StartCoroutine(PlayEffects(effectsDisplayTime, shootingPoint, hitDirection, enemy));

                HealthController.PlayerHasShot();

                return true;
            }
            return false;
        }

        public void EnableScopeMarkRenderer(bool value)
        {
            scopeMarkOverlayRenderer.enabled = value;
        }

        private EnemyBehaviour CalculateShootingPoint(out Vector3 shootingPoint, out Vector3 hitDirection)
        {
            RaycastHit hit;

            if(Physics.Raycast(scopeMarkCamera.transform.position, scopeMarkCamera.transform.forward, out hit, range, shootableMask))
            {
                shootingPoint = hit.point;
                hitDirection = -hit.normal;
                EnemyBehaviour enemy = hit.transform.gameObject.GetComponent<EnemyBehaviour>();
                if(enemy != null)
                {
                    return enemy;
                }
            }
            shootingPoint = scopeMarkCamera.transform.position + scopeMarkCamera.transform.forward * range;
            hitDirection = Vector3.zero;
            return null;   
        }

        private IEnumerator PlayEffects(float time, Vector3 shootingPoint, Vector3 hitDirection, EnemyBehaviour enemy)
        {
            gunLine.SetPosition(0, effectObject.transform.position);
            gunLine.SetPosition(1, shootingPoint);
            gunLine.enabled = true;

            faceLight.enabled = true;

            gunParticles.Play();

            yield return new WaitForSeconds(time);

            if(enemy != null) {
                EnemyHealthController enemyHC = enemy.GetComponent<EnemyHealthController>();

                enemyHC.Knockback(5f, hitDirection, true);
                enemyHC.AttackedByGunParticles(shootingPoint);
                if(!enemyHC.TakeDamage(damage))
                    enemy.GetComponent<EnemyBehaviour>().HitOnBody();
            }
            
            gunLine.enabled = false;
            faceLight.enabled = false;
        }
    }
}

