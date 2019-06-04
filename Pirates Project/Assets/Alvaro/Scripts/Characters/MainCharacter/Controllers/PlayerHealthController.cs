using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript 
{
    public class PlayerHealthController : HealthController
    {
        protected CharacterController m_CharacterController;
        public CharacterController CharacterController
        {
            get {
                if(m_CharacterController == null) m_CharacterController = GetComponent<CharacterController>();
                return m_CharacterController;
            }
        }

        public PlayerUIController PlayerUIController;

        public float initialReloadEnergy = 100f;
        public float recoveringReloadEnergySpeed = 5f;

        private bool ableToShoot;
        private float reloadEnergy = 100f;

        private int currentMoney = 0;

        private bool hasKey = false;

        protected override void Update()
        {
            base.Update();
            if(!ableToShoot)
            {
                RecoverReloadEnergy();
            }
        }

        protected override IEnumerator PlayKnockback(Vector3 impact, float time)
        {
            float elapsedTime = 0.0f;

            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;
                CharacterController.Move(impact * Time.deltaTime);
                impact = Vector3.Lerp(impact, Vector3.zero, elapsedTime / time);
                yield return null;
            }
        }

        public override bool TakeDamage(float damage)
        {
            if(CharacterBehaviour.GetAlive())
            {
                health -= damage;

                if(health <= 0f)
                {   

                    GameManager.Instance.AIEnemyController.PlayerDead();

                    GameManager.Instance.SceneController.ShowDeathText();

                    GetComponent<PlayerBehaviour>().stopInput = true;
                    CharacterBehaviour.SetAlive(false);

                    
                    GetComponent<PlayerSoundController>().PlayDeath();
                    GetComponent<PlayerAnimatorController>().Die();
                }

                PlayerUIController.UpdateHealthBar(health <= 0f ? 0f : health, false);

                return health <= 0f;
            }
            return false;
        }

        public void RecoverHealth(float amount)
        {
            health += amount;
            if(health > initialHealth) health = initialHealth;

            PlayerUIController.UpdateHealthBar(health, true);
        }

        public override bool ReduceStamina(float amount)
        {
            stamina -= amount;

            if(stamina <= 0f) 
            {
                stamina = 0f;
                runOutOfStamina = true;
            }

            PlayerUIController.UpdateStaminaBar(stamina <= 0f ? 0f: stamina);

            return stamina <= 0f; //Devuelve true si el personaje ha perdido toda su stamina
        }

        protected override void RecoverStamina()
        {
            base.RecoverStamina();

            PlayerUIController.UpdateStaminaBar(stamina);
        }

        public void PlayerHasShot()
        {
            ableToShoot = false;
            reloadEnergy = 0f;

            PlayerUIController.UpdateReloadGunBar(reloadEnergy);
        }

        protected void RecoverReloadEnergy()
        {
            if(reloadEnergy < initialReloadEnergy)
            {
                reloadEnergy += recoveringReloadEnergySpeed * Time.deltaTime;
            }
            else
            {
                reloadEnergy = initialReloadEnergy;
                ableToShoot = true;
            }

            PlayerUIController.UpdateReloadGunBar(reloadEnergy);
        }

        public bool GetAbleToShoot()
        {
            return ableToShoot;
        }

        public void IncreaseMoney(int increase)
        {
            currentMoney += increase;
            if(currentMoney < 0) currentMoney = 0;

            PlayerUIController.IncreaseMoney(increase);
        }

        public void ObtainKey()
        {
            hasKey = true;

            PlayerUIController.EnableKey(true);
        }

        public bool HasKey()
        {
            return hasKey;
        }
    }
}