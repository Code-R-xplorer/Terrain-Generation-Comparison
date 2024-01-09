using System;
using FFT;
using UnityEngine;

namespace Mesh_Gen
{
    [RequireComponent(typeof(MeshFilter))]
    public class MeshGenerator : MonoBehaviour
    {
        private Mesh _mesh;
        
        private Vector3[] _vertices;
        private int[] _triangles;

        public int xSize = 20;
        public int zSize = 20;
        
        // Start is called before the first frame update
        void Start()
        {
            _mesh = new Mesh();
            GetComponent<MeshFilter>().mesh = _mesh;
            // GenerateTerrain(Noise.GenerateNoiseGrid(xSize+1, zSize+1, 0, 0.2));
        }

        public void GenerateTerrain(float[,] noiseMap)
        {
            CreateShape(noiseMap);
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            _mesh.Clear();

            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            
            _mesh.RecalculateNormals();
        }

        private void CreateShape(float[,] noiseMap)
        {
            _vertices = new Vector3[(xSize + 1) * (zSize + 1)];
            for (int i = 0, z = 0; z <= zSize; z++)
            {
                for (int x = 0; x <= xSize; x++)
                {
                    // Debug.Log(noiseMap[x,z]);
                    // float y = Mathf.PerlinNoise(x * 0.3f, z * 0.3f) * 2f;
                    _vertices[i] = new Vector3(x, noiseMap[x,z], z);
                    i++;
                }
            }
            
            _triangles = new int[xSize * zSize * 6];

            int vert = 0;
            int tris = 0;

            for (int z = 0; z < zSize; z++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    _triangles[tris + 0] = vert;
                    _triangles[tris + 1] = vert + xSize + 1;
                    _triangles[tris + 2] = vert + 1;
                    _triangles[tris + 3] = vert + 1;
                    _triangles[tris + 4] = vert + xSize + 1;
                    _triangles[tris + 5] = vert + xSize + 2;

                    vert++;
                    tris += 6;
                }

                vert++;
            }
        }
    }
}
