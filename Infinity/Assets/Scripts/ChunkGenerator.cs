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
                    var worldPosition = chunk.WorldPosition;

                    var height =
                        (Amplitude * Perlin.NoiseWithOctaves(Octaves, OctaveAmplitude, OctaveScale, (x + worldPosition.x) * Scale, (z + worldPosition.z) * Scale))
                        + GroundHeight - worldPosition.y;

                    height = Math.Min(height, Chunk.Size.y);

                    for (int y = 0; y < height; y++)
                    {
                        chunk[x, y, z] = BallsItch(x, y, z) ? BlockTypes.Dirt : null;
                    }
                }
            }
        }

        public static  bool BallsItch(float x, float y,  float  z)
        {
            float noiseScale = 0.05f;
            return PerlinNoise3D(x   *  noiseScale, y  *  noiseScale, z *  noiseScale) > 0.5;
        }

        public static float PerlinNoise3D(float x, float y, float z)
        {
            y += 1;
            z += 2;
            float xy = _perlin3DFixed(x, y);
            float xz = _perlin3DFixed(x, z);
            float yz = _perlin3DFixed(y, z);
            float yx = _perlin3DFixed(y, x);
            float zx = _perlin3DFixed(z, x);
            float zy = _perlin3DFixed(z, y);
            return xy * xz * yz * yx * zx * zy;
        }
        static float _perlin3DFixed(float a, float b)
        {
            return Mathf.Sin(Mathf.PI * Mathf.PerlinNoise(a, b));
        }
    }
}