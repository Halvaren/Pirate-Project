using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class TransitionDepur : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
            GameManager.Instance.SceneController.ChangeToScene(1);
    }
}
