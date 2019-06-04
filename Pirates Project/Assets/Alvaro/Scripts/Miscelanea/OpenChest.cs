using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;
using Cinemachine;

public class OpenChest : MonoBehaviour
{
    //public GameObject player; 
    // Start is called before the first frame update
    //public Camera mainCam;
    //public Camera rotatingCam;
    private bool nearChest;
    private bool spacePressed;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space)) spacePressed = true;
    }
    
    private void OnTriggerEnter (Collider other) {
        if ((other.gameObject.tag == "Player") && (spacePressed)/*&& (Input.GetKeyDown(KeyCode.Space))*/) {
            //nearChest = true; 
           //if() print("cosas");
             GameManager.Instance.LocalPlayer.finishedGame = true; 
              Debug.Log("ey");
            other.gameObject.active = false;
            GetComponent<Animator>().SetBool("isOpening", true);
            
           /* if (other.GetComponent<PlayerHealthController>().HasKey()) {
                GameManager.Instance.LocalPlayer.stopInput = true; 
        
            }
           */
        }

    }
}
