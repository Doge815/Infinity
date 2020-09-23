using System;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;

namespace Assets
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshCollider))]
    [ExecuteInEditMode]
    public class Chunk : MonoBehaviour
    {
        public static Vector3Int Size = new Vector3Int(16, 16, 16);

        /// <summary>
        /// The position of the chunk in the world, measured from the local block at (0, 0, 0).
        /// </summary>
        public Vector3Int WorldPosition;

        /// <summary>
        /// The block types of all blocks in the chunk.
        /// </summary>
        private BlockType[,,] Map;

        public ChunkGenerator ChunkGenerator;

        public BlockType this[Vector3Int pos]
        {
            get => this[pos.x, pos.y, pos.z];
            set => this[pos.x, pos.y, pos.z] = value;
        }

        public BlockType this[int x, int y, int z]
        {
            get => Map[x, y, z];
            set => Map[x, y, z] = value;
        }

        [HideInInspector]
        public Mesh mesh;
        [HideInInspector]
        public List<Vector3> verts = new List<Vector3>();
        [HideInInspector]
        public List<int> tris = new List<int>();
        [HideInInspector]
        public List<Vector2> uv = new List<Vector2>();
        [HideInInspector]
        public MeshFilter meshFilter;
        [HideInInspector]
        public MeshCollider meshCollider;

        public void Start()
        {
            Map = new BlockType[Size.x, Size.y, Size.z];

            ChunkGenerator.Populate(this);

            mesh = new Mesh();

            meshFilter = GetComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;
            meshCollider = GetComponent<MeshCollider>();

            RegenerateMesh();
        }

        public void RegenerateMesh()
        {
            verts.Clear();
            tris.Clear();
            uv.Clear();

            for (int x = 0; x < Size.x; x++)
            {
                for (int y = 0; y < Size.y; y++)
                {
                    for (int z = 0; z < Size.z; z++)
                    {
                        var block = this[x, y, z];
                        if (block == null) continue;
                        DrawBlock(x, y, z);
                    }
                }
            }

            mesh.Clear();
            mesh.vertices = verts.ToArray();
            mesh.triangles = tris.ToArray();
            mesh.RecalculateNormals();

            meshCollider.sharedMesh = null;
            meshCollider.sharedMesh = mesh;
        }

        private void DrawBlock(int x, int y, int z)
        {
            var pos = new Vector3(x, y, z) + (Vector3.forward / 2) + (Vector3.up / 2) + (Vector3.right / 2);
            Vector3 offset1, offset2;

            if(IsInvisible(x, y-1, z))
            {
                offset1 = Vector3.left;
                offset2 = Vector3.back;
                DrawTriangle(pos + (Vector3.down / 2), offset1, offset2);
            }
            if (IsInvisible(x, y + 1, z))
            {
                offset1 = Vector3.right;
                offset2 = Vector3.back;
                DrawTriangle(pos + (Vector3.up / 2), offset1, offset2);
            }
            if (IsInvisible(x - 1, y, z))
            {
                offset1 = Vector3.up;
                offset2 = Vector3.back;
                DrawTriangle(pos + (Vector3.left / 2), offset1, offset2);
            }
            if (IsInvisible(x + 1, y, z))
            {
                offset1 = Vector3.down;
                offset2 = Vector3.back;
                DrawTriangle(pos + (Vector3.right / 2), offset1, offset2);
            }
            if (IsInvisible(x, y, z-1))
            {
                offset1 = Vector3.left;
                offset2 = Vector3.up;
                DrawTriangle(pos + (Vector3.back / 2), offset1, offset2);
            }
            if (IsInvisible(x, y, z+1))
            {
                offset1 = Vector3.right;
                offset2 = Vector3.up;
                DrawTriangle(pos + (Vector3.forward / 2), offset1, offset2);
            }
        }

        private void DrawTriangle(Vector3 origin, Vector3 offset1, Vector3 offset2)
        {
            var index = verts.Count;

            origin -= (offset1 + offset2) / 2;

            verts.Add(origin);
            verts.Add(origin + offset1);
            verts.Add(origin + offset2);
            verts.Add(origin + offset1 + offset2);

            tris.Add(index + 0);
            tris.Add(index + 1);
            tris.Add(index + 2);
            tris.Add(index + 3);
            tris.Add(index + 2);
            tris.Add(index + 1);
        }

        private bool IsInvisible(int x, int y, int z) =>
            x < 0 || y < 0 || z < 0
            || x >= Size.x || y >= Size.y || z >= Size.z
            || Map[x, y, z] == null;
    }
}