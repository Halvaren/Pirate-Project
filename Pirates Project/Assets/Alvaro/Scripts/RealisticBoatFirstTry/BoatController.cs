using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatController0 : MonoBehaviour
{
    public Vector3 COM;
    public float speed = 1.0f;
    public float steerSpeed = 1.0f;
    public float movementThreshold = 10.0f;

    private Transform m_COM;
    private float verticalInput;
    private float horizontalInput;
    private float movementFactor;
    private float steerFactor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Balance();
        Movement();
        Steer();

        if(Input.GetKeyDown(KeyCode.Space))
        {
            movementFactor = steerFactor = 0.0f;
        }
    }

    void Balance()
    {
        if(!m_COM)
        {
            m_COM = new GameObject("COM").transform;
            m_COM.SetParent(transform);
        }
        m_COM.position = COM;
        GetComponent<Rigidbody>().centerOfMass = m_COM.position;
    }

    private void Movement()
    {
        verticalInput = Input.GetAxis("Vertical");
        movementFactor = Mathf.Lerp(movementFactor, verticalInput, Time.deltaTime / movementThreshold);
        transform.Translate(0.0f, 0.0f, movementFactor * speed);
    }

    private void Steer()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        steerFactor = Mathf.Lerp(steerFactor, horizontalInput * verticalInput, Time.deltaTime / movementThreshold);
        transform.Rotate(0.0f, steerFactor * steerSpeed, 0.0f);
    }
}
