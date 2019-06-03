using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerCameraController : MonoBehaviour
{
    public CinemachineFreeLook playerThirdPersonCamera;

    private bool stopInput;

    public float thirdPersonCameraXAxisMaxSpeed = 300f;
    public float thirdPersonCameraYAxisMaxSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InputChanged(bool value)
    {
        stopInput = value;
        if(stopInput)
        {
            playerThirdPersonCamera.m_XAxis.m_MaxSpeed = 0f;
            playerThirdPersonCamera.m_YAxis.m_MaxSpeed = 0f;
        }
        else
        {
            playerThirdPersonCamera.m_XAxis.m_MaxSpeed = thirdPersonCameraXAxisMaxSpeed;
            playerThirdPersonCamera.m_YAxis.m_MaxSpeed = thirdPersonCameraYAxisMaxSpeed;
        }
    }
}
