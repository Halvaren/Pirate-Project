using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoatEngine : MonoBehaviour
{
    //Drags
    public Transform waterJetTransform;
    public Slider steerSlider; //Esto hay que modificarlo, obviamente

    //How fast should the engine accelerate?
    public float powerFactor;

    //What's the boat's maximum engine power?
    public float maxPower;

    public float steerVelocity = 2f;

    //The boat's current engine power is public for debugging
    public float currentJetPower;

    private float thrustFromWaterJet = 0f;

    private Rigidbody boatRB;

    private float WaterJetRotation_Y = 0f;

    BoatController boatController;

    // Start is called before the first frame update
    void Start()
    {
        boatRB = GetComponent<Rigidbody>();

        boatController = GetComponent<BoatController>();
    }

    // Update is called once per frame
    void Update()
    {
        UserInput();
    }

    void FixedUpdate()
    {
        UpdateWaterJet();
    }

    void UserInput()
    {
        //Forward / reverse
        if(Input.GetKey(KeyCode.W))
        {
            if(boatController.CurrentSpeed < 50f && currentJetPower < maxPower)
            {
                currentJetPower += 1f * powerFactor;
            }
        }
        else
        {
            currentJetPower = 0f;
        }

        //Steer left
        if(Input.GetKey(KeyCode.A))
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y + steerVelocity;

            if(WaterJetRotation_Y > 210f)
            {
                WaterJetRotation_Y = 210f;
            }

            steerSlider.value = WaterJetRotation_Y - 180f;

            Vector3 newRotation = new Vector3(0f, WaterJetRotation_Y, 0f);

            waterJetTransform.localEulerAngles = newRotation;
        }
        //Steer right
        else if(Input.GetKey(KeyCode.D))
        {
            WaterJetRotation_Y = waterJetTransform.localEulerAngles.y - steerVelocity;

            print(WaterJetRotation_Y);

            if(WaterJetRotation_Y < 150f)
            {
                WaterJetRotation_Y = 150f;
            }

            steerSlider.value = WaterJetRotation_Y - 180f;

            Vector3 newRotation = new Vector3(0f, WaterJetRotation_Y, 0f);

            waterJetTransform.localEulerAngles = newRotation;
        }
        else if(Input.GetKey(KeyCode.Space))
        {
            WaterJetRotation_Y = 180f;

            steerSlider.value = 0f;

            waterJetTransform.localEulerAngles = new Vector3(0f, WaterJetRotation_Y, 0f);
        }
    }

    void UpdateWaterJet()
    {
        //Debug.Log(boatController.CurrentSpeed);

        Vector3 forceToAdd = -waterJetTransform.forward * currentJetPower;

        Debug.DrawRay(waterJetTransform.position, waterJetTransform.forward * 3f, Color.magenta);

        //Only add the force if the is below sea level
        float waveYPos= WaterController.current.GetWaveYPos(waterJetTransform.position, Time.time);

        if(waterJetTransform.position.y < waveYPos)
        {
            boatRB.AddForceAtPosition(forceToAdd, waterJetTransform.position);
        }
        else
        {
            boatRB.AddForceAtPosition(Vector3.zero, waterJetTransform.position);
        }
    }
}
