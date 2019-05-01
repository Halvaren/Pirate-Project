using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class GameManager
    {
        private GameObject gameObject;
        public GameObject GameObject
        {
            get {
                return gameObject;
            }
        }

        private static GameManager m_Instance; //Instancia estática del GameManager. Es singleton
        public static GameManager Instance {
            get{
                if(m_Instance == null)
                {
                    m_Instance = new GameManager();
                    m_Instance.gameObject = new GameObject("_gameManager");

                    m_Instance.gameObject.AddComponent<CursorController>();
                    m_Instance.gameObject.AddComponent<InputController>();
                    m_Instance.gameObject.AddComponent<AudioController>();
                    m_Instance.gameObject.AddComponent<BundleController>();
                }

                return m_Instance;
            }
        }

        private InputController m_InputController; //Instancia del InputController
        public InputController InputController {
            get{
                if(m_InputController == null)
                {
                    m_InputController = m_Instance.gameObject.GetComponent<InputController>();
                }
                return m_InputController;
            }
        }

        private AudioController m_AudioController;
        public AudioController AudioController {
            get {
                if(m_AudioController == null)
                {
                    m_AudioController = m_Instance.gameObject.GetComponent<AudioController>();
                    m_AudioController.BackgroundMusicSource = m_Instance.gameObject.AddComponent<AudioSource>();
                    m_AudioController.SoundEffectSource = m_Instance.gameObject.AddComponent<AudioSource>();
                }
                return m_AudioController;
            }
        }

        private BundleController m_BundleController;
        public BundleController BundleController
        {
            get {
                if(m_BundleController == null)
                {
                    m_BundleController = m_Instance.gameObject.GetComponent<BundleController>();
                }
                return m_BundleController;
            }
        }

        private PlayerBehaviour m_LocalPlayer; //Instancia del Player
        public PlayerBehaviour LocalPlayer {
            get {
                return m_LocalPlayer;
            }
            set {
                m_LocalPlayer = value;
            }
        }

        private CursorController m_CursorController;
        public CursorController CursorController
        {
            get {
                if(m_CursorController == null) m_CursorController = m_Instance.gameObject.GetComponent<CursorController>();
                return m_CursorController;
            }
        }

        private SceneController m_SceneController;
        public SceneController SceneController {
            get {
                return m_SceneController;
            }

            set {
                m_SceneController = value;
            }
        }
    }
}

