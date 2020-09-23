using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public class World
    {
        public static World ActiveWorld;
        public Dictionary<Vector3Int, Chunk> Chunks;

        public World()
        {
            Chunks = new Dictionary<Vector3Int, Chunk>();
        }
    }
}