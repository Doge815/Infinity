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

                    for (int y = 0; y < Chunk.Size.y; y++)
                    {
                        chunk[x, y, z] = y < height ? BlockTypes.Dirt : null;
                    }
                }
            }
        }
    }
}