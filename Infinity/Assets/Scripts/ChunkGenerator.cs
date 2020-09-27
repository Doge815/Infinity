using System;
using TreeEditor;
using UnityEngine;

namespace Assets.Scripts
{
    [CreateAssetMenu(menuName = "Chunk Generator")]
    public class ChunkGenerator : ScriptableObject
    {
        [HideInInspector]
        public Perlin Perlin;

        public float GroundHeight = 6f;

        [Space]
        public float Amplitude = 4f;
        public float Scale = 0.01f;

        [Space]
        public int Octaves = 6;
        public float OctaveAmplitude = .2f;
        public float OctaveScale = .5f;

        public ChunkGenerator()
        {
            Perlin = new Perlin();
        }

        public void Populate(Chunk chunk)
        {
            for (int x = 0; x < Chunk.Size.x; x++)
            {
                for (int z = 0; z < Chunk.Size.z; z++)
                {
                    var chunkWorldPosition = chunk.WorldPosition;
                    var NoisePos = (new Vector3(x, 0, z)  + chunkWorldPosition)  * Scale;

                    var height =
                        (Amplitude * Perlin.NoiseWithOctaves(Octaves, OctaveAmplitude, OctaveScale, (x + chunkWorldPosition.x) * Scale, (z + chunkWorldPosition.z) * Scale))
                        + GroundHeight - chunkWorldPosition.y;

                    height = Math.Min(height, Chunk.Size.y);

                    for (int y = 0; y < height; y++)
                    {
                        chunk[x, y, z] = (PerlinNoise3D(NoisePos.x, NoisePos.y  +  y  * Scale, NoisePos.z) > 0.2f) ? BlockTypes.Dirt : null;
                    }
                }
            }
        }

        public float PerlinNoise3D(float x, float y, float z) => Perlin.NoiseWithOctaves(3, 1, .5f, x, y, z);
    }
}