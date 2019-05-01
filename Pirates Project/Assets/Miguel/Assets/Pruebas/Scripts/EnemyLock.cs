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
    private Plane[] planes;
    // Start is called before the first frame update
    void Start()
    {
        targets = new List<GameObject>();
        selectedTarget = null;
        myTransform = transform;
        planes = GeometryUtility.CalculateFrustumPlanes(cam);
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
        SortTargetsByDistance();
        selectedTarget = targets[0];
        if (!IsSelected()) //Si no ha sido seleccionado y está dentro de la visión de la cámara
        {
            SelectTarget();
        }
        else
        {
            DeselectTarget();
        }
        /*else
        {
            int index = targets.IndexOf(selectedTarget);
            if (index < targets.Count - 1) //Si es menor que el total de enmigos
            {
                index++;
            }
            else
            {
                index = 0;
            }
            DeselectTarget();
            selectedTarget = targets[index];
        }*/
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
    private bool EnemyOnCameraView()
    {
        if (GeometryUtility.TestPlanesAABB(planes, enemigo.GetComponent<Collider>().bounds))
        {
            Debug.Log("lo veo");
            return true;
        }
        else
        {
            Debug.Log("no lo veo");
            return false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse2))
        {
            TargetEnemy();
        }
        EnemyOnCameraView();
        //transform.LookAt(selectedTarget);
    }
}
