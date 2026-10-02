using _28_VuDucLuc_Assignment01_BackEnd.Models;

namespace _28_VuDucLuc_Assignment01_BackEnd.Repositories
{
    public interface ICategoryRepository
    {
        List<Category> GetAll();
        Category? GetById(short id);
        void Add(Category category);
        void Update(Category category);
        bool Delete(short id);
        bool Exists(short id);
    }
}
