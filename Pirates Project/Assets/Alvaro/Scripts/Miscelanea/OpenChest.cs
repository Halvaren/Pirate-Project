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
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnTriggerEnter (Collider other) {
        if (other.gameObject.tag == "Player") {
           
             GameManager.Instance.LocalPlayer.stopInput = true; 
              Debug.Log("ey");
            GetComponent<Animator>().SetBool("isOpening", true);
            
           /* if (other.GetComponent<PlayerHealthController>().HasKey()) {
                GameManager.Instance.LocalPlayer.stopInput = true; 
        
            }
           */
        }

    }
}
