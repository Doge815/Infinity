using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public static class VectorExtensions
    {
        public static Vector3Int ToVector3Int(this Vector3 vector) => new Vector3Int((int)vector.x, (int)vector.y, (int)vector.z);
        public static Vector3 ToVector3(this Vector3Int vector) => new Vector3(vector.x, vector.y, vector.z);

        public static Vector3Int Divide(this Vector3Int lhs, Vector3Int rhs) => new Vector3Int(lhs.x / rhs.x, lhs.y / rhs.y, lhs.z / rhs.z);
        public static Vector3Int Divide(this Vector3Int lhs, int rhs) => new Vector3Int(lhs.x / rhs, lhs.y / rhs, lhs.z / rhs);
    }
}