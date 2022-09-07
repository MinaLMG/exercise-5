using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;
using static server.Categories;

namespace exercise_5_frontend.Pages
{
    public class CategoryModel : PageModel
    {
        public HttpClient HttpClient = new();
        public List<server.Category> Categories = new();
        [BindProperty(SupportsGet = true)]
        public string ReqResult { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Msg { get; set; }
        [BindProperty(SupportsGet = true)]
        public Guid ID { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Name { get; set; }
        [BindProperty(SupportsGet = true)]
        public string Open { get; set; }
        private readonly ILogger<IndexModel> logger;
        private readonly IConfiguration configuration;
        private readonly GrpcChannel channel;
        private readonly CategoriesClient client;

        public CategoryModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            logger = logger;
            configuration = configuration;
            channel = GrpcChannel.ForAddress("https://localhost:5500");
            client = new CategoriesClient(channel);

        }

        public async Task OnGet()
        {
            await ListCategories();
        }
        public async Task ListCategories()
        {
            var reply = await client.ListCategoriesAsync(new server.VoidCategory { });
            foreach (var category in reply.Categories)
            {
                this.Categories.Add(category);
            }
        }
        public async Task<IActionResult> OnPostAddCategory()
        {
            var reply = await client.CreateCategoryAsync(new server.CategoryToAdd { Name = Name });
            return Redirect("/Categories?ReqResult=success&Msg=your category has been added successfully");

        }
        public async Task<IActionResult> OnPostUpdateCategory()
        {
            server.Category toEdit = new ();
            toEdit.Id = ID.ToString();
            toEdit.Name = Name;
            var reply = await client.EditCategoryAsync(toEdit);
            return Redirect("/Categories?ReqResult=success&Msg=the category has been updated successfully");
        }
        public async Task<IActionResult> OnPostDeleteCategory()
        {
            server.CategoryToDelete toDelete = new();
            toDelete.Id = ID.ToString();
            var reply = await client.DeleteCategoryAsync(toDelete);
            return Redirect("/Categories?ReqResult=success&Msg=the category has been deleted successfully");
           
        }
    }
}