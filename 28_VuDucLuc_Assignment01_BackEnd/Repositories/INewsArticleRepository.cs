using _28_VuDucLuc_Assignment01_BackEnd.Models;

namespace _28_VuDucLuc_Assignment01_BackEnd.Repositories
{
    public interface INewsArticleRepository
    {
        List<NewsArticle> GetAll();
        List<NewsArticle> GetActive();
        List<NewsArticle> GetByAuthor(short authorId);
        NewsArticle? GetById(string id);
        void Add(NewsArticle article, List<int>? tagIds = null);
        void Update(NewsArticle article, List<int>? tagIds = null);
        bool Delete(string id);
        bool Exists(string id);
    }
}
