using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transitions : MonoBehaviour
{
    private Scene currentScene;
    void Start()
    {
        currentScene = SceneManager.GetActiveScene();
    }
    private void OnTriggerEnter(Collider myCollision)
    {
        if (myCollision.gameObject.tag == "Player" && currentScene.name == "Island1" && this.gameObject.name == "cavernEntrance")
        {
            StartCoroutine(LoadCavern());
        }
        else if (myCollision.gameObject.tag == "Player" && currentScene.name == "Island1" && this.gameObject.name == "boatStation" )
        {
            StartCoroutine(LoadBoat());
        }
        else if (myCollision.gameObject.tag == "Player" && currentScene.name == "caverna")
        {
            StartCoroutine(LoadIsland());
        }
        else if (myCollision.gameObject.tag == "Boat" && currentScene.name == "BoatPhysics")
        {
            StartCoroutine(LoadIsland());
        }
    }

    IEnumerator LoadCavern()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("caverna", LoadSceneMode.Additive);

        while (!load.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(currentScene);
    }
    IEnumerator LoadBoat()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("BoatPhysics", LoadSceneMode.Additive);

        while (!load.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(currentScene);
    }
    IEnumerator LoadIsland()
    {
        AsyncOperation load = SceneManager.LoadSceneAsync("Island1", LoadSceneMode.Additive);

        while (!load.isDone)
        {
            yield return null;
        }

        SceneManager.UnloadSceneAsync(currentScene);
    }
}
