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
        public Serialization Serializer = new();
        private readonly ILogger<RecipesService> _logger;
        public RecipesService(ILogger<RecipesService> logger)
        {
            _logger = logger;
        }

        public override async Task<RecipesList> ListRecipes(VoidRecipe request, ServerCallContext context)
        {
            try
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
            catch
            {
                throw new RpcException(new Status(StatusCode.Internal, "something went wrong!"));
            }
        }
        public override async Task<Recipe> CreateRecipe(RecipeToAdd request, ServerCallContext context)
        {
            try
            {
                if (!isLoaded)
                {
                    await LoadData();
                }
                Recipe toAdd = new();
                toAdd.Id = Guid.NewGuid().ToString();
                toAdd.Title = request.Title.Trim();
                if (toAdd.Title == "")
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe title!"));
                }
                foreach (var cat in request.Categories)
                {
                    toAdd.Categories.Add(cat);
                }
                if (toAdd.Categories.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe categories!"));
                }
                foreach (var ins in request.Instructions)
                {
                    if (ins.Trim() != "")
                        toAdd.Instructions.Add(ins);
                }

                if (toAdd.Instructions.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe instructions!"));
                }
                foreach (var ing in request.Ingredients)
                {
                    if (ing.Trim() != "")
                        toAdd.Ingredients.Add(ing);
                }

                if (toAdd.Ingredients.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe ingredients!"));
                }
                Recipes.Add(toAdd);
                Serializer.Serialize(this.Recipes, this.RecipesLoc);
                return toAdd;
            }
            catch
            {
                throw new RpcException(new Status(StatusCode.Internal, "something went wrong!"));
            }
        }

        public override async Task<Recipe> DeleteRecipe(RecipeToDelete request, ServerCallContext context)
        {
            try
            {
                if (!isLoaded)
                {
                    await LoadData();
                }
                Recipe toDelete;
                try
                {
                    toDelete = Recipes.Single(x => x.Id == request.Id);
                }
                catch
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "no recipe exists with this ID!"));
                }
                Recipes.Remove(toDelete);
                Serializer.Serialize(this.Recipes, this.RecipesLoc);
                return toDelete;
            }
            catch
            {
                throw new RpcException(new Status(StatusCode.Internal, "something went wrong!"));
            }
        }

        public override async Task<Recipe> EditRecipe(Recipe request, ServerCallContext context)
        {
            try
            {
                if (!isLoaded)
                {
                    await LoadData();
                }
                Recipe toEdit;
                try
                {
                    toEdit = Recipes.Single(x => x.Id == request.Id);
                }
                catch
                {
                    throw new RpcException(new Status(StatusCode.NotFound, "no recipe exists with this ID!"));
                }
                toEdit.Title = request.Title.Trim();
                if (toEdit.Title == "")
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe title!"));
                }
                toEdit.Categories.Clear();
                foreach (var cat in request.Categories)
                {
                    toEdit.Categories.Add(cat);
                }
                if (toEdit.Categories.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe categories!"));
                }
                toEdit.Instructions.Clear();
                foreach (var ins in request.Instructions)
                {
                    if (ins.Trim() != "")
                        toEdit.Instructions.Add(ins);
                }
                if (toEdit.Instructions.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe isntructions!"));
                }
                toEdit.Ingredients.Clear();
                foreach (var ing in request.Ingredients)
                {
                    if (ing.Trim() != "")
                        toEdit.Ingredients.Add(ing);
                }
                if (toEdit.Ingredients.Count == 0)
                {
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "no value for recipe ingredients!"));
                }
                Serializer.Serialize(this.Recipes, this.RecipesLoc);
                return toEdit;
            }
            catch
            {
                throw new RpcException(new Status(StatusCode.Internal, "something went wrong!"));
            }
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
            this.CategoriesLoc = $@"{mainPath}\categories.json";
            this.Categories = (List<Category>)Serializer.Deserialize(this.CategoriesLoc, SerializationType.ListOfCategories);
            /****/
            this.CategoriesMap = new Dictionary<string, Guid>();
            this.CategoriesNamesMap = new Dictionary<Guid, string>();
            for (int i = 0; i < this.Categories.Count; i++)
            {
                this.CategoriesMap[this.Categories[i].Name] = Guid.Parse(this.Categories[i].Id);
                this.CategoriesNamesMap[Guid.Parse(this.Categories[i].Id)] = this.Categories[i].Name;
            }
            this.RecipesLoc = $@"{mainPath}\recipes.json";
            string recipesString = File.ReadAllText(this.RecipesLoc);
            this.Recipes = (List<Recipe>)Serializer.Deserialize(this.RecipesLoc, SerializationType.ListOfRecipes);
            isLoaded = true;
        }
    }
}