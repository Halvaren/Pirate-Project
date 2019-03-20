using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public float movingSpeed = 120.0f;
    public float clampAngle = 80.0f;
    public float inputSensitivty = 150.0f;

    public GameObject cameraFollowObj;
    public GameObject cameraObj;
    public GameObject playerObj;

    public float camDistanceXToPlayer;
    public float camDistanceYToPlayer;
    public float camDistanceZToPlayer;

    public float mouseX;
    public float mouseY;
    public float finalInputX;
    public float finalInputZ;

    public float smoothX;
    public float smoothY;

    private Vector3 followPosition;
    private float rotX = 0.0f;
    private float rotY = 0.0f;

    public Transform naturalPosition;
    public Transform aimingPosition;
    public float aimingSpeed = 10f;
    private bool aiming;

    // Start is called before the first frame update
    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        rotY = rot.y;
        rotX = rot.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!aiming) RotateCamera(); //Gira la cámara en función del input
        else AimCamera();
    }

    void LateUpdate()
    {
        if(!aiming) MoveCamera(); //Mueve la cámara siguiendo al target
    }

    void RotateCamera()
    {
        float inputX = Input.GetAxis("RightStickHorizontal"); //Input para giro horizontal con joystick
        //float inputZ = Input.GetAxis("RightStickVertical"); //Input para giro vertical con joystick

        mouseX = Input.GetAxis("Mouse X"); //Input para giro horizontal con ratón
        //mouseY = Input.GetAxis("Mouse Y"); //Input para giro vertical con ratón

        finalInputX = inputX + mouseX; //Input final para giro horizontal
        //finalInputZ = inputZ + mouseY; //Input final para giro vertical

        rotY += finalInputX * inputSensitivty * Time.deltaTime; //Aplicación del input a la rotación en el eje Y (giro horizontal)
        rotX += finalInputZ * inputSensitivty * Time.deltaTime; //Aplicación del input a la rotación en el eje X (giro vertical)

        rotX = Mathf.Clamp(rotX, -clampAngle, clampAngle); //Limitación del giro vertical

        Quaternion localRotation = Quaternion.Euler(rotX, rotY, 0.0f);
        transform.rotation = localRotation;
    }
    
    void AimCamera()
    {
        transform.position = Vector3.MoveTowards(transform.position, aimingPosition.position, aimingSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, cameraFollowObj.transform.rotation, aimingSpeed * Time.deltaTime);
    }

    void MoveCamera()
    {
        Transform target = cameraFollowObj.transform;

        float step = movingSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, target.position, step);
    }

    public void SetAiming(bool param)
    {
        aiming = param;
    }
}
