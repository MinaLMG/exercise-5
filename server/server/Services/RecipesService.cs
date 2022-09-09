using Grpc.Core;
using server;
using System.Text.Json;

namespace server.Services
{
    public class RecipesService : Recipes.RecipesBase
    {
        public List<Category> Categories { get; set; } = new();
        public List<Recipe> Recipes { get; set; } = new();
        public Dictionary<string, Guid> CategoriesMap { get; set; }
        public Dictionary<Guid, string> CategoriesNamesMap { get; set; }
        public string RecipesLoc { get; set; }
        public string CategoriesLoc { get; set; }
        private bool isLoaded = false;
        public JsonSerializerOptions Options { get; set; }

        private readonly ILogger<RecipesService> _logger;
        public RecipesService(ILogger<RecipesService> logger)
        {
            _logger = logger;
        }

        public override async Task<RecipesList> ListRecipes(VoidRecipe request, ServerCallContext context)
        {

            if (!isLoaded)
            {
                await LoadData();
            }
            RecipesList response = new();
            Recipes.ForEach(rec =>
            {
                response.Recipes.Add(rec);
            });
            return response;
        }
        public override async Task<Recipe> CreateRecipe(RecipeToAdd request, ServerCallContext context)
        {
            if (!isLoaded)
            {
                await LoadData();
            }
            Recipe toAdd = new();
            toAdd.Id = Guid.NewGuid().ToString();
            toAdd.Title = request.Title.Trim();
            foreach (var cat in request.Categories) {
                toAdd.Categories.Add(cat);
            }
            foreach (var ins in request.Instructions)
            {
                toAdd.Instructions.Add(ins);
            }
            foreach (var ing in request.Ingredients)
            {
                toAdd.Ingredients.Add(ing);
            }
            Recipes.Add(toAdd);
            this.WriteInFolder(JsonSerializer.Serialize(this.Recipes, new JsonSerializerOptions { WriteIndented = true }), this.RecipesLoc);
            return toAdd;
        }

        public override Task<Recipe> DeleteRecipe(RecipeToDelete request, ServerCallContext context)
        {
            return base.DeleteRecipe(request, context);
        }

        public override Task<Recipe> EditRecipe(Recipe request, ServerCallContext context)
        {
            return base.EditRecipe(request, context);
        }
        public void WriteInFolder(string text, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.WriteLine(text);
            }
        }
        public async Task LoadData()
        {

            this.Options = new JsonSerializerOptions { WriteIndented = true };
            string mainPath = Environment.CurrentDirectory;

            //if (app.Environment.IsDevelopment())
            //{
            this.CategoriesLoc = $@"{mainPath}\..\categories.json";
            //}
            //else
            //{
            //    this.CategoriesLoc = $@"{mainPath}\categories.json";
            //}
            string categoriesString = File.ReadAllText(this.CategoriesLoc);
            this.Categories = JsonSerializer.Deserialize<List<server.Category>>(categoriesString);
            /****/
            this.CategoriesMap = new Dictionary<string, Guid>();
            this.CategoriesNamesMap = new Dictionary<Guid, string>();
            for (int i = 0; i < this.Categories.Count; i++)
            {
                this.CategoriesMap[this.Categories[i].Name] = Guid.Parse(this.Categories[i].Id);
                this.CategoriesNamesMap[Guid.Parse(this.Categories[i].Id)] = this.Categories[i].Name;
            }
            //if (app.Environment.IsDevelopment())
            //{
            this.RecipesLoc = $@"{mainPath}\..\recipes.json";
            //}
            //else
            //{
            //    this.RecipesLoc = $@"{mainPath}\recipes.json";
            //}
            string recipesString = File.ReadAllText(this.RecipesLoc);
            this.Recipes = JsonSerializer.Deserialize<List<server.Recipe>>(recipesString);

            isLoaded = true;
        }
    }
}