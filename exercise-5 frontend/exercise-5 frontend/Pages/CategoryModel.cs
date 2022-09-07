using exercise_5_frontend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text;
using System.Text.Json;

namespace exercise_5_frontend.Pages
{
    public class CategoryModel : PageModel
    {
        public HttpClient HttpClient = new();
        public List<Category> Categories = new();
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
        public CategoryModel(ILogger<IndexModel> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        public async Task OnGet()
        {
            await ListCategories();
        }
        public async Task ListCategories()
        {
            var res = await HttpClient.GetAsync(Configuration["BaseUrl"] + "categories");
            var serializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };
            var inBetween = res.Content.ReadAsStringAsync().Result;
            List<Category> categories = JsonSerializer.Deserialize<List<Category>>(inBetween, serializeOptions);
            this.Categories = categories;
        }
        public async Task<IActionResult> OnPostAddCategory()
        {
            Category toAdd = new Category(Name);
            var temp = JsonSerializer.Serialize(toAdd);
            var res = await HttpClient.PostAsync(Configuration["BaseUrl"] + "categories", new StringContent(temp, Encoding.UTF8, "application/json"));
            if ((int)res.StatusCode == 200)
            {
                //ReqResult = "success";
                //Msg = "your category has been added successfully";
                return Redirect("/Categories?ReqResult=success&Msg=your category has been added successfully");
            }
            else
            {
                //ReqResult = "failure";
                //Msg = "something went wrong with your request .. check your data and try again";
                return Redirect("/Categories?ReqResult=failure&Msg=something went wrong with your request .. check your data and try again&name=" + Name);
            }
        }
        public async Task<IActionResult> OnPostUpdateCategory()
        {
            Category toEdit = new Category(Name);
            toEdit.ID = ID;
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