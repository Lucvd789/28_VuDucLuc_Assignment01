using _28_VuDucLuc_Assignment01_BackEnd.DataAccessObjects;
using _28_VuDucLuc_Assignment01_BackEnd.Models;

namespace _28_VuDucLuc_Assignment01_BackEnd.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public List<NewsArticle> GetAll() => NewsArticleDAO.Instance.GetAll();
        public List<NewsArticle> GetActive() => NewsArticleDAO.Instance.GetActive();
        public List<NewsArticle> GetByAuthor(short authorId) => NewsArticleDAO.Instance.GetByAuthor(authorId);
        public NewsArticle? GetById(string id) => NewsArticleDAO.Instance.GetById(id);
        public void Add(NewsArticle article, List<int>? tagIds = null) => NewsArticleDAO.Instance.Add(article, tagIds);
        public void Update(NewsArticle article, List<int>? tagIds = null) => NewsArticleDAO.Instance.Update(article, tagIds);
        public bool Delete(string id) => NewsArticleDAO.Instance.Delete(id);
        public bool Exists(string id) => NewsArticleDAO.Instance.Exists(id);
    }
}
