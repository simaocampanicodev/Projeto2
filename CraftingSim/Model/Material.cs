using System;
using System.IO;

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

        public bool Equals(IMaterial obj)
        {
            if (obj == null || obj.GetType() != GetType()) return false;
            Material other = (Material) obj;
            return Id == other.Id || Name == other.Name;
        }
    }
}