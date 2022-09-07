using exercise_5_frontend.Models;
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
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration Configuration;
        private readonly GrpcChannel channel;
        private readonly CategoriesClient client;

        public CategoryModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
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
            Category toEdit = new Category(Name);
            toEdit.Id = ID;
            var temp = JsonSerializer.Serialize(toEdit);
            var res = await HttpClient.PutAsync(Configuration["BaseUrl"] + "categories/" + ID, new StringContent(temp, Encoding.UTF8, "application/json"));
            if ((int)res.StatusCode == 200)
            {
                //ReqResult = "success";
                //Msg = "the category has been updated successfully";
                return Redirect("/Categories?ReqResult=success&Msg=the category has been updated successfully");
            }
            else
            {
                //ReqResult = "failure";
                //Msg = "something went wrong with your request .. review your data and try again";
                //Open = "edit";
                return Redirect("/Categories?ReqResult=failure&Msg=something went wrong with your request .. review your data and try again&name=" + Name + "&id=" + ID + "&open=edit");
            }
        }
        public async Task<IActionResult> OnPostDeleteCategory()
        {
            var res = await HttpClient.DeleteAsync(Configuration["BaseUrl"] + "categories/" + ID);
            if ((int)res.StatusCode == 200)
            {
                //ReqResult = "success";
                //Msg = "the category has been deleted successfully";
                return Redirect("/Categories?ReqResult=success&Msg=the category has been deleted successfully");
            }
            else
            {
                //ReqResult = "failure";
                //Msg = "something went wrong with your request .. you can retry after some seconds";
                return Redirect("/Categories?ReqResult=failure&Msg=something went wrong with your request .. you can retry after some seconds&id=" + ID + "&open=delete");
            }
        }
    }
}