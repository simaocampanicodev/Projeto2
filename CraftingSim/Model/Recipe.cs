using System;
using System.Collections.Generic;

namespace CraftingSim.Model
{
    public class Recipe : IRecipe
    {
        public string Name { get; }
        public IReadOnlyDictionary<IMaterial, int> RequiredMaterials { get; }
        public double SuccessRate { get; }

        public Recipe(string name, Dictionary<IMaterial, int> materials, double successRate)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            RequiredMaterials = new Dictionary<IMaterial, int>(materials ?? 
                                                               throw new ArgumentNullException(nameof(materials)));
            SuccessRate = successRate;

            if (successRate < 0 || successRate > 1)
            {
                throw new ArgumentException("Success rate must be between 0 and 1", 
                    nameof(successRate));
            }
        }

        public int CompareTo(IRecipe other)
        {
            if (other == null) return 1;
            return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}