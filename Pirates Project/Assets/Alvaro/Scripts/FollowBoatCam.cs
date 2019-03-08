using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBoatCam : MonoBehaviour
{
    public float DistanceFromBoat;
    public float yPositionAirCamera;
    public float RotationSpeed;
    public Transform BoatTransform;
    public float XAngle;

    private float yAngle;
    private bool airCamera;
    private bool transition;
    private float yPositionSeaCamera;

    // Start is called before the first frame update
    void Start()
    {
        yAngle = 0f;
        airCamera = false;
        transition = false;

        yPositionSeaCamera = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Z))
        {
            airCamera = !airCamera;
            transition = true;
            StartCoroutine(TransitionBetweenCameras(1.0f));
        }
        
        if(Input.GetMouseButton(0) && !airCamera)
        {
            yAngle += RotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if(!transition)
        {
            transform.rotation = Rotate();
            transform.position = Move();
        }
    }

    IEnumerator TransitionBetweenCameras(float time)
    {
        Vector3 initialPosition = transform.position;
        Quaternion initialRotation = transform.rotation;

        Vector3 finalPosition = Move();
        Quaternion finalRotation = Rotate();

        float elapsedTime = 0.0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            finalPosition = Move();
            finalRotation = Rotate();

            transform.position = Vector3.Lerp(initialPosition, finalPosition, elapsedTime / time);
            transform.rotation = Quaternion.Slerp(initialRotation, finalRotation, elapsedTime / time);

            yield return null;
        }
        transform.position = finalPosition;
        transform.rotation = finalRotation;
        transition = false;
    }

    Vector3 Move()
    {
        if(airCamera)
        {
            return new Vector3(BoatTransform.position.x, yPositionAirCamera, BoatTransform.position.z);
        }
        else
        {
            return new Vector3(BoatTransform.position.x - DistanceFromBoat * Mathf.Sin(yAngle), yPositionSeaCamera, BoatTransform.position.z - DistanceFromBoat * Mathf.Cos(yAngle));
        }
    }

    Quaternion Rotate()
    {
        if(airCamera)
        {
            return Quaternion.Euler(90f, 0f, 0f);
        }
        else
        {
            return Quaternion.Euler(XAngle, yAngle * Mathf.Rad2Deg, transform.rotation.z);
        }
    }
}
