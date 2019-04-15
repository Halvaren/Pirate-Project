using System.Collections.Generic;
using UnityEngine;

namespace NopeScript
{
    public class GenerateExtraBoatMeshes
    {
        private Transform boatTrans;

        public GenerateExtraBoatMeshes(GameObject boatObj)
        {
            boatTrans = boatObj.transform;
        }

        public void DisplayMesh(Mesh mesh, string name, List<TriangleData> trianglesData)
        {
            List<Vector3> vertexList = new List<Vector3>();
            List<int> indexList = new List<int>();

            for(int i = 0; i < trianglesData.Count; i++)
            {
                Vector3 p1Position = boatTrans.InverseTransformPoint(trianglesData[i].p1);
                Vector3 p2Position = boatTrans.InverseTransformPoint(trianglesData[i].p2);
                Vector3 p3Position = boatTrans.InverseTransformPoint(trianglesData[i].p3);

                vertexList.Add(p1Position);
                indexList.Add(vertexList.Count - 1);
                
                vertexList.Add(p2Position);
                indexList.Add(vertexList.Count - 1);

                vertexList.Add(p3Position);
                indexList.Add(vertexList.Count - 1);
            }

            mesh.Clear();
            mesh.name = name;
            mesh.vertices = vertexList.ToArray();
            mesh.triangles = indexList.ToArray();
            mesh.RecalculateBounds();
        }

        public void DisplayMirrorMesh(Mesh mesh, string name, List<TriangleData> trianglesData)
        {
            float time = Time.time;
            for(int i = 0; i < trianglesData.Count; i++)
            {
                TriangleData triangleData = trianglesData[i];
                triangleData.p1.y -= WaterController.current.DistanceToWater(triangleData.p1, time) * 2f;
                triangleData.p2.y -= WaterController.current.DistanceToWater(triangleData.p2, time) * 2f;
                triangleData.p3.y -= WaterController.current.DistanceToWater(triangleData.p3, time) * 2f;

                Vector3 p2 = triangleData.p2;
                triangleData.p2 = triangleData.p3;
                triangleData.p3 = p2;

                trianglesData[i] = triangleData;
            }
            DisplayMesh(mesh, name, trianglesData);
        }

        public void GenerateFoamSkirt(Mesh mesh, string name, List<Vector3> intersectionVertices)
        {
            List<Vector3> cleanedVertices = CleanVertices(intersectionVertices);
            List<Vector3> sortedVertices = ConvexHull.SortVerticesConvexHull(cleanedVertices);
            List<Vector3> finalVertices = AddVertices(sortedVertices);
            CreateFoamMesh(finalVertices, mesh, name);
        }

        private List<Vector3> CleanVertices(List<Vector3> intersectionVertices)
        {
            List<Vector3> result = new List<Vector3>();
            for(int i = 0; i < intersectionVertices.Count; i++)
            {
                bool flag = false;
                for(int j = 0; j < result.Count; i++)
                {
                    if(Vector3.SqrMagnitude(result[j] - intersectionVertices[i]) < 0.1f)
                    {
                        flag = true;
                        break;
                    }
                }
                if(!flag) result.Add(intersectionVertices[i]);
            }
            return result;
        }
        
        //Add more vertices by splitting sections that are too far away to get a smoother foam
        private List<Vector3> AddVertices(List<Vector3> sortedVertices)
        {
            List<Vector3> finalVertices = new List<Vector3>();
            float distBetweenNewVertices = 4f;

            for(int i = 0; i < sortedVertices.Count; i++)
            {
                int j = i - 1;
                if(j < 0) j = sortedVertices.Count - 1;

                Vector3 lastVertex = sortedVertices[j];
                Vector3 thisVertex = sortedVertices[i];

                float distanceBetweenVertices = Vector3.Magnitude(thisVertex - lastVertex);
                Vector3 directionBetweenVertices = Vector3.Normalize(thisVertex - lastVertex);

                int newVertices = Mathf.FloorToInt(distanceBetweenVertices / distBetweenNewVertices);

                finalVertices.Add(lastVertex);

                for(int k = 1; k < newVertices; k++)
                {
                    Vector3 newVert = lastVertex + j * directionBetweenVertices * distBetweenNewVertices;

                    finalVertices.Add(newVert);
                }
            }

            finalVertices.Add(sortedVertices[sortedVertices.Count - 1]);

            float timeSinceStart = Time.time;

            for(int i = 0; i < finalVertices.Count; i++)
            {
                Vector3 thisVertex = finalVertices[i];

                thisVertex.y = WaterController.current.GetWaveYPos(thisVertex, timeSinceStart);
                thisVertex.y += 0.1f;
                
                finalVertices[i] = thisVertex;
            }

            return finalVertices;
        }

