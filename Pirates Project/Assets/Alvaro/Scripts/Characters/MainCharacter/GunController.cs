using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefinitiveScript 
{
    public class GunController : MonoBehaviour
    {
        public float damage = 50f;
        public float range = 1000f;
        public float timeBetweenBullets = 1f;
        public float effectsDisplayTime = 0.1f;

        public GameObject effectObject;
        private ParticleSystem gunParticles;
        private LineRenderer gunLine;
        private Light faceLight;

        public GameObject scopeMarkCamera;
        public GameObject scopeMarkOverlay;
        private Image scopeMarkCameraRenderer;
        private Image scopeMarkOverlayRenderer;

        private float elapsedTime;
        private bool ableToShoot;
        private bool m_GunPrepared;
        public bool gunPrepared {
            get { return m_GunPrepared; }
            set { m_GunPrepared = value; }
        }

        [SerializeField] LayerMask shootableMask;

        private Player m_LocalPlayer;
        public Player LocalPlayer {
            get {
                if(m_LocalPlayer == null) m_LocalPlayer = GetComponent<Player>();
                return m_LocalPlayer;
            }
        }

        // Start is called before the first frame update
        void Awake()
        {
            gunParticles = effectObject.GetComponent<ParticleSystem>();
            gunLine = effectObject.GetComponent<LineRenderer>();
            faceLight = effectObject.GetComponentInChildren<Light>();

            scopeMarkCameraRenderer = scopeMarkCamera.GetComponent<Image>();
            scopeMarkOverlayRenderer = scopeMarkOverlay.GetComponent<Image>();
        }

        void Start()
        {
            ableToShoot = true;

            gunParticles.Stop();
            gunLine.enabled = false;
            faceLight.enabled = false;
            scopeMarkCameraRenderer.enabled = false;
            scopeMarkOverlayRenderer.enabled = false;
        }

        void Update()
        {
            Debug.DrawLine(scopeMarkCamera.transform.position, scopeMarkCamera.transform.position + scopeMarkCamera.transform.forward * range, Color.red);
            if(gunPrepared)
            {
                if(elapsedTime < timeBetweenBullets)
                {
                    elapsedTime += Time.deltaTime;
                    scopeMarkCameraRenderer.enabled = false;
                    scopeMarkOverlayRenderer.enabled = false;
                }
                else {
                    ableToShoot = true;
                    scopeMarkCameraRenderer.enabled = true;
                    scopeMarkOverlayRenderer.enabled = true;
                }
            }
            else
            {
                scopeMarkCameraRenderer.enabled = false;
                scopeMarkOverlayRenderer.enabled = false;
            }
        }

        // Update is called once per frame
        public bool Shoot()
        {
            if(ableToShoot)
            {
                Vector3 shootingPoint;
                Vector3 hitDirection;
                EnemyBehaviour enemy = CalculateShootingPoint(out shootingPoint, out hitDirection);
                StartCoroutine(PlayEffects(effectsDisplayTime, shootingPoint, hitDirection, enemy));

                elapsedTime = 0.0f;
                ableToShoot = false;

                return true;
            }
            return false;
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

            //if(enemy != null) enemy.AttackedByGun(damage, hitDirection, shootingPoint);
            
            gunLine.enabled = false;
            faceLight.enabled = false;
        }
    }
}

