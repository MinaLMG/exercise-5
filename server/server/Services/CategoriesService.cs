using Grpc.Core;
using System.Text.Json;
namespace server.Services
{
    public class CategoriesService : Categories.CategoriesBase
    {
        public List<Category> Categories { get; set; } = new();
        //public List<server.Recipe> Recipes { get; set; } = new();
        public Dictionary<string, Guid> CategoriesMap { get; set; }
        public Dictionary<Guid, string> CategoriesNamesMap { get; set; }
        //public string RecipesLoc { get; set; }
        public string CategoriesLoc { get; set; }
        private bool isLoaded = false;
        public JsonSerializerOptions Options { get; set; }

        private readonly ILogger<CategoriesService> _logger;
        public CategoriesService(ILogger<CategoriesService> logger)
        {
            _logger = logger;
        }
        public override async Task<CategoriesList> ListCategories(VoidCategory request, ServerCallContext context)
        {
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
            this.WriteInFolder(JsonSerializer.Serialize(this.Categories, this.Options), this.CategoriesLoc);
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
            this.WriteInFolder(JsonSerializer.Serialize(this.Categories, this.Options), this.CategoriesLoc);
            //To Do : remove category from recipes
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
            this.WriteInFolder(JsonSerializer.Serialize(this.Categories, this.Options), this.CategoriesLoc);
            this.CategoriesMap[toEdit.Name] = Guid.Parse(toEdit.Id);
            this.CategoriesNamesMap[Guid.Parse(toEdit.Id)] = toEdit.Name;
            //To Do : remove category from recipes
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
            isLoaded = true;
        }
    }
}