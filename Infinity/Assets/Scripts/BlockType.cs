using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public struct BlockType : System.IEquatable<BlockType>
    {
        public string Id;

        public bool Equals(BlockType other) => string.Equals(Id, other.Id, System.StringComparison.CurrentCulture);

        public override bool Equals(object obj) => obj is BlockType other ? Equals(other);

        public override int GetHashCode() => 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);

        public override string ToString() => Id;

        public static bool operator ==(BlockType lhs, BlockType rhs) => lhs.Equals(rhs);

        public static bool operator !=(BlockType lhs, BlockType rhs) => !(lhs == rhs);
    }
}