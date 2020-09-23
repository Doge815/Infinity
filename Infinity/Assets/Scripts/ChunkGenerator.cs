using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

namespace Assets
{
    [Serializable]
    public class ChunkGenerator
    {
        [HideInInspector]
        public Perlin Perlin;

        public float GroundHeight = 6f;

        [Space]
        public float Amplitude = 4f;
        public float Scale = 0.1f;

        [Space]
        public int Octaves = 4;
        public float OctaveAmplitude = .5f;
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
                        (Amplitude * Perlin.NoiseWithOctaves(Octaves, OctaveAmplitude, OctaveScale, (x - worldPosition.x) * Scale, (z - worldPosition.z) * Scale))
                        + GroundHeight - worldPosition.y;

                    var heightInChunk = Math.Min(Mathf.Floor(height), Chunk.Size.y);

                    for (int y = 0; y < heightInChunk; y++)
                    {
                        chunk[x, y, z] = BlockTypes.Dirt;
                    }
                }
            }
        }
    }
}