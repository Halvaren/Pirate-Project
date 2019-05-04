using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLock : MonoBehaviour
{
    public List<GameObject> targets;
    public GameObject selectedTarget;
    public GameObject enemigo;
    private Transform myTransform;
    public Camera cam;
    public float maxZPos = 20;
    // Start is called before the first frame update
    void Start()
    {
        targets = new List<GameObject>();
        selectedTarget = null;
        myTransform = transform;
        AddAllEnemies();
    }

    public void AddAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach(GameObject enemy in enemies)
        {
            AddTarget(enemy);
        }
    }
    public void AddTarget(GameObject enemy)
    {
        targets.Add(enemy);
    }

    private void SortTargetsByDistance() //Ordena la lista para que el primer enemigo sea el más cerca
    {
        targets.Sort(delegate(GameObject t1, GameObject t2)
        {
            return Vector3.Distance(t1.GetComponent<Transform>().position, myTransform.position).CompareTo(Vector3.Distance(t2.GetComponent<Transform>().position, myTransform.position));
        });
    }
    private void TargetEnemy()
    {
        if (selectedTarget == null) //Si no ha sido seleccionado y está dentro de la visión de la cámara
        {
            SortTargetsByDistance();
            selectedTarget = targets[0];
            if (EnemyOnCameraView(selectedTarget.GetComponent<Transform>()))
            {
                SelectTarget();
            }
        }
        else
        {
            DeselectTarget();
        }
    }
    private void SelectTarget()
    {
        selectedTarget.GetComponent<Renderer>().material.color = Color.red;
    }
    private void DeselectTarget()
    {
        selectedTarget.GetComponent<Renderer>().material.color = Color.white;
        selectedTarget = null;
    }

    private bool IsSelected() //Cuando el puntero esté seleccionando al enemigo éste está seleccionado
    {
        if (selectedTarget.GetComponent<Renderer>().material.color == Color.red)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private bool EnemyOnCameraView(Transform selectedEnemy) //Devuelve true si el enemigo seleccionado está dentro de la cámara
    {
        Vector3 screenPos = cam.WorldToScreenPoint(selectedEnemy.position);
        if (0 < screenPos.x && screenPos.x < Screen.width && 0 < screenPos.y && screenPos.y < Screen.height && screenPos.z < maxZPos && !EnemyBehindObstacle())
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public bool EnemyBehindObstacle()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        int layerMask = 1<<8;
        layerMask = ~layerMask;
        if (Physics.Raycast(transform.position, fwd, maxZPos, layerMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    private void changeSelectedTarget(GameObject selectedEnemy, float direction)
    {
        int index = targets.IndexOf(selectedEnemy);
        GameObject newSelectedEnemy;
        if (direction > 0)
        {
            if (index < targets.Count - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }
            newSelectedEnemy = targets[index];
            if (selectedEnemy != null && EnemyOnCameraView(newSelectedEnemy.GetComponent<Transform>()))
            {
                DeselectTarget();
                selectedTarget = newSelectedEnemy;
                SelectTarget();
            }
        }
        else if (direction < 0)
        {
            if (index > 0)
            {
                index--;
            }
            else
            {
                index = targets.Count - 1;
            }
            newSelectedEnemy = targets[index];
            if (selectedEnemy != null && EnemyOnCameraView(newSelectedEnemy.GetComponent<Transform>()))
            {
                DeselectTarget();
                selectedTarget = newSelectedEnemy;
                SelectTarget();
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            TargetEnemy();
        }
        else if (Input.mouseScrollDelta.y > 0 || Input.mouseScrollDelta.y < 0)
        {
            Debug.Log("se cumple");
            changeSelectedTarget(selectedTarget, Input.mouseScrollDelta.y);
        }
    }
}
