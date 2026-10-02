using _28_VuDucLuc_Assignment01_BackEnd.DataAccessObjects;
using _28_VuDucLuc_Assignment01_BackEnd.Models;

namespace _28_VuDucLuc_Assignment01_BackEnd.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public List<Category> GetAll() => CategoryDAO.Instance.GetAll();
        public Category? GetById(short id) => CategoryDAO.Instance.GetById(id);
        public void Add(Category category) => CategoryDAO.Instance.Add(category);
        public void Update(Category category) => CategoryDAO.Instance.Update(category);
        public bool Delete(short id) => CategoryDAO.Instance.Delete(id);
        public bool Exists(short id) => CategoryDAO.Instance.Exists(id);
    }
}
