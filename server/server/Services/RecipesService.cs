using Grpc.Core;
using server;
using static server.Serialization;

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
        //public JsonSerializerOptions Options { get; set; }
        public Serialization Serializer = new();

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
            foreach (var cat in request.Categories)
            {
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
            Serializer.Serialize(this.Recipes, this.RecipesLoc);
            return toAdd;
        }

        public override async Task<Recipe> DeleteRecipe(RecipeToDelete request, ServerCallContext context)
        {
            if (!isLoaded)
            {
                await LoadData();
            }
            Recipe toDelete = Recipes.Single(x => x.Id == request.Id);
            Recipes.Remove(toDelete);
            Serializer.Serialize(this.Recipes, this.RecipesLoc);
            return toDelete;
        }

        public override async Task<Recipe> EditRecipe(Recipe request, ServerCallContext context)
        {
            if (!isLoaded)
            {
                await LoadData();
            }
            Recipe toEdit = Recipes.Single(x => x.Id == request.Id);
            toEdit.Categories.Clear();
            foreach (var cat in request.Categories)
            {
                toEdit.Categories.Add(cat);
            }
            toEdit.Instructions.Clear();
            foreach (var ins in request.Instructions)
            {
                toEdit.Instructions.Add(ins);
            }
            toEdit.Ingredients.Clear();
            foreach (var ing in request.Ingredients)
            {
                toEdit.Ingredients.Add(ing);
            }
            Serializer.Serialize(this.Recipes, this.RecipesLoc);
            return toEdit;
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

            string mainPath = Environment.CurrentDirectory;

            //if (app.Environment.IsDevelopment())
            //{
            this.CategoriesLoc = $@"{mainPath}\..\categories.json";
            //}
            //else
            //{
            //    this.CategoriesLoc = $@"{mainPath}\categories.json";
            //}
            this.Categories = (List<Category>)Serializer.Deserialize(this.CategoriesLoc, SerializationType.ListOfCategories);
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
            this.Recipes = (List<Recipe>)Serializer.Deserialize(this.RecipesLoc, SerializationType.ListOfRecipes);

            isLoaded = true;
        }
    }
}