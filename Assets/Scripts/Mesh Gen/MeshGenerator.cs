using System;
using UnityEngine;
using UnityEngine.Rendering;

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
            _mesh.indexFormat = IndexFormat.UInt32; 
            GetComponent<MeshFilter>().mesh = _mesh;
        }

        public void GenerateTerrain(float[,] noiseMap, int size, float scale)
        {
            CreateShape(noiseMap, size, scale);
            UpdateMesh();
        }

        private void UpdateMesh()
        {
            // Clear any previous generated mesh 
            _mesh.Clear();
                
            // Add the generated vertices and triangles
            _mesh.vertices = _vertices;
            _mesh.triangles = _triangles;
            
            // Recalculate the mesh normals so lighting can be properly calculated
            _mesh.RecalculateNormals();
        }
        
        private void CreateShape(float[,] noiseMap, int size, float scale)
        {
            size--;
            _vertices = new Vector3[(size + 1) * (size + 1)];
            
            for (int i = 0, z = 0; z <= size; z++)
            {
                for (int x = 0; x <= size; x++)
                {
                    // Apply scale to x and z positions
                    float worldX = x * scale;
                    float worldZ = z * scale;

                    _vertices[i] = new Vector3(worldX, noiseMap[x,z], worldZ);
                    i++;
                }
            }
    
            _triangles = new int[size * size * 6];

            for (int vert = 0, tris = 0, z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    // a----b
                    // |    |
                    // c----d
                    
                    _triangles[tris + 0] = vert; // c
                    _triangles[tris + 1] = vert + size + 1; // a
                    _triangles[tris + 2] = vert + 1; // d
                    _triangles[tris + 3] = vert + 1; // d
                    _triangles[tris + 4] = vert + size + 1; // a
                    _triangles[tris + 5] = vert + size + 2; // b

                    vert++;
                    tris += 6;
                }
                
                // Increase vert at the end of the row to
                // prevent the last vert of the previous row being connected
                vert++; 
            }
        }
    }
}
