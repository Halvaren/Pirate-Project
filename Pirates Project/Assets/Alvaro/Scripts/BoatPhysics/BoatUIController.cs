using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatUIController : MonoBehaviour
{
    public RectTransform rudderContainer;
    public RectTransform compassPointer;

    public float rudderMaxAngle = 90f;

    private float RudderRotation_Z;
    private float CompassRotation_Z;
    private Coroutine restartRudderCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        RudderRotation_Z = rudderContainer.localEulerAngles.z;
        CompassRotation_Z = compassPointer.localEulerAngles.z;
    }

    public void UpdateCompass(Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        print(direction + " " + (angle - 90f));

        CompassRotation_Z = angle - 90f;

        compassPointer.localEulerAngles = new Vector3(0f, 0f, CompassRotation_Z);
    }

    public void UpdateRudder(float rotationAmount)
    {
        if(restartRudderCoroutine != null) 
        {
            StopCoroutine(restartRudderCoroutine);
            restartRudderCoroutine = null;

            rudderContainer.localEulerAngles = new Vector3(0f, 0f, 0f);
        }

        RudderRotation_Z += rotationAmount;

        if(RudderRotation_Z > rudderMaxAngle) RudderRotation_Z = rudderMaxAngle;
        else if(RudderRotation_Z < -rudderMaxAngle) RudderRotation_Z = -rudderMaxAngle;

        rudderContainer.localEulerAngles = new Vector3(0f, 0f, RudderRotation_Z);
    }

    public void RestartUI()
    {
        RestartRudder();
    }

    public void RestartRudder()
    {
        restartRudderCoroutine = StartCoroutine(RestartRudderCoroutine(0.3f));
        
        RudderRotation_Z = 0f;
    }

    IEnumerator RestartRudderCoroutine(float time)
    {
        float initialRotation = RudderRotation_Z;
        float finalRotation = 0f;
        float newRotation = 0f;

        float elapsedTime = 0f;

        while(elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            newRotation = Mathf.Lerp(initialRotation, finalRotation, elapsedTime / time);
            rudderContainer.localEulerAngles = new Vector3(0f, 0f, newRotation);

            yield return null;
        }
        rudderContainer.localEulerAngles = new Vector3(0f, 0f, finalRotation);
    }
}
