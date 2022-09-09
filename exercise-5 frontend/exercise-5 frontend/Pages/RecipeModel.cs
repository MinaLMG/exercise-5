using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using server;
using System.Text;
using System.Text.Json;
using static server.Categories;
using static server.Recipes;

namespace exercise_5_frontend.Pages
{
    public class RecipeModel : PageModel
    {
        public HttpClient HttpClient = new();
        public List<Recipe> Recipes = new();
        public List<Category> CategoriesList = new();
        public Dictionary<Guid, string> categoriesNamesMap = new Dictionary<Guid, string>();

        [BindProperty(SupportsGet = true)]
        public string ReqResult { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Msg { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid ID { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Title { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Ingredients { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Instructions { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Message { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Open { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid[] Categories { get; set; } = new Guid[0];

        private readonly ILogger<IndexModel> _logger;
        private readonly GrpcChannel channel;
        private readonly CategoriesClient categoriesClient;
        private readonly RecipesClient recipesClient;
        private readonly IConfiguration Configuration;
        public RecipeModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            Configuration = configuration;
            _logger = logger;
            channel = GrpcChannel.ForAddress("https://localhost:5500");
            categoriesClient = new CategoriesClient(channel);
            recipesClient = new RecipesClient(channel);
        }

        public async Task OnGet()
        {
            await ListItems();
        }
        public async Task ListItems()
        {
            var reply = await categoriesClient.ListCategoriesAsync(new server.VoidCategory { });
            foreach (var category in reply.Categories)
            {
                this.CategoriesList.Add(category);
            }
            /* storing some dictionaries*/
            //Dictionary<string, Guid> categoriesMap = new Dictionary<string, Guid>();
            for (int i = 0; i < CategoriesList.Count; i++)
            {
                //categoriesMap[categories[i].Name] = categories[i].ID;
                this.categoriesNamesMap[Guid.Parse(CategoriesList[i].Id)] = CategoriesList[i].Name;
            }
            /**** getting recipes ****/
            var reply2 = await recipesClient.ListRecipesAsync(new server.VoidRecipe { });
            foreach (var recipe in reply2.Recipes)
            {
                this.Recipes.Add(recipe);
            }
        }
        public async Task<IActionResult> OnPostCreateRecipe()
        {

            RecipeToAdd toAdd = new();
            toAdd.Title = Title.Trim();

            foreach (Guid category in Categories)
            {
                toAdd.Categories.Add(category.ToString());
            }

            // using the method
            String[] strlist = Instructions.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (String s in strlist)
            {
                if (s.Trim() != "")
                {
                    toAdd.Instructions.Add(s.Trim());
                }
            }

            strlist = Ingredients.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (String s in strlist)
            {
                if (s.Trim() != "")
                    toAdd.Ingredients.Add(s.Trim());
            }
            var reply = await recipesClient.CreateRecipeAsync(toAdd);
            return Redirect("/recipes?ReqResult=success&Msg=your recipe has been added successfully");
        }
        public async Task<IActionResult> OnPostUpdateRecipe()
        {
            Recipe toEdit = new Recipe();
            toEdit.Id = ID.ToString();
            toEdit.Title = Title.Trim();
            foreach (Guid category in Categories)
            {
                toEdit.Categories.Add(category.ToString());
            }
            Instructions = Instructions.Trim();
            String[] strlist = Instructions.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (String s in strlist)
            {
                if (s.Trim() != "")
                    toEdit.Instructions.Add(s.Trim());
            }
            Ingredients = Ingredients.Trim();
            strlist = Ingredients.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            foreach (String s in strlist)
            {
                if (s.Trim() != "")
                    toEdit.Ingredients.Add(s.Trim());
            }
            var reply= await recipesClient.EditRecipeAsync(toEdit);
            return Redirect("/recipes?ReqResult=success&Msg=the recipe has been updated successfully");
        }
        public async Task<IActionResult> OnPostDeleteRecipe()
        {
            var reply =await recipesClient.DeleteRecipeAsync(new RecipeToDelete { Id = ID.ToString() });
            return Redirect("/recipes?ReqResult=success&Msg=the recipe has been deleted successfully");

        }
    }
}