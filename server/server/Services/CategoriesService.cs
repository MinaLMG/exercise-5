using Grpc.Core;
using Newtonsoft.Json;

namespace server.Services
{
    public class CategoriesService : Categories.CategoriesBase
    {
        public List<Category> Categories { get; set; } = new();
        public List<Recipe> Recipes { get; set; } = new();

        //public List<server.Recipe> Recipes { get; set; } = new();
        public Dictionary<string, Guid> CategoriesMap { get; set; }
        public Dictionary<Guid, string> CategoriesNamesMap { get; set; }
        //public string RecipesLoc { get; set; }
        public string CategoriesLoc { get; set; }
        public string RecipesLoc { get; set; }
        private bool isLoaded = false;
        //public JsonSerializerOptions Options { get; set; }
        public Serialization Serializer=new();
        private readonly ILogger<CategoriesService> _logger;
        public CategoriesService(ILogger<CategoriesService> logger)
        {
            _logger = logger;
        }
        public override async Task<CategoriesList> ListCategories(VoidCategory request, ServerCallContext context)
        {
            //string mainPath = Environment.CurrentDirectory;
            //this.CategoriesLoc = $@"{mainPath}\..\categories.json";
            //Category cs =new Category();
            //cs.Id = "5fb8915e-3819-4ae1-97f7-b7ed11e828a0";
            //cs.Name = "dsaijkld";
            //this.Categories.Add(cs)  ;
            //Serializer.Serialize(this.Categories, this.CategoriesLoc);
            if (!isLoaded)
            {
                await LoadData();
            }
            CategoriesList response = new();
            Categories.ForEach(cat =>
            {
                response.Categories.Add(cat);
            });
            return response;
        }

        public override async Task<Category> CreateCategory(CategoryToAdd request, ServerCallContext context)
        {
            if (!isLoaded)
            {
                await LoadData();
            }
            Category toAdd = new Category();
            toAdd.Name = request.Name;
            toAdd.Id = Guid.NewGuid().ToString();
            Categories.Add(toAdd);
            this.CategoriesMap[toAdd.Name] = Guid.Parse(toAdd.Id);
            this.CategoriesNamesMap[Guid.Parse(toAdd.Id)] = toAdd.Name;
            Serializer.Serialize(this.Categories, this.CategoriesLoc);
            //this.WriteInFolder(JsonSerializer.Serialize(this.Categories, this.Options), this.CategoriesLoc);
            return toAdd;
        }

        public override async Task<Category> DeleteCategory(CategoryToDelete request, ServerCallContext context)
        {
            if (!isLoaded)
            {
                await LoadData();
            }
            Category toDelete = Categories.Single(x => x.Id == request.Id);
            CategoriesMap.Remove(toDelete.Name);
            CategoriesMap.Remove(toDelete.Id);
            Categories.Remove(toDelete);
            Serializer.Serialize(this.Categories, this.CategoriesLoc);
            //remove category from recipes
            Recipes.ForEach(rec =>
            {
                try
                {
                    string toRemove = rec.Categories.Single(C => C == request.Id);
                    rec.Categories.Remove(toRemove);
                }
                catch { }
            });
            Serializer.Serialize(this.Recipes, this.RecipesLoc);
            return toDelete;
        }

        public override async Task<Category> EditCategory(Category request, ServerCallContext context)
        {
            if (!isLoaded)
            {
                await LoadData();
            }
            Category toEdit = Categories.Single(x => x.Id == request.Id);
            CategoriesMap.Remove(toEdit.Name);
            toEdit.Name = request.Name;
            Serializer.Serialize(this.Categories, this.CategoriesLoc);
            this.CategoriesMap[toEdit.Name] = Guid.Parse(toEdit.Id);
            this.CategoriesNamesMap[Guid.Parse(toEdit.Id)] = toEdit.Name;
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
             
            this.Categories =(List<Category>)Serializer.Deserialize(this.CategoriesLoc,"cat");
            //foreach (var cat in x)
            //{
            //    this.Categories.Add(cat);
            //}           
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
            this.Recipes = (List<Recipe>)Serializer.Deserialize(this.RecipesLoc, "rec");

            isLoaded = true;
        }
    }
}