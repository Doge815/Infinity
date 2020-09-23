using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Local
    {
        public static Local ActiveLocal = new Local();

        public int RenderDistance = 10;

        public Dictionary<Vector3Int, Chunk> LoadedChunks = new Dictionary<Vector3Int, Chunk>();

        public void ForceRedraw() => throw new NotImplementedException();

        public void LoadNeededChunks(Vector3 pos) => throw new NotImplementedException();
    }
}