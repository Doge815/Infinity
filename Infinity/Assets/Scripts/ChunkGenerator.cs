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

        [Space, Header("Surface")]
        public float SurfaceAmplitude = 4f;
        public float SurfaceScale = 0.01f;

        [Space]
        public int SurfaceOctaves = 6;
        public float SurfaceOctaveAmplitude = .2f;
        public float SurfaceOctaveScale = .5f;

        [Space, Header("Caves")]
        public float CaveScale = 0.01f;
        public float CaveThreshold = 0.2f;

        [Space]
        public int CaveOctaves = 6;
        public float CaveOctaveAmplitude = .2f;
        public float CaveOctaveScale = .5f;

        public ChunkGenerator()
        {
            Perlin = new Perlin();
        }

        public void Populate(Chunk chunk)
        {
            var map = chunk.Map ?? new BlockType[Chunk.Size.x, Chunk.Size.y, Chunk.Size.z];

            for (int x = 0; x < Chunk.Size.x; x++)
            {
                for (int z = 0; z < Chunk.Size.z; z++)
                {
                    var chunkWorldPosition = chunk.WorldPosition;
                    var worldPosition = new Vector3(x, 0, z) + chunkWorldPosition;

                    var height = (SurfaceAmplitude * Perlin.NoiseWithOctaves(SurfaceOctaves, SurfaceOctaveAmplitude, SurfaceOctaveScale,
                            worldPosition.x * SurfaceScale, worldPosition.z * SurfaceScale))
                        + GroundHeight - chunkWorldPosition.y;

                    height = Math.Min(height, Chunk.Size.y);

                    for (int y = 0; y < height; y++)
                    {
                        map[x, y, z] = Perlin.NoiseWithOctaves(CaveOctaves, CaveOctaveAmplitude, CaveOctaveScale,
                                worldPosition.x * CaveScale, worldPosition.y * CaveScale, worldPosition.z * CaveScale) > CaveThreshold
                            ? BlockTypes.Dirt
                            : null;

                        worldPosition.y++;
                    }
                }
            }

            chunk.Map = map;
            chunk.RedrawRequired = true;
        }
    }
}