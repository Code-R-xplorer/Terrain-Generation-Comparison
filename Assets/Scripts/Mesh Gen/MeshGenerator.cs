using UnityEngine;

namespace Mesh_Gen
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour
    {
        private Mesh _mesh;
        
        private Vector3[] _vertices;
        private int[] _triangles;
        
        // Start is called before the first frame update
        void Start()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;

            CreateShape();
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            
            _mesh.RecalculateNormals();
        }

        private void CreateShape()
        {
            _vertices = new[]
            {
                new Vector3(0,0,0),
                new Vector3(0,0,1),
                new Vector3(1,0,0),
                new Vector3(1,0,1)
            };

            _triangles = new[]
            {
                0, 1, 2,
                1, 3, 2
            };
        }
    }
}
