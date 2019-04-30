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
        if (myCollision.gameObject.tag == "Player" && currentScene.name == "Island1" && this.gameObject.tag == "cavernEntrance")
        {
            SceneManager.LoadScene("caverna", LoadSceneMode.Single);
        }
        else if (myCollision.gameObject.tag == "Player" && currentScene.name == "Island1" && this.gameObject.tag == "boatStation" )
        {
            SceneManager.LoadScene("BoatPhysics", LoadSceneMode.Single);
        }
        else if (myCollision.gameObject.tag == "Player" && currentScene.name == "caverna")
        {
            SceneManager.LoadScene("Island1", LoadSceneMode.Single);
        }
        else if (myCollision.gameObject.tag == "Boat" && currentScene.name == "BoatPhysics")
        {
            SceneManager.LoadScene("Island1", LoadSceneMode.Single);
        }
    }
}
