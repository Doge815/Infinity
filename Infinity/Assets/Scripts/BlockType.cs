using System;
using System.Collections.Generic;

namespace Assets.Scripts
{
    public class BlockType : IEquatable<BlockType>
    {
        public string Id { get; }

        public BlockType(string id)
        {
            Id = id;
        }

        public bool Equals(BlockType other) => other != null && string.Equals(Id, other.Id, StringComparison.OrdinalIgnoreCase);

        public override bool Equals(object obj) => obj is BlockType other && Equals(other);

        public override int GetHashCode() => 2108858624 + EqualityComparer<string>.Default.GetHashCode(Id);

        public override string ToString() => Id;

        public static bool operator ==(BlockType lhs, BlockType rhs) => lhs?.Equals(rhs) ?? rhs is null;

        public static bool operator !=(BlockType lhs, BlockType rhs) => !(lhs == rhs);
    }
}