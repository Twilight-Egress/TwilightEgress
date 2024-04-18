﻿using CalamityMod.Items.Accessories;
using Terraria.ModLoader.Exceptions;

namespace Cascade.Core.Systems
{
    public class RecipeSystem : ModSystem
    {
        private readonly List<int> RecipesToBeDisabled = new()
        {
            ModContent.ItemType<PlagueHive>()
        };

        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                // Disabling recipes.
                foreach (int result in RecipesToBeDisabled)
                {
                    if (recipe.HasResult(result) && recipe.Mod == Cascade.Instance.CalamityMod)
                        recipe.DisableRecipe();
                }
            }
        } 
    }
}
