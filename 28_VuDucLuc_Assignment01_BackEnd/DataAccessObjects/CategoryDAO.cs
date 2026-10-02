using _28_VuDucLuc_Assignment01_BackEnd.Models;

namespace _28_VuDucLuc_Assignment01_BackEnd.DataAccessObjects
{
    public class CategoryDAO
    {
        private static CategoryDAO? _instance;
        private static readonly object _lock = new object();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new CategoryDAO();
                    return _instance;
                }
            }
        }

        public List<Category> GetAll()
        {
            using var context = new FunewsManagementContext();
            return context.Categories.ToList();
        }

        public Category? GetById(short id)
        {
            using var context = new FunewsManagementContext();
            return context.Categories.FirstOrDefault(c => c.CategoryId == id);
        }

        public void Add(Category category)
        {
            using var context = new FunewsManagementContext();
            context.Categories.Add(category);
            context.SaveChanges();
        }

        public void Update(Category category)
        {
            using var context = new FunewsManagementContext();
            context.Categories.Update(category);
            context.SaveChanges();
        }

        public bool Delete(short id)
        {
            using var context = new FunewsManagementContext();
            // Kiểm tra ràng buộc: category đang được dùng trong NewsArticle
            bool isUsed = context.NewsArticles.Any(n => n.CategoryId == id);
            if (isUsed) return false;

            var category = context.Categories.FirstOrDefault(c => c.CategoryId == id);
            if (category == null) return false;

            context.Categories.Remove(category);
            context.SaveChanges();
            return true;
        }

        public bool Exists(short id)
        {
            using var context = new FunewsManagementContext();
            return context.Categories.Any(c => c.CategoryId == id);
        }
    }
}
