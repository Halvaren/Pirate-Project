using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DefinitiveScript 
{
    public class Shooting : MonoBehaviour
    {
        public float damage = 50f;
        public float range = 1000f;
        public float timeBetweenBullets = 1f;
        public float effectsDisplayTime = 0.1f;

        public GameObject effectObject;
        private ParticleSystem gunParticles;
        private LineRenderer gunLine;
        private Light faceLight;

        public GameObject scopeMark;
        private Image scopeMarkRenderer;

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

        /* private CharacterAnimationController m_CharacterAnimationController;
        public CharacterAnimationController CharacterAnimationController
        {
            get {
                if (m_CharacterAnimationController == null) m_CharacterAnimationController = GetComponent<CharacterAnimationController>();
                return m_CharacterAnimationController;
            }
        }*/

        // Start is called before the first frame update
        void Awake()
        {
            gunParticles = effectObject.GetComponent<ParticleSystem>();
            gunLine = effectObject.GetComponent<LineRenderer>();
            faceLight = effectObject.GetComponentInChildren<Light>();

            scopeMarkRenderer = scopeMark.GetComponent<Image>();
        }

        void Start()
        {
            ableToShoot = true;

            gunParticles.Stop();
            gunLine.enabled = false;
            faceLight.enabled = false;
            scopeMarkRenderer.enabled = false;
        }

        void Update()
        {
            Debug.DrawLine(scopeMark.transform.position, scopeMark.transform.position + scopeMark.transform.forward * range, Color.red);
            if(gunPrepared)
            {
                if(elapsedTime < timeBetweenBullets)
                {
                    elapsedTime += Time.deltaTime;
                    scopeMarkRenderer.enabled = false;
                }
                else {
                    ableToShoot = true;
                    scopeMarkRenderer.enabled = true;
                }
            }
            else
            {
                scopeMarkRenderer.enabled = false;
            }
        }

        // Update is called once per frame
        public bool Shoot()
        {
            if(ableToShoot)
            {
                Vector3 shootingPoint;
                Vector3 hitDirection;
                DebugEnemyBehaviour enemy = CalculateShootingPoint(out shootingPoint, out hitDirection);
                StartCoroutine(PlayEffects(effectsDisplayTime, shootingPoint, hitDirection, enemy));

                elapsedTime = 0.0f;
                ableToShoot = false;

                return true;
            }
            return false;
        }

        private DebugEnemyBehaviour CalculateShootingPoint(out Vector3 shootingPoint, out Vector3 hitDirection)
        {
            RaycastHit hit;

            if(Physics.Raycast(scopeMark.transform.position, scopeMark.transform.forward, out hit, range, shootableMask))
            {
                shootingPoint = hit.point;
                hitDirection = -hit.normal;
                DebugEnemyBehaviour enemy = hit.transform.gameObject.GetComponent<DebugEnemyBehaviour>();
                if(enemy != null)
                {
                    return enemy;
                }
            }
            shootingPoint = scopeMark.transform.position + scopeMark.transform.forward * range;
            hitDirection = Vector3.zero;
            return null;   
        }

        private IEnumerator PlayEffects(float time, Vector3 shootingPoint, Vector3 hitDirection, DebugEnemyBehaviour enemy)
        {
            gunLine.SetPosition(0, effectObject.transform.position);
            gunLine.SetPosition(1, shootingPoint);
            gunLine.enabled = true;

            faceLight.enabled = true;

            gunParticles.Play();

            yield return new WaitForSeconds(time);

            if(enemy != null) enemy.Attacked(damage, hitDirection);
            
            gunLine.enabled = false;
            faceLight.enabled = false;
        }
    }
}

