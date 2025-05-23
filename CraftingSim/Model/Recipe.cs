using System;
using System.IO;
using System.Collections.Generic;
using Spectre.Console;

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
            RequiredMaterials = materials ?? throw new ArgumentNullException(nameof(materials));
            SuccessRate = successRate;
        }
    
        public int CompareTo(IRecipe obj)
        {
            if (obj == null) return 1;
            return string.Compare(Name, obj.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}