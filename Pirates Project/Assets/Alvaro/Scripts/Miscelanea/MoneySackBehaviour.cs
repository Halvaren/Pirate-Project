using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class MoneySackBehaviour : MonoBehaviour
{
    public int moneyAmount;

    private GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        parent = transform.parent.gameObject;
    }

    public void Destroy()
    {
        transform.parent = null;
        Destroy(parent);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            other.GetComponent<PlayerHealthController>().IncreaseMoney(moneyAmount);
            Destroy();
        }
    }
}
