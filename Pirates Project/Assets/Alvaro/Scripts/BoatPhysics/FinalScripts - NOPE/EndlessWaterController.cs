using System.Collections.Generic;
using UnityEngine;

namespace NopeScript
{
    public class EndlessWaterController: MonoBehaviour
    {
        private float chunkWidth = 200f;
        private List<EndlessWaterController.WaterChunk> allWaterChunks = new List<EndlessWaterController.WaterChunk>();
        private List<Vector3> coordinatesWithoutChunkList = new List<Vector3>();

        public static EndlessWaterController current;
        public GameObject toFollowObj;
        public GameObject waterSquareObj;
        public Transform waterCylinder;
        private int oldChunkPosX;
        private int oldChunkPosZ;
        private int oldChunkResolution;

        private void Start()
        {
            EndlessWaterController.current = this;
            GenerateSea();

            oldChunkPosX = 200000;
            oldChunkPosZ = 200000;
            oldChunkResolution = 20000;
        }

        private void Update()
        {
            MoveWaterToObjToFollow();
            UpdateWaterChunks();
        }

        private void GenerateSea()
        {
            allWaterChunks.Clear();
            
            for (int i = 0; i < 20; i++)
                AddNewWaterSquare();
        }

        private void AddNewWaterSquare()
        {
            EndlessWaterController.WaterChunk waterChunk = new EndlessWaterController.WaterChunk();
            allWaterChunks.Add(waterChunk);

            WaterSquare component = waterSquareObj.GetComponent<WaterSquare>();
            component.width = chunkWidth;

            component.resolution = 2f;
            GameObject highDetailedWaterChunk = Instantiate(waterSquareObj, transform);
            waterChunk.highDetailedWaterChunk = highDetailedWaterChunk;

            component.resolution = 5f;
            GameObject mediumDetailedWaterChunk = Instantiate(waterSquareObj, transform);
            waterChunk.mediumDetailedWaterChunk = mediumDetailedWaterChunk;

            component.resolution = 50f;
            GameObject lowDetailedWaterChunk = Instantiate(waterSquareObj, transform);
            waterChunk.lowDetailedWaterChunk = lowDetailedWaterChunk;
        }

        private void MoveWaterToObjToFollow()
        {
            Vector3 position = toFollowObj.transform.position;
            position.y = -20f;
            //waterCylinder.position = position;
        }

        private void UpdateWaterChunks()
        {
            int nChunksPerSide = Mathf.RoundToInt(500f / chunkWidth);
            int num2 = Mathf.RoundToInt(toFollowObj.transform.position.x / chunkWidth);
            int num3 = Mathf.RoundToInt(toFollowObj.transform.position.z / chunkWidth);

            if(num2 == oldChunkPosX && num3 == oldChunkPosZ && oldChunkResolution == GetChunkResolution())
            {
                return;
            }

            oldChunkPosX = num2;
            oldChunkPosZ = num3;
            oldChunkResolution = GetChunkResolution();

            for(int i = 0; i < allWaterChunks.Count; i++)
            {
                allWaterChunks[i].DeactivateChunk();
            }
            coordinatesWithoutChunkList.Clear();

            for(int i = - nChunksPerSide; i <= nChunksPerSide; i++)
            {
                for(int j = - nChunksPerSide; j <= nChunksPerSide; j++)
                {
                    Vector3 position = new Vector3(num2 + j, 0.0f, num3 + i);
                    position.x *= chunkWidth;
                    position.y *= chunkWidth;

                    bool flag = false;
                    for(int k = 0; k < allWaterChunks.Count; k++)
                    {
                        if(allWaterChunks[k].chunkPosition == position)
                        {
                            int chunkResolution = GetChunkResolution();
                            allWaterChunks[k].ActivateChunk(chunkResolution);

                            flag = true;
                            break;
                        }
                    }
                    if(!flag) coordinatesWithoutChunkList.Add(position);
                }
            }

            for(int i = 0; i < coordinatesWithoutChunkList.Count; i++)
            {
                bool flag = false;

                for(int j = 0; j < allWaterChunks.Count; j++)
                {
                    if(!allWaterChunks[j].isActive)
                    {
                        Vector3 coordinatesWithoutChunk = coordinatesWithoutChunkList[i];
                        int chunkResolution = GetChunkResolution();
                        allWaterChunks[i].ActivateChunk(chunkResolution, coordinatesWithoutChunk);

                        flag = true;
                        break;
                    }
                }

                if(!flag)
                {
                    AddNewWaterSquare();
                    allWaterChunks[allWaterChunks.Count - 1].ActivateChunk(GetChunkResolution(), coordinatesWithoutChunkList[i]);
                }
            }
        }

        private int GetChunkResolution()
        {
            float y = toFollowObj.transform.position.y;

            if(y >= 50.0f)
            {
                if(y >= 150.0f) return 2;
                else return 1;
            }
            else return 0;
        }

        public class WaterChunk
        {
            public GameObject highDetailedWaterChunk;
            public GameObject mediumDetailedWaterChunk;
            public GameObject lowDetailedWaterChunk;

            public Vector3 chunkPosition;
            public bool isActive;

            public WaterChunk()
            {
                chunkPosition = Vector3.zero;
            }

            public void DeactivateChunk()
            {
                highDetailedWaterChunk.SetActive(false);
                mediumDetailedWaterChunk.SetActive(false);
                lowDetailedWaterChunk.SetActive(false);
                isActive = false;
            }

            public void ActivateChunk(int resolution)
            {
                GetChunk(resolution).SetActive(true);
                isActive = true;
            }

            public void ActivateChunk(int resolution, Vector3 chunkPos)
            {
                GameObject chunk = GetChunk(resolution);
                chunk.SetActive(true);
                chunk.transform.position = chunkPos;
                isActive = true;
            }

            private GameObject GetChunk(int resolution)
            {
                if(resolution == 0)
                    return highDetailedWaterChunk;

                if(resolution == 1)
                    return mediumDetailedWaterChunk;

                return lowDetailedWaterChunk;
            }
        }
    }
}
