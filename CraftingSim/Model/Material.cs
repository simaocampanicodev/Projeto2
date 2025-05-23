using System;

namespace CraftingSim.Model
{
    public class Material : IMaterial
    {
        public int Id { get; }
        public string Name { get; }

        public Material(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public bool Equals(IMaterial other)
        {
            if (other == null) return false;
            return Id == other.Id || Name.Equals(other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            if (obj is IMaterial other)
            {
                return Equals(other);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name.ToLowerInvariant());
        }
    }
}