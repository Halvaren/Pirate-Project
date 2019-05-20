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


            if(SceneManager.GetActiveScene().buildIndex == boatSceneID)
            {
                FindBoatInitialPoint();
                FindBoat();
                FindBoatDocks();
            }
            else if(SceneManager.GetActiveScene().buildIndex == islandSceneID)
            {
                FindPlayer();
                FindIslandDocks();
                FindExitCavernSpawnPoint();
            }
            else if(SceneManager.GetActiveScene().buildIndex == cavernSceneID)
            {
                FindPlayer();
                FindEnterCavernSpawnPoint();
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
            BoatDocks = FindObjectsOfType<DockController>();
        }

        private void FindIslandDocks()
        {
            IslandDocks = FindObjectsOfType<DockController>();
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

        public void BackToMenu()
        {
            StartCoroutine(BackToMenuCoroutine());
        }

        private IEnumerator BackToMenuCoroutine()
        {
            yield return StartCoroutine(FadeOut(fadingTime));
            SceneManager.LoadScene(mainMenuID);
            yield return StartCoroutine(FadeIn(fadingTime));
        }

        public void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            yield return StartCoroutine(FadeOut(fadingTime));
            SceneManager.LoadScene(islandSceneID);

            if(BoatTransform == null) FindBoat();
            if(boatInitialPoint == null) FindBoatInitialPoint();
            BoatTransform.position = boatInitialPoint.position;

            if(BoatDocks == null) FindBoatDocks();

            yield return StartCoroutine(FadeIn(fadingTime));
        }

        public void DockTheBoat(int dockID)
        {
            StartCoroutine(DockTheBoatCoroutine(dockID));
        }

        private IEnumerator DockTheBoatCoroutine(int dockID)
        {
            yield return StartCoroutine(FadeOut(fadingTime));
            SceneManager.LoadScene(islandSceneID);

            if(PlayerBehaviour == null) FindPlayer();
            PlayerBehaviour.stopInput = true;

            if(IslandDocks == null) FindIslandDocks();
            PlayerBehaviour.transform.position = IslandDocks[dockID].playerSpawnPoint.position;

            if(exitFromCavernSpawnPoint == null) FindExitCavernSpawnPoint();

            yield return StartCoroutine(FadeIn(fadingTime));

            PlayerBehaviour.stopInput = false;
        }

        public void ToSail(int dockID)
        {
            StartCoroutine(ToSailCoroutine(dockID));
        }

        private IEnumerator ToSailCoroutine(int dockID)
        {
            yield return StartCoroutine(FadeOut(fadingTime));
            SceneManager.LoadScene(islandSceneID);

            if(BoatTransform == null) FindBoat();

            if(BoatDocks == null) FindBoatDocks();
            BoatTransform.position = IslandDocks[dockID].boatSpawnPoint.position;

            yield return StartCoroutine(FadeIn(fadingTime));
        }

        public void EnterIntoTheCavern()
        {
            StartCoroutine(EnterIntoTheCavernCoroutine());
        }

        private IEnumerator EnterIntoTheCavernCoroutine()
        {
            yield return StartCoroutine(FadeOut(fadingTime));
            SceneManager.LoadScene(cavernSceneID);

            if(PlayerBehaviour == null) FindPlayer(); 
            PlayerBehaviour.stopInput = true;

            if(enterIntoCavernSpawnPoint == null) FindEnterCavernSpawnPoint();
            PlayerBehaviour.transform.position = enterIntoCavernSpawnPoint.position;

            yield return StartCoroutine(FadeIn(fadingTime));

            PlayerBehaviour.stopInput = false;
        }

        public void ExitFromTheCavern()
        {
            StartCoroutine(ExitFromTheCavernCoroutine());
        }

        private IEnumerator ExitFromTheCavernCoroutine()
        {
            yield return StartCoroutine(FadeOut(fadingTime));
            SceneManager.LoadScene(islandSceneID);

            if(PlayerBehaviour == null) FindPlayer(); 
            PlayerBehaviour.stopInput = true;

            if(exitFromCavernSpawnPoint == null) FindExitCavernSpawnPoint();
            PlayerBehaviour.transform.position = exitFromCavernSpawnPoint.position;

            if(IslandDocks == null) FindIslandDocks();

            yield return StartCoroutine(FadeIn(fadingTime));

            PlayerBehaviour.stopInput = false;
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
                elapsedTime = 0.0f;

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
                elapsedTime = 0.0f;

                c.a = Mathf.Lerp(initialAlpha, finalAlpha, elapsedTime / time);
                blackScreen.color = c;

                yield return null;
            }
            c.a = finalAlpha;
            blackScreen.color = c;
        }
    }
}