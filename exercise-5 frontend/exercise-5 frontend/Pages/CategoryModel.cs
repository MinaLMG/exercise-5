using Grpc.Net.Client;
using Grpc.Net.Client.Web;
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

        public CategoryModel(ILogger<IndexModel> logger, IConfiguration configuration, CategoriesClient cc)
        {
            logger = logger;
            configuration = configuration;
            //channel = GrpcChannel.ForAddress(configuration["BaseUrl"], new GrpcChannelOptions
            //{
            //    HttpHandler = new GrpcWebHandler(new HttpClientHandler())
            //});
            client = cc;

        }

        public async Task OnGet()
        {
            await ListCategories();
        }
        public async Task ListCategories()
        {
            try
            {
                var reply = await client.ListCategoriesAsync(new server.VoidCategory { });
                foreach (var category in reply.Categories)
                {
                    this.Categories.Add(category);
                }
            }
            catch
            {

            }
        }
        public async Task<IActionResult> OnPostAddCategory()
        {
            try
            {
                var reply = await client.CreateCategoryAsync(new server.CategoryToAdd { Name = Name });
                return Redirect("/Categories?ReqResult=success&Msg=your category has been added successfully");
            }
            catch (Exception e)
            {
                return Redirect("/Categories?ReqResult=failure&Msg=something went wrong with your request .. check your data and try again&name=" + Name);
            }
        }
        public async Task<IActionResult> OnPostUpdateCategory()
        {
            try
            {
                server.Category toEdit = new();
                toEdit.Id = ID.ToString();
                toEdit.Name = Name;
                var reply = await client.EditCategoryAsync(toEdit);
                return Redirect("/Categories?ReqResult=success&Msg=the category has been updated successfully");
            }
            catch (Exception e)
            {
                return Redirect("/Categories?ReqResult=failure&Msg=something went wrong with your request .. review your data and try again&name=" + Name + "&id=" + ID + "&open=edit");

            }
        }
        public async Task<IActionResult> OnPostDeleteCategory()
        {
            try
            {
                server.CategoryToDelete toDelete = new();
                toDelete.Id = ID.ToString();
                var reply = await client.DeleteCategoryAsync(toDelete);
                return Redirect("/Categories?ReqResult=success&Msg=the category has been deleted successfully");
            }
            catch (Exception e)
            {
                return Redirect("/Categories?ReqResult=failure&Msg=something went wrong with your request .. you can retry after some seconds&id=" + ID + "&open=delete");
            }
        }
    }
}