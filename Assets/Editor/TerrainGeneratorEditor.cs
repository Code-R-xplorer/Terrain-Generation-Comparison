using FFT;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(TerrainGenerator))]
    public class TerrainGeneratorEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            TerrainGenerator terrainGenerator = (TerrainGenerator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Generate"))
            {
                terrainGenerator.GenerateTerrain();
            }
        }
    }
}