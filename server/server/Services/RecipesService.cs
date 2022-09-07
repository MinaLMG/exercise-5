using Grpc.Core;
using server;

namespace server.Services
{
    public class RecipesService : Recipes.RecipesBase
    {
        private readonly ILogger<RecipesService> _logger;
        public RecipesService(ILogger<RecipesService> logger)
        {
            _logger = logger;
        }

        public override Task<RecipesList> ListRecipes(VoidRecipe request, ServerCallContext context)
        {
            return base.ListRecipes(request, context);
        }
        public override Task<Recipe> CreateRecipe(RecipeToAdd request, ServerCallContext context)
        {
            return base.CreateRecipe(request, context);
        }
        public override Task<Recipe> DeleteRecipe(RecipeToDelete request, ServerCallContext context)
        {
            return base.DeleteRecipe(request, context);
        }

        public override Task<Recipe> EditRecipe(Recipe request, ServerCallContext context)
        {
            return base.EditRecipe(request, context);
        }
    }
}