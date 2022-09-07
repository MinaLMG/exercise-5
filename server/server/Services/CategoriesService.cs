using Grpc.Core;
using server;

namespace server.Services
{
    public class CategoriesService : Categories.CategoriesBase
    {
        private readonly ILogger<CategoriesService> _logger;
        public CategoriesService(ILogger<CategoriesService> logger)
        {
            _logger = logger;
        }
        public override Task<CategoriesList> ListCategories(VoidCategory request, ServerCallContext context)
        {
            return base.ListCategories(request, context);
        }

        public override Task<Category> CreateCategory(CategoryToAdd request, ServerCallContext context)
        {
            return base.CreateCategory(request, context);
        }

        public override Task<Category> DeleteCategory(CategoryToDelete request, ServerCallContext context)
        {
            return base.DeleteCategory(request, context);
        }

        public override Task<Category> EditCategory(Category request, ServerCallContext context)
        {
            return base.EditCategory(request, context);
        }
    }
}