using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class EnemyLootController : MonoBehaviour
{
    public const float smallSackProbability = 0.5f;
    public const float mediumSackProbability = 0.85f;
    public const float bigSackProbablity = 1f;

    public const float healthPackageProbability = 0.4f;

    public GameObject smallSackObj;
    public GameObject mediumSackObj;
    public GameObject bigSackObj;

    public GameObject healthPackageObj;
    public GameObject keyObj;

    public ParticleSystem hasKeyParticleSystem;

    private bool hasKey;

    public GameObject releasePoint;
    public float releaseForce = 75f;

    // Start is called before the first frame update
    void Start()
    {
        hasKey = false;
    }

    public void SetHasKey(bool value)
    {
        hasKey = value;
        if(value) hasKeyParticleSystem.Play();
    }

    public void ReleaseLoot()
    {
        float rand = Random.value;
        GameObject releasedObj;

        if(rand < smallSackProbability)
        {
            releasedObj = Instantiate(smallSackObj, releasePoint.transform.position, Quaternion.identity);
        }
        else if(rand < mediumSackProbability)
        {
            releasedObj = Instantiate(mediumSackObj, releasePoint.transform.position, Quaternion.identity);
        }
        else
        {
            releasedObj = Instantiate(bigSackObj, releasePoint.transform.position, Quaternion.identity);
        }

        float x = (Random.value * 2) - 1;
        float z = Mathf.Sqrt(1 - Mathf.Pow(x, 2)) * Mathf.Pow(-1, Random.Range(0, 1));

        Vector3 force = new Vector3(x, 2f, z) * releaseForce;
        releasedObj.GetComponent<Rigidbody>().AddForce(force);

        rand = Random.value;
        if(rand < healthPackageProbability)
        {
            releasedObj = Instantiate(healthPackageObj, releasePoint.transform.position, Quaternion.identity);

            x = (Random.value * 2) - 1;
            z = Mathf.Sqrt(1 - Mathf.Pow(x, 2)) * Mathf.Pow(-1, Random.Range(0, 1));

            force = new Vector3(x, 2f, z) * releaseForce;
            releasedObj.GetComponent<Rigidbody>().AddForce(force);
        }

        if(hasKey)
        {
            releasedObj = Instantiate(keyObj, releasePoint.transform.position, Quaternion.identity);

            x = (Random.value * 2) - 1;
            z = Mathf.Sqrt(1 - Mathf.Pow(x, 2)) * Mathf.Pow(-1, Random.Range(0, 1));

            force = new Vector3(x, 2f, z) * releaseForce;
            releasedObj.GetComponent<Rigidbody>().AddForce(force);
        }
    }
}
