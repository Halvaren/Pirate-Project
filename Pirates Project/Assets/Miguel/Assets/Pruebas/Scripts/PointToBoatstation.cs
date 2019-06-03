using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToBoatstation : MonoBehaviour
{
    public Transform target;
    public GameObject arrow;

    private Vector3 pos;
    // Start is called before the first frame update
    void Start()
    {
        pos = arrow.GetComponent<Transform>().position;
    }
    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target);
        pos.y = Vector3.Distance(target.position, transform.position)/5f;
        Debug.Log(Vector3.Distance(target.position, transform.position)/10f);
        arrow.GetComponent<Transform>().position = pos;
        if (Vector3.Distance(target.position, transform.position) < 100)
        {
            arrow.SetActive(false);
        }
        else
        {
            arrow.SetActive(true);
        }
    }
}
