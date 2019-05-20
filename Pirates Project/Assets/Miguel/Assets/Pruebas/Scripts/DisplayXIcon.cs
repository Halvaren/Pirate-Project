using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayXIcon : MonoBehaviour
{
    private SpriteRenderer rend;
    public Color visibleColor;
    public Transform playerMapIcon;
    public Transform panelMapIcon;
    public float distance = 3f;

    void Start()
    {
        rend = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if(Vector2.Distance(panelMapIcon.position, playerMapIcon.position) < distance)
        {
            Debug.Log("es visible");
            rend.color = visibleColor;
        }
    }
}
