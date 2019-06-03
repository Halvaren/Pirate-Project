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

    public GameObject releasePoint;
    public float releaseForce = 75f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReleaseLoot()
    {
        float rand = Random.value;
        GameObject releasedSack;

        if(rand < smallSackProbability)
        {
            releasedSack = Instantiate(smallSackObj, releasePoint.transform.position, Quaternion.identity);
        }
        else if(rand < mediumSackProbability)
        {
            releasedSack = Instantiate(mediumSackObj, releasePoint.transform.position, Quaternion.identity);
        }
        else
        {
            releasedSack = Instantiate(bigSackObj, releasePoint.transform.position, Quaternion.identity);
        }

        float x = Random.value;
        float z = Mathf.Sqrt(1 - Mathf.Pow(x, 2));

        Vector3 force = new Vector3(x, 2f, z) * releaseForce;
        releasedSack.GetComponent<Rigidbody>().AddForce(force);
    }
}
