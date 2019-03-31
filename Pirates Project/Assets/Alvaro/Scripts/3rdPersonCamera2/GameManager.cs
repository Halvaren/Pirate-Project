﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefinitiveScript
{
    public class GameManager
    {
        private GameObject gameObject;

        private static GameManager m_Instance; //Instancia estática del GameManager. Es singleton
        public static GameManager Instance {
            get{
                if(m_Instance == null)
                {
                    m_Instance = new GameManager();
                    m_Instance.gameObject = new GameObject("_gameManager");
                    m_Instance.gameObject.AddComponent<InputController>();
                }

                return m_Instance;
            }
        }

        private InputController m_InputController; //Instancia del InputController
        public InputController InputController {
            get{
                if(m_InputController == null)
                {
                    m_InputController = gameObject.GetComponent<InputController>();
                }
                return m_InputController;
            }
        }

        private Player m_LocalPlayer; //Instancia del Player
        public Player LocalPlayer {
            get {
                return m_LocalPlayer;
            }
            set {
                m_LocalPlayer = value;
                if(m_Camera != null) m_Camera.InitializeCamera();
            }
        }

        private ThirdPersonCamera m_Camera; //Instancia de la Camera
        public ThirdPersonCamera Camera {
            get {
                return m_Camera;
            }
            set {
                m_Camera = value;
                if(m_LocalPlayer != null) m_Camera.InitializeCamera();
            }
        }
    }
}

