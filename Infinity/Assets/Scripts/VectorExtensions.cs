using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class VectorExtensions
    {
        public static Vector3Int ToVector3Int(this Vector3 vector) => new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
    }
}