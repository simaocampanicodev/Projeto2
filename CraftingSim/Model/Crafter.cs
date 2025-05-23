using System;
using System.Collections.Generic;
using System.IO;
using Spectre.Console;

namespace CraftingSim.Model
{
    /// <summary>
    /// Implementation of ICrafter. 
    /// </summary>
    public class Crafter : ICrafter
    {
        private readonly Inventory inventory;
        private readonly List<IRecipe> recipeList;

        public Crafter(Inventory inventory)
        {
            this.inventory = inventory;
            recipeList = new List<IRecipe>();
        }

        /// <summary>
        /// returns a read only list of loaded recipes.
        /// </summary>
        public IEnumerable<IRecipe> RecipeList => recipeList;

        /// <summary>
        /// Loads recipes from the files.
        /// Must parse the name, success rate, required materials and
        /// necessary quantities.
        /// </summary>
        /// <param name="recipeFiles">Array of file paths</param>
        public void LoadRecipesFromFile(string[] recipeFiles)
        {
            foreach (string file in recipeFiles)
            {
                try
                {
                    if (!File.Exists(file)) continue;

                    string[] lines = File.ReadAllLines(file);
                    if (lines.Length < 2) continue;
                    
                    // Parse recipe header (name, success rate)
                    string[] recipeInfo = lines[0].Split(',');
                    if (recipeInfo.Length != 2) continue;

                    string recipeName = recipeInfo[0].Trim();
                    
                    // Try parsing success rate as decimal with both . and , separators
                    string successRateStr = recipeInfo[1].Trim().Replace(',', '.');
                    if (!double.TryParse(successRateStr, System.Globalization.NumberStyles.Float, 
                        System.Globalization.CultureInfo.InvariantCulture, out double successRate)) 
                        continue;

                    Dictionary<IMaterial, int> materials = new Dictionary<IMaterial, int>();
                    
                    // Parse required materials
                    for (int i = 1; i < lines.Length; i++)
                    {
                        string line = lines[i].Trim();
                        if (string.IsNullOrEmpty(line)) continue;
                        
                        string[] parts = line.Split(',');
                        if (parts.Length != 2) continue;

                        if (int.TryParse(parts[0].Trim(), out int materialId) && 
                            int.TryParse(parts[1].Trim(), out int quantity))
                        {
                            IMaterial material = inventory.GetMaterial(materialId);
                            if (material != null)
                            {
                                materials[material] = quantity;
                            }
                        }
                    }

                    if (materials.Count > 0)
                    {
                        Recipe recipe = new Recipe(recipeName, materials, successRate);
                        recipeList.Add(recipe);
                    }
                }
                catch (Exception)
                {
                    // Skip invalid recipe files
                    continue;
                }
            }

            recipeList.Sort();
        }

        /// <summary>
        /// Attempts to craft an item from a given recipe. Consumes inventory 
        /// materials and returns the result message.
        /// </summary>
        /// <param name="recipeName">Name of the recipe to craft</param>
        /// <returns>A message indicating success, failure, or error</returns>
        public string CraftItem(string recipeName)
        {
            if (recipeList.Count == 0)
            {
                return "No recipes available. Please check your recipe files.";
            }
            
            IRecipe selected = null;

            for (int i = 0; i < recipeList.Count; i++)
            {
                if (recipeList[i].Name.Equals(recipeName,
                        StringComparison.OrdinalIgnoreCase))
                {
                    selected = recipeList[i];
                    break;
                }
            }
            
            if (selected == null)
                return "Recipe not found.";

            // Check if we have enough materials
            foreach (KeyValuePair<IMaterial, int> required in selected.RequiredMaterials)
            {
                IMaterial material = required.Key;
                int need = required.Value;
                int have = inventory.GetQuantity(material);

                if (have < need)
                {
                    if (have == 0)
                    {
                        return "Missing material: " + material.Name;
                    }
                    return "Not enough " + material.Name +
                           " (need " + need +
                           ", have " + have + ")";
                }
            }

            // Consume materials
            foreach (KeyValuePair<IMaterial, int> required in selected.RequiredMaterials)
            {
                if (!inventory.RemoveMaterial(required.Key, required.Value))
                {
                    return "Error: Could not remove materials from inventory";
                }
            }

            // Determine success/failure
            Random rng = new Random();
            if (rng.NextDouble() < selected.SuccessRate)
                return "Crafting '" + selected.Name + "' succeeded!";
            else
                return "Crafting '" + selected.Name + "' failed. Materials lost.";
        }
    }
}