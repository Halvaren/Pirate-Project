using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatPhysics : MonoBehaviour
{

    //Drags
    public GameObject underWaterObj;

    //Script that's doing everything needed with the boat mesh, such as finding out which part is above the water
    private ModifyBoatMesh modifyBoatMesh;

    //Mesh for debugging
    private Mesh underWaterMesh;

    //The boats rigidbody
    private Rigidbody boatRB;

    //The density of the water the boat is travelling in
    [SerializeField] private float rhoWater = 1027f;

    // Start is called before the first frame update
    void Start()
    {
        boatRB = GetComponent<Rigidbody>();

        modifyBoatMesh = new ModifyBoatMesh(gameObject);

        underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;
    }

    // Update is called once per frame
    void Update()
    {
        modifyBoatMesh.GenerateUnderwaterMesh();

        //modifyBoatMesh.DisplayMesh(underWaterMesh, "UnderWater Mesh", modifyBoatMesh.underWaterTriangleData);
    }

    void FixedUpdate()
    {
        if(modifyBoatMesh.underWaterTriangleData.Count > 0)
        {
            AddUnderWaterForces();
        }
    }

    void AddUnderWaterForces()
    {
        List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;

        for(int i = 0; i < underWaterTriangleData.Count; i++)
        {
            TriangleData triangleData = underWaterTriangleData[i];

            Vector3 buoyancyForce = BuoyancyForce(rhoWater, triangleData);

            boatRB.AddForceAtPosition(buoyancyForce, triangleData.center);

            //Debug

            //Normal
            Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

            //Buoyancy
            Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);
        }
    }

    //The buoyancy force so the boat can float
    private Vector3 BuoyancyForce(float rho, TriangleData triangleData)
    {
        //Buoyancy is a hydrostatic foce - it's there even if the water isn't flowing or if the boat stays still

        //F_buoyancy = rho * g * V
        //rho - density of the mediaum you are in
        //g - gravity
        //V - volume of the fluid directly above the curved surface

        //V = z * S * n
        //z - distance to surface
        //S - surface area
        //n - normal to the surface

        Vector3 buoyancyForce = rho * Physics.gravity.y * triangleData.distanceToSurface * triangleData.area * triangleData.normal;

        //The vertical component of the hydrostatic forces don't cancel out but the horizontal do
        buoyancyForce.x = 0f;
        buoyancyForce.z = 0f;

        return buoyancyForce;
    }
}


