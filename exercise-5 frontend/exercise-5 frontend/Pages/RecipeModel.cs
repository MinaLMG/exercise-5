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
             var reply2 = await recipesClient.ListRecipesAsync(new server.VoidRecipe { });
            foreach (var recipe in reply2.Recipes)
            {
                this.Recipes.Add(recipe);
            }
        }
        public async Task<IActionResult> OnPostCreateRecipe()
        {
            //Recipe toAdd = new Recipe("", new(), new(), new());

            //toAdd.Title = Title.Trim();
            //foreach (Guid category in Categories)
            //{
            //    toAdd.Categories.Add(category);
            //}

            //toAdd.Instructions = new();
            //// using the method
            //String[] strlist = Instructions.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            //foreach (String s in strlist)
            //{
            //    if (s.Trim() != "")
            //    {
            //        toAdd.Instructions.Add(s.Trim());
            //    }
            //}
            //strlist = Ingredients.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            //foreach (String s in strlist)
            //{
            //    if (s.Trim() != "")
            //        toAdd.Ingredients.Add(s.Trim());
            //}


            //var temp = JsonSerializer.Serialize(toAdd);
            var res = await HttpClient.PostAsync(Configuration["BaseUrl"] + "recipes", new StringContent("", Encoding.UTF8, "application/json"));
            if ((int)res.StatusCode == 200)
                return Redirect("/recipes?ReqResult=success&Msg=your recipe has been added successfully");
            else
                return RedirectToPage("/recipes", new { ReqResult = "failure", Msg = "something went wrong with your request .. check your data and try again", open = "add", title = Title, instructions = Instructions, ingredients = Ingredients, categories = Categories });
        }
        public async Task<IActionResult> OnPostUpdateRecipe()
        {
            //Recipe toEdit = new Recipe("", new(), new(), new());
            //toEdit.ID = ID;
            //toEdit.Title = Title.Trim();
            //foreach (Guid category in Categories)
            //{
            //    toEdit.Categories.Add(category);
            //}

            //toEdit.Instructions = new();
            //// using the method
            //Instructions = Instructions.Trim();
            //String[] strlist = Instructions.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            //foreach (String s in strlist)
            //{
            //    if (s.Trim() != "")
            //        toEdit.Instructions.Add(s.Trim());
            //}
            //Ingredients = Ingredients.Trim();
            //strlist = Ingredients.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            //foreach (String s in strlist)
            //{
            //    if (s.Trim() != "")
            //        toEdit.Ingredients.Add(s.Trim());
            //}


            //var temp = JsonSerializer.Serialize(toEdit);
            var res = await HttpClient.PutAsync(Configuration["BaseUrl"] + "recipes/" + ID, new StringContent("", Encoding.UTF8, "application/json"));
            if ((int)res.StatusCode == 200)
                return Redirect("/recipes?ReqResult=success&Msg=the recipe has been updated successfully");
            else
                return RedirectToPage("/recipes", new { ReqResult = "failure", Msg = "something went wrong with your request .. review your data and try again", open = "edit", title = Title, id = ID, instructions = Instructions, ingredients = Ingredients, categories = Categories }); /*+ "&instructions=" + Instructions + "&ingredients=" + Ingredients);*/
        }
        public async Task<IActionResult> OnPostDeleteRecipe()
        {
            var res = await HttpClient.DeleteAsync(Configuration["BaseUrl"] + "recipes/" + ID);
            if ((int)res.StatusCode == 200)
                return Redirect("/recipes?ReqResult=success&Msg=the recipe has been deleted successfully");
            else
                return RedirectToPage("/recipes",
                    new
                    {
                        ReqResult = "failure",
                        Msg = "something went wrong with your request .. you can retry after some seconds",
                        open = "delete",
                        id = ID,
                    });
        }
    }
}