using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayIcon : MonoBehaviour
{
    private SpriteRenderer rend;
    public Color visibleColor;
    public Transform playerMapIcon;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(Vector2.Distance(transform.position, playerMapIcon.position) < 3f)
        {
            Debug.Log("es visible");
            rend.color = visibleColor;
        }
    }
}
