using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowBoatCam : MonoBehaviour
{
    public float DistanceFromBoat;
    public float RotationSpeed;
    public Transform BoatTransform;
    public float XAngle;

    private float yAngle;

    // Start is called before the first frame update
    void Start()
    {
        yAngle = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            yAngle += RotationSpeed * Input.GetAxis("Mouse X") * Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(XAngle, yAngle * Mathf.Rad2Deg, transform.rotation.z);
        transform.position = new Vector3(BoatTransform.position.x - DistanceFromBoat * Mathf.Sin(yAngle), transform.position.y, BoatTransform.position.z - DistanceFromBoat * Mathf.Cos(yAngle));
    }
}
