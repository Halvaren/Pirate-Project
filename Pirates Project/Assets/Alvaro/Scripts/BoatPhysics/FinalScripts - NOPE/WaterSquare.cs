using UnityEngine;

namespace NopeScript
{
    [RequireComponent(typeof (MeshFilter), typeof(MeshRenderer))]
    public class WaterSquare : MonoBehaviour
    {
        public float width;
        public float resolution;

        private void Awake()
        {
            Mesh mesh = new Mesh();
            mesh.name = "Procesural Grid";
            GetComponent<MeshFilter>().mesh = GenerateGrid.GenerateMesh(mesh, width, resolution);
        }
    }
}