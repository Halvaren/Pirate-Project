using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class CavernEntry: MonoBehaviour
{
    public bool enteringCavern;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(enteringCavern) GameManager.Instance.SceneController.EnterIntoTheCavern();
            else GameManager.Instance.SceneController.ExitFromTheCavern();
        }
    }
}
