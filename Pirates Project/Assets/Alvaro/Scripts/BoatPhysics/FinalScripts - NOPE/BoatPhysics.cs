using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NopeScript
{
    public class BoatPhysics : MonoBehaviour
    {

        //Drags
        public GameObject boatMeshObj;
        public GameObject aboveWaterObj;
        public GameObject underWaterObj;
        public GameObject underWaterMirrorObj;
        public GameObject foamSkirtObj;

        //Change the center of mass
        public Vector3 centerOfMass;

        private List<Vector3> intersectionVertices = new List<Vector3>();

        //Script that's doing everything needed with the boat mesh, such as finding out which part is above the water
        private ModifyBoatMesh modifyBoatMesh;

        private GenerateExtraBoatMeshes generateExtraMeshes;

        //Meshes for debugging
        private Mesh underWaterMesh;
        private Mesh aboveWaterMesh;
        private Mesh underWaterMirrorMesh;
        private Mesh foamMesh;

        //The boats rigidbody
        private Rigidbody boatRB;

        //The density of the water the boat is travelling in
        private float rhoWater = BoatPhysicsMath.RHO_OCEAN_WATER;
        private float rhoAir = BoatPhysicsMath.RHO_AIR;

        void Awake()
        {
            boatRB = GetComponent<Rigidbody>();
        }

        // Start is called before the first frame update
        void Start()
        {
            modifyBoatMesh = new ModifyBoatMesh(boatMeshObj, underWaterObj, aboveWaterObj, boatRB);
            //generateExtraMeshes = new GenerateExtraBoatMeshes(boatMeshObj);
            underWaterMesh = underWaterObj.GetComponent<MeshFilter>().mesh;
            aboveWaterMesh = aboveWaterObj.GetComponent<MeshFilter>().mesh;
            //underWaterMirrorMesh = underWaterMirrorObj.GetComponent<MeshFilter>().mesh;
        }

        // Update is called once per frame
        void Update()
        {
            modifyBoatMesh.GenerateUnderwaterMesh(intersectionVertices);

            //generateExtraMeshes.DisplayMesh(underWaterMesh, "UnderWater Mesh", modifyBoatMesh.underWaterTriangleData);
            //generateExtraMeshes.DisplayMesh(aboveWaterMesh, "AboveWater Mesh", modifyBoatMesh.aboveWaterTriangleData);
            //generateExtraMeshes.DisplayMirrorMesh(underWaterMirrorMesh, "UnderWater Mirror Mesh", modifyBoatMesh.aboveWaterTriangleData);
            //generateExtraMeshes.GenerateFoamSkirt(foamMesh, "Foam skirt", intersectionVertices);
        }

        void FixedUpdate()
        {
            boatRB.centerOfMass = centerOfMass;

            if(modifyBoatMesh.underWaterTriangleData.Count > 0)
            {
                AddUnderWaterForces();
            }
            if(modifyBoatMesh.aboveWaterTriangleData.Count <= 0) return;

            AddAboveWaterForces();
        }

        void AddUnderWaterForces()
        {
            float Cf = BoatPhysicsMath.ResistanceCoefficient(rhoWater, boatRB.velocity.magnitude, modifyBoatMesh.CalculateUnderWaterLength());

            //To calculate the slamming force we need the velocity at each of the original triangles
            List<SlammingForceData> slammingForceData = modifyBoatMesh.slammingForceData;

            CalculateSlammingVelocities(slammingForceData);

            //Need this data for slamming forces
            float boatArea = modifyBoatMesh.boatArea;
            float boatMass = 600000f; //boatRB.mass;

            //To connect the submerged triangles with the original triangles
            List<int> indexOfOriginalTriangle = modifyBoatMesh.indexOfOriginalTriangle;

            List<TriangleData> underWaterTriangleData = modifyBoatMesh.underWaterTriangleData;

            for(int i = 0; i < underWaterTriangleData.Count; i++)
            {
                TriangleData triangleData = underWaterTriangleData[i];

                //Calculate the forces
                Vector3 forceToAdd = Vector3.zero;

                //Force 1 - The hydrostatic force (buoyancy)
                forceToAdd += BoatPhysicsMath.BuoyancyForce(rhoWater, triangleData);

                //Force 2 - Viscous Water Resistance
                forceToAdd += BoatPhysicsMath.ViscousWaterResistanceForce(rhoWater, triangleData, Cf);

                //Force 3 - Pressure drag
                forceToAdd += BoatPhysicsMath.PressureDragForce(triangleData);

                //Force 4 - Slamming force
                //Which of the original triangles is this triangle a part of
                int originalTriangleIndex = indexOfOriginalTriangle[i];

                SlammingForceData slammingData = slammingForceData[originalTriangleIndex];

                forceToAdd += BoatPhysicsMath.SlammingForce(slammingData, triangleData, boatArea, boatMass);

                //Add the forces to the boat
                boatRB.AddForceAtPosition(forceToAdd, triangleData.center);

                //Debug

                //Normal
                //Debug.DrawRay(triangleData.center, triangleData.normal * 3f, Color.white);

                //Buoyancy
                //Debug.DrawRay(triangleData.center, buoyancyForce.normalized * -3f, Color.blue);

                //Velocity
                //Debug.DrawRay(triangleCenter, triangleVelocityDir * 3f, Color.black);

                //Viscous Water Resistance
                //Debug.DrawRay(triangleCenter, viscousWaterResistanceForce.normalized * 3f, Color.black);
            }
        }

        //Add all forces that act on the squares above the water
        void AddAboveWaterForces()
        {
            //Get all triangles
            List<TriangleData> aboveWaterTriangleData = modifyBoatMesh.aboveWaterTriangleData;

            //Loop through all triangles
            for(int i = 0; i < aboveWaterTriangleData.Count; i++)
            {
                TriangleData triangleData = aboveWaterTriangleData[i];

                //Calculate the forces
                Vector3 forceToAdd = Vector3.zero;

                //Force 1 - Air resistance
                forceToAdd += BoatPhysicsMath.AirResistanceForce(rhoAir, triangleData, 0.8f); //boatRB.drag);

                //Add the forces to the boat
                boatRB.AddForceAtPosition(forceToAdd, triangleData.center);

                //Debug

                //Normal
                //Debug.DrawRay(triangleCenter, triangleNormal * 3f, Color.white);

                //Velocity
                //Debug.DrawRay(triangleCenter, triangleVelocityDir * 3f, Color.black);

                if(triangleData.cosTheta > 0f)
                {
                    //Debug.DrawRay(triangleCenter, triangleVelocityDir * 3f, Color.black);
                }

                //Air resistance
                //-3 to show it in the opposite direction to see what's going on
                //Debug.DrawRay(triangleCenter, airResistanceForce.normalized * -3f, Color.blue);
            }
        }

        //Calculate the current velocity at the center of each triangle of the original boat mesh
        private void CalculateSlammingVelocities(List<SlammingForceData> slammingForceData)
        {
            for(int i = 0; i < slammingForceData.Count; i++)
            {
                //Set the new velocity to the old velocity
                slammingForceData[i].previousVelocity = slammingForceData[i].velocity;

                //Center of the triangle in world space
                Vector3 center = transform.TransformPoint(slammingForceData[i].triangleCenter);

                //Get the current velocity at the center of the triangle
                slammingForceData[i].velocity = BoatPhysicsMath.GetTriangleVelocity(boatRB, center);
            }
        }
    }    
}


