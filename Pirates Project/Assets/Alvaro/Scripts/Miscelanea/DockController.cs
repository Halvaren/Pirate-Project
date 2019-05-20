using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class DockController : MonoBehaviour
{
    public bool enteringIsland;

    public int dockID;
    public Transform boatSpawnPoint;
    public Transform playerSpawnPoint;

    private SceneController m_SceneController;
    public SceneController SceneController {
        get {
            if(m_SceneController == null) m_SceneController = GameManager.Instance.SceneController;
            return m_SceneController;
        }
    }

    private void OnTriggerEnter(Collider myCollision)
    {
        if(myCollision.gameObject.tag == "Boat" && enteringIsland)
        {
            SceneController.DockTheBoat(dockID);
        }
        else if(myCollision.gameObject.tag == "Player" && !enteringIsland)
        {
            SceneController.ToSail(dockID);
        }
    }
}
