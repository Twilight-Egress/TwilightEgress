using CalamityMod.Items.Accessories;

namespace TwilightEgress.Core.Systems
{
    public class RecipeSystem : ModSystem
    {
        public override void PostAddRecipes()
        {
            /*for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];
                // Disabling recipes.
                foreach (int result in RecipesToBeDisabled)
                {
                    if (recipe.HasResult(result) && recipe.Mod is CalamityMod.CalamityMod)
                        recipe.DisableRecipe();
                }
            }*/
        } 
    }
}
