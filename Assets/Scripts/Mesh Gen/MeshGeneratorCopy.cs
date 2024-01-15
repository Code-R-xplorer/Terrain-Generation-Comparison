using System;
using FFT;
using UnityEngine;

namespace Mesh_Gen
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGeneratorCopy : MonoBehaviour
    {
        private Mesh _mesh;
        
        private Vector3[] _vertices;
        private int[] _triangles;
        
        // Start is called before the first frame update
        void Start()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        public void GenerateTerrain(float[,] noiseMap, int width, int depth)
        {
            CreateShape(noiseMap, width - 1, depth - 1);
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            
            _mesh.RecalculateNormals();
        }

        private void CreateShape(float[,] noiseMap, int width, int depth)
        {
            _vertices = new Vector3[(width + 1) * (depth + 1)];
            for (int i = 0, z = 0; z <= depth; z++)
            {
                for (int x = 0; x <= width; x++)
                {
                    // Debug.Log(noiseMap[x,z]);
                    // float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                    _vertices[i] = new Vector3(x, noiseMap[x, z], z);
                    i++;
                }
            }
            
            _triangles = new int[width * depth * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    _triangles[tris + 0] = vert;
                    _triangles[tris + 1] = vert + width + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + width + 1;
                    _triangles[tris + 5] = vert + width + 2;

                    vert++;
                    tris += 6;
                }

                vert++;
            }
        }

        public void FuckItImWritingMyOwnCreateShapeFunction(int width, int depth)
        {
            _vertices = new Vector3[(width + 1) * (depth + 1)];
            //_vertices = new Vector3[(width + 1) * (depth + 1)];
            for (int i = 0, z = 0; z <= depth; z++)
            {
                for (int x = 0; x <= width; x++)
                {
                    // Debug.Log(noiseMap[x,z]);
                    // float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                    //_vertices[i] = new Vector3(x, UnityEngine.Random.Range(-1,1), z);
                    
                    _vertices[i] = new Vector3(x, 0, z);

                    //if (i == width * depth / 4)
                    //{

                    //    _vertices[i] = new Vector3(x, 10, z);
                    //}
                    //if (x == width / 2)
                    //{

                    //    _vertices[i] = new Vector3(x, UnityEngine.Random.Range(-1, 1), z);
                    //}

                    i++;
                    //print(i);
                }
            }


            //_vertices[_vertices.Length / 2].y = 20;


            _triangles = new int[width * depth * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    _triangles[tris + 0] = vert;
                    _triangles[tris + 1] = vert + width + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + width + 1;
                    _triangles[tris + 5] = vert + width + 2;

                    vert++;
                    tris += 6;
                }

                vert++;
            }
            //UpdateMesh();
        }

        public void TestForMidpoint()
        {

            UpdateMesh();
        }

        public Vector3[] GetVertices()
        {
            return _vertices;
        }

        public void SetVertexHeight(int location, float value)
        {
            _vertices[location].y = value;
        }
    }
}
