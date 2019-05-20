using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using DefinitiveScript;
public class MapController : MonoBehaviour
{
    private Camera cam;
    public GameObject vcam;
    public Transform playerPos;
    private Vector3 desiredPos;
    public GameObject map;
    public GameObject playerIcon;
    private bool inMap;
    private float orthographicSize;
    private int priority = -1;

    void Start()
    {
        cam = Camera.main;
        playerIcon.SetActive(false);
        map.SetActive(false);
        desiredPos = playerIcon.GetComponent<Transform>().position;
        orthographicSize = vcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            GameManager.Instance.LocalPlayer.stopInput = !GameManager.Instance.LocalPlayer.stopInput;
            vcam.GetComponent<CinemachineVirtualCamera>().Priority = vcam.GetComponent<CinemachineVirtualCamera>().Priority * priority;
            if (cam.orthographic)
            {
                playerIcon.SetActive(false);
                map.SetActive(false);
                cam.orthographic = false;
                inMap = false;
            }
            else
            {
                playerIcon.SetActive(true);
                map.SetActive(true);
                cam.orthographic = true;
                inMap = true;
            }
        }
        if (inMap)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f ) // forward
            {
                orthographicSize = orthographicSize - 10;
                if(orthographicSize < 50)
                {
                    orthographicSize = 50; // Min size 
                }
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0f ) // backwards
            {
                orthographicSize = orthographicSize + 10;
                if(orthographicSize > 100)
                {
                    orthographicSize = 100; // Max size 
                }
            }
            vcam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = orthographicSize;
        }
        desiredPos.x = playerPos.position.x / 4.7673f;
        desiredPos.z = playerPos.position.z / 4.7673f;
        playerIcon.GetComponent<Transform>().position = desiredPos;
    }
}