        private void CreateFoamMesh(List<Vector3> finalVertices, Mesh mesh, string name)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();
            List<Vector2> uvs = new List<Vector2>();

            float foamSize = 2f;
            float timeSinceStart = Time.time;

            Vector3 TL = finalVertices[finalVertices.Count - 1];
            Vector3 TR = finalVertices[0];

            Vector3 vecBetween = Vector3.Normalize(TR - TL);

            Vector3 normal = new Vector3(vecBetween.z, 0f, -vecBetween.x);

            Vector3 vecBetweenLeft = Vector3.Normalize(TL - finalVertices[finalVertices.Count - 2]);

            Vector3 normalLeft = new Vector3(vecBetweenLeft.z, 0f, -vecBetweenLeft.x);

            Vector3 averageNormalLeft = Vector3.Normalize((normalLeft + normal) * 0.5f);

            Vector3 BL = TL + averageNormalLeft * foamSize;

            BL.y = WaterController.current.GetWaveYPos(BL, timeSinceStart);

            Vector3 TL_local = boatTrans.InverseTransformPoint(TL);
            Vector3 BL_local = boatTrans.InverseTransformPoint(BL);

            vertices.Add(TL_local);
            vertices.Add(BL_local);

            for(int i = 0; i < finalVertices.Count; i++)
            {
                int rightPos = i + 1;
                if(rightPos > finalVertices.Count - 1) rightPos = 0;

                Vector3 vecBetweenRight = Vector3.Normalize(finalVertices[rightPos] - TR);

                Vector3 normalRight = new Vector3(vecBetweenRight.z, 0f, -vecBetweenRight.x);
                
                Vector3 averageNormalRight = Vector3.Normalize((normalRight + normal) * 0.5f);

                Vector3 BR = TR + averageNormalRight * foamSize;

                BR.y = WaterController.current.GetWaveYPos(BR, timeSinceStart);

                Vector3 TR_local = boatTrans.InverseTransformPoint(TR);
                Vector3 BR_local = boatTrans.InverseTransformPoint(BR);

                vertices.Add(TR_local);
                vertices.Add(BR_local);

                uvs.Add(new Vector2(1f, 0f));
                uvs.Add(new Vector2(1f, 1f));

                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 1);
                triangles.Add(vertices.Count - 3);
                
                triangles.Add(vertices.Count - 4);
                triangles.Add(vertices.Count - 2);
                triangles.Add(vertices.Count - 1);

                normalLeft = normal;

                normal = normalRight;

                averageNormalLeft = averageNormalRight;

                TL = TR;
                TR = finalVertices[rightPos];
            }

            mesh.Clear();
            mesh.name = name;
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            mesh.uv = uvs.ToArray();

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
        }

        private void DisplayVerticesOrder(List<Vector3> verticesList, Color color)
        {
            float height = 0.5f;
            for(int i = 0; i < verticesList.Count; i++)
            {
                Vector3 start = verticesList[i] + Vector3.up * height;

                int endPos = i + 1;

                if(i == verticesList.Count - 1) endPos = 0;
                
                Vector3 end = verticesList[endPos] + Vector3.up * height;

                Debug.DrawLine(start, end, color);
            }
        }

        private void DisplayVerticesOrderHeight(List<Vector3> verticesList, Color color)
        {
            float length = 0.1f;
            for(int i = 0; i < verticesList.Count; i++)
            {
                Debug.DrawRay(verticesList[i], Vector3.up * length, color);

                length += 0.2f;
            }
        }
    }
}