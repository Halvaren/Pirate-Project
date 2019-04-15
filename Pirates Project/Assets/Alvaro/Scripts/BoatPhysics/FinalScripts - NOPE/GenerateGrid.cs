using UnityEngine;

namespace NopeScript
{
    public static class GenerateGrid
    {
        public static Mesh GenerateMesh(Mesh mesh, float width, float resolution)
        {
            int num1 = (int) (width / resolution);
            int num2 = num1;
            int num3 = num1;

            Vector3[] vector3Array = new Vector3[(num2 + 1) * (num3 + 1)];
            Vector2[] vector2Array = new Vector2[vector3Array.Length];
            Vector4[] vector4Array = new Vector4[vector3Array.Length];

            Vector4 vector4 = new Vector4(1f, 0.0f, 0.0f, -1f);
            float z = width / 2.0f;

            int i = 0;
            for(int j = 0; j < num3; j++)
            {
                float x = width / 2.0f;
                for(int k = 0; k < num2; k++)
                {
                    vector3Array[i] = new Vector3(x, 0.0f, z);
                    vector2Array[i] = new Vector2(k / num2, j / num3);
                    vector4Array[i] = vector4;

                    x += resolution;
                    i++;
                }
                z += resolution;
            }

            int[] numArray = new int[num2 * num3 * 6];
            i = 0;
            int num4 = 0;
            int num5 = 0;
            while(num5 < num3)
            {
                int num6 = 0;
                while(num6 < num2)
                {
                    numArray[i] = num4;
                    numArray[i + 3] = numArray[i + 2] = num4 + 1;
                    numArray[i + 4] = numArray[i + 1] = num4 + num2 + 1;
                    numArray[i + 5] = num4 + num2 + 2;
                    num6++;
                    i += 6;
                    num4++;
                }
                num5++;
                num4++;
            }

            mesh.vertices = vector3Array;
            mesh.uv = vector2Array;
            mesh.tangents = vector4Array;
            mesh.triangles = numArray;
            mesh.RecalculateNormals();
            return mesh;
        }
    }
}