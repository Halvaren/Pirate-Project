using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DefinitiveScript 
{
    public class SceneController : MonoBehaviour
    {
        private bool created;
        private PlayerBehaviour PlayerBehaviour;
        private GameObject GM;

        void Awake()
        {
            if (!created)
            {
                DontDestroyOnLoad(GameManager.Instance.GameObject);
                DontDestroyOnLoad(this.gameObject);
                created = true;
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Start()
        {
            GameManager.Instance.SceneController = this;

            GameObject aux = GameObject.Find("Player");
            if(aux != null)
            {
                PlayerBehaviour = aux.GetComponent<PlayerBehaviour>();

                GameManager.Instance.LocalPlayer = PlayerBehaviour;
                PlayerBehaviour.enabled = true;

                GameManager.Instance.CursorController.LockCursor();
            }
        }

        public void ChangeToScene(string name)
        {
            PantallaDeCarga.Instancia.CargarEscena(name);
        }
        public void ChangeToScene(int id)
        {
            SceneManager.LoadScene(id);
        }
    }
}