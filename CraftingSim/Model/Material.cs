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
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public bool Equals(IMaterial other)
        {
            if (other == null) return false;
            return Id == other.Id || 
                   string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }

        public override bool Equals(object obj)
        {
            return obj is IMaterial other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name.ToLowerInvariant());
        }
    }
}