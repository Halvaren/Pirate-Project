using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace NopeScript
{
    public class ConvexHull
    {
        public static List<Vector3> SortVerticesConvexHull(List<Vector3> unSortedList)
        {
            List<Vector3> sortedList = new List<Vector3>();

            float smallestValue = unSortedList[0].x;
            int smallestIndex = 0;

            for(int i = 1; i < unSortedList.Count; i++)
            {
                if(unSortedList[i].x < smallestValue)
                {
                    smallestValue = unSortedList[i].x;
                    smallestIndex = i;
                }
                else if(unSortedList[i].x == smallestValue)
                {
                    if(unSortedList[i].z < unSortedList[smallestIndex].z)
                    {
                        smallestIndex = i;
                    }
                }
            }

                sortedList.Add(unSortedList[smallestIndex]);

                unSortedList.RemoveAt(smallestIndex);

                Vector3 firstPoint = sortedList[0];
                firstPoint.y = 0f;

                unSortedList = unSortedList.OrderBy(n => GetAngle(new Vector3(n.x, 0f, n.z) - firstPoint)).ToList();

                unSortedList.Reverse();

                sortedList.Add(unSortedList[unSortedList.Count - 1]);

                unSortedList.RemoveAt(unSortedList.Count - 1);

                int safety = 0; //XD
                while(unSortedList.Count > 0 && safety < 1000)
                {
                    safety += 1;

                    Vector3 a = sortedList[sortedList.Count - 2];
                    Vector3 b = sortedList[sortedList.Count - 1];

                    Vector3 c = unSortedList[unSortedList.Count - 1];
                    unSortedList.RemoveAt(unSortedList.Count - 1);

                    sortedList.Add(c);
                    while(isClockWise(a, b, c) && safety < 1000)
                    {
                        sortedList.RemoveAt(sortedList.Count - 2);

                        a = sortedList[sortedList.Count - 3];
                        b = sortedList[sortedList.Count - 2];
                        c = sortedList[sortedList.Count - 1];

                        safety += 1;
                    }
                }

                return sortedList;
        }

        private static bool isClockWise(Vector3 a, Vector3 b, Vector3 c)
        {
            float signedArea = (b.x - a.x) * (c.z - a.z) - (b.z - a.z) * (c.x - a.x);

            if(signedArea > 0f)
            {
                return false;
            }
            else return true;
        }

        private static float GetAngle(Vector3 vec)
        {
            float angle = Mathf.Atan2(vec.z, vec.x);

            return angle;
        }
    }  
}
