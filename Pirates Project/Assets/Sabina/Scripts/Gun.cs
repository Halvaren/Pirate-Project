
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 50f; 

    //public GameObject player; 
    public ParticleSystem gunParticles; 

    public GameObject impactEffect;
    public Camera cam; 

    public LineRenderer gunLine;

    private Ray raymouse;

     void Awake ()
    {
        //shootableMask = LayerMask.GetMask ("Shootable");
       //gunParticles = GetComponent<ParticleSystem> ();
        gunLine = GetComponent <LineRenderer> ();
        //player = GameObject.FindWithTag("Player");
     
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Fire1")){ 
            Shoot();
        }
    }

    void Shoot() 
    {
       
      
        //raymouse = cam.ScreenPointToRay(mousePos);
       // gunParticles.Play();
        //gunLine.enabled = true;
        //gunLine.SetPosition (0, transform.position);
         Debug.DrawRay(transform.position, transform.forward*10, Color.green);
        RaycastHit hit; 
        if (Physics.Raycast(transform.position,transform.forward, out hit, range)) {
            Debug.Log(hit.transform.name);
            
            //Instantiate(impactEffect.Play(), hit.point, Quaternion.LookRotation(hit.normal));
           
        }
        //Instantiate(impactEffect, hit.point, Quaternion.LookRotation(hit.normal));
    }
}