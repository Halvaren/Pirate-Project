using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefinitiveScript;

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
            GameManager.Instance.SceneController.ChangeToScene("caverna");
        }
        else if (myCollision.gameObject.tag == "Player" && currentScene.name == "Island1" && this.gameObject.tag == "boatStation" )
        {
            GameManager.Instance.SceneController.ChangeToScene("BoatPhysics");
        }
        else if (myCollision.gameObject.tag == "Player" && currentScene.name == "caverna")
        {
            GameManager.Instance.SceneController.ChangeToScene("Island1");
        }
        else if (myCollision.gameObject.tag == "Boat" && currentScene.name == "BoatPhysics")
        {
            GameManager.Instance.SceneController.ChangeToScene("Island1");
        }
    }
}
