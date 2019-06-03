using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DefinitiveScript;

public class PuzleController : MonoBehaviour
{
    public Puzle[] puzles;

    private int resolvedPuzles;
    public int nPuzleObjective = 3;

    public GameObject rockInEntry;
    public BoxCollider entryTrigger;

    void Start() {

        if(GameManager.Instance.SceneController.GetResolvedPuzles()) OpenCavern();

        resolvedPuzles = 0;

        for(int i = 0; i < puzles.Length; i++)
        {
            puzles[i].PuzleController = this;
        }
    }

    void Update() {
        if(Input.GetKey(KeyCode.H) && Input.GetKeyDown(KeyCode.K))
        {
            resolvedPuzles = 2;
            PuzleResolved();
        }
    }

    public void PuzleResolved()
    {
        resolvedPuzles++;
        if(resolvedPuzles == nPuzleObjective)
        {
            OpenCavern();
            GameManager.Instance.SceneController.SetResolvedPuzles(true);
        }
    }

    private void OpenCavern()
    {
        print("Open cavern");
        rockInEntry.SetActive(false);
        entryTrigger.enabled = true;
    }
}
