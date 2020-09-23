using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class Local
    {
        public static int RenderDistance = 10;
        public static List<List<List<Chunk>>> LoadedChunks = new List<List<List<Chunk>>>();
        public static void ForceRedraw() => throw new NotImplementedException();
        public static void LoadNeededChunks(Vector3 pos) => throw new NotImplementedException();
    }
}