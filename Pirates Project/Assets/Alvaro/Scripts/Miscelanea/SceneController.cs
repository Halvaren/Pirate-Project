using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace DefinitiveScript 
{
    public class SceneController : MonoBehaviour
    {
        private bool created;
        private PlayerBehaviour PlayerBehaviour;
        private Transform BoatTransform;
        private GameObject GM;

        public Image blackScreen;
        public float fadingTime = 0.5f;

        private int lastScene;
        private int dockID;
        
        private bool changedScene = false;

        private DockController[] BoatDocks;
        private DockController[] IslandDocks;

        private Transform boatInitialPoint;
        private Transform exitFromCavernSpawnPoint;
        private Transform enterIntoCavernSpawnPoint;

        private int mainMenuID = 0;
        private int boatSceneID = 1;
        private int islandSceneID = 2;
        private int cavernSceneID = 3;

        void Awake()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("SceneController");

            if(objs.Length > 1 && !created)
            {
                Destroy(this.gameObject);
            }

            if(!created) lastScene = -1;

            DontDestroyOnLoad(GameManager.Instance.GameObject);
            DontDestroyOnLoad(this.gameObject);

            created = true;
        }

        void Start()
        {
            GameManager.Instance.SceneController = this;

            InitializeScene();
        }

        void Update() {
            if(changedScene && lastScene != SceneManager.GetActiveScene().buildIndex)
            {
                InitializeScene();
                changedScene = false;
            }
        }

        private void FindPlayer()
        {
            GameObject aux = GameObject.Find("Player");
            if(aux != null)
            {
                PlayerBehaviour = aux.GetComponent<PlayerBehaviour>();

                GameManager.Instance.LocalPlayer = PlayerBehaviour;
                PlayerBehaviour.enabled = true;

                GameManager.Instance.CursorController.LockCursor();
            }
        }

        private void FindBoat()
        {
            GameObject aux = GameObject.Find("Boat");
            if(aux != null) BoatTransform = aux.transform;
        }

        private void FindBoatInitialPoint()
        {
            GameObject aux = GameObject.Find("BoatInitialPoint");
            if(aux != null) boatInitialPoint = aux.transform;
        }

        private void FindBoatDocks()
        {
            DockController[] aux = FindObjectsOfType<DockController>();
            
            BoatDocks = new DockController[aux.Length/2];

            int j = 0;
            for(int i = 0; i < aux.Length; i++)
            {
                if(aux[i].enteringIsland) 
                {
                    BoatDocks[j] = aux[i];
                    j++;
                }
            }
        }

        private void FindIslandDocks()
        {
            DockController[] aux = FindObjectsOfType<DockController>();
            
            IslandDocks = new DockController[aux.Length/2];

            int j = 0;
            for(int i = 0; i < aux.Length; i++)
            {
                if(!aux[i].enteringIsland) 
                {
                    IslandDocks[j] = aux[i];
                    j++;
                }
            }
        }

        private void FindExitCavernSpawnPoint()
        {
            GameObject aux = GameObject.Find("ExitFromCavernSpawnPoint");
            if(aux != null) exitFromCavernSpawnPoint = aux.transform;
        }

        private void FindEnterCavernSpawnPoint()
        {
            GameObject aux = GameObject.Find("EnterIntoCavernSpawnPoint");
            if(aux != null) enterIntoCavernSpawnPoint = aux.transform;
        }

        private void InitializeScene()
        {
            StartCoroutine(InitializeSceneCoroutine());
        }

        private IEnumerator InitializeSceneCoroutine()
        {
            if(lastScene != -1)
            {
                if(SceneManager.GetActiveScene().buildIndex == boatSceneID)
                {
                    FindBoatInitialPoint();
                    FindBoat();
                    FindBoatDocks();

                    if(lastScene == mainMenuID)
                    {
                        BoatTransform.position = boatInitialPoint.position;
                    }
                    else if(lastScene == islandSceneID)
                    {
                        BoatTransform.position = IslandDocks[dockID].boatSpawnPoint.position;
                    }
                    
                    yield return StartCoroutine(FadeIn(fadingTime));
                }
                else if(SceneManager.GetActiveScene().buildIndex == islandSceneID)
                {
                    FindPlayer();
                    FindIslandDocks();
                    FindExitCavernSpawnPoint();

                    PlayerBehaviour.stopInput = true;
                    if(lastScene == boatSceneID)
                    {
                        PlayerBehaviour.transform.position = IslandDocks[dockID].playerSpawnPoint.position;
                    }
                    else if(lastScene == cavernSceneID)
                    {
                        PlayerBehaviour.transform.position = exitFromCavernSpawnPoint.position;
                    }

                    yield return StartCoroutine(FadeIn(fadingTime));

                    PlayerBehaviour.stopInput = false;
                }
                else if(SceneManager.GetActiveScene().buildIndex == cavernSceneID)
                {
                    FindPlayer();
                    FindEnterCavernSpawnPoint();

                    if(lastScene == islandSceneID)
                    {   
                        PlayerBehaviour.stopInput = true;

                        PlayerBehaviour.transform.position = enterIntoCavernSpawnPoint.position;

                        yield return StartCoroutine(FadeIn(fadingTime));

                        PlayerBehaviour.stopInput = false;
                    }
                }
            }
            
        }

        private IEnumerator ChangeToScene(int sceneID)
        {
            lastScene = SceneManager.GetActiveScene().buildIndex;
            changedScene = true;
            yield return StartCoroutine(FadeOut(fadingTime));

            SceneManager.LoadScene(sceneID);
        }

        public void BackToMenu()
        {
            StartCoroutine(ChangeToScene(mainMenuID));
        }

        public void StartGame()
        {
            StartCoroutine(ChangeToScene(boatSceneID));
        }

        public void DockTheBoat(int dockID)
        {
            this.dockID = dockID;

            StartCoroutine(ChangeToScene(islandSceneID));
        }

        public void ToSail(int dockID)
        {
            this.dockID = dockID;

            StartCoroutine(ChangeToScene(boatSceneID));
        }

        public void EnterIntoTheCavern()
        {
            StartCoroutine(ChangeToScene(cavernSceneID));
        }

        public void ExitFromTheCavern()
        {
            StartCoroutine(ChangeToScene(islandSceneID));
        }

        /*public void ChangeToScene(string name)
        {
            StartCoroutine(TransitionToScene(fadingTime, name));
        }

        public void ChangeToScene(int id)
        {
            StartCoroutine(TransitionToScene(fadingTime, id));
        }

        private IEnumerator TransitionToScene(float time, string name)
        {
            yield return StartCourutine(FadeOut(time));
            SceneManager.LoadScene(name);
            yield return StartCoroutine(FadeIn(time));
        }

        private IEnumerator TransitionToScene(float time, int id)
        {
            yield return StartCourutine(FadeOut(time));
            SceneManager.LoadScene(id);
            yield return StartCoroutine(FadeIn(time));
        }*/

        private IEnumerator FadeOut(float time)
        {
            float initialAlpha = 0f;
            float finalAlpha = 1f;

            float elapsedTime = 0.0f;

            Color c = blackScreen.color;
            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
                blackScreen.color = c;

                yield return null;
            }
            c.a = finalAlpha;
            blackScreen.color = c;
        }

        private IEnumerator FadeIn(float time)
        {
            float initialAlpha = 1f;
            float finalAlpha = 0f;

            float elapsedTime = 0.0f;

            Color c = blackScreen.color;
            while(elapsedTime < time)
            {
                elapsedTime += Time.deltaTime;

                c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
                blackScreen.color = c;

                yield return null;
            }

            c.a = finalAlpha;
            blackScreen.color = c;
        }
    }
}