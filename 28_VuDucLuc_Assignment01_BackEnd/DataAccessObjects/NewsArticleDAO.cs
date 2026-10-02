using _28_VuDucLuc_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _28_VuDucLuc_Assignment01_BackEnd.DataAccessObjects
{
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance;
        private static readonly object _lock = new object();

        private NewsArticleDAO() { }

        public static NewsArticleDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new NewsArticleDAO();
                    return _instance;
                }
            }
        }

        public List<NewsArticle> GetAll()
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .ToList();
        }

        public List<NewsArticle> GetActive()
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Where(n => n.NewsStatus == true)
                .ToList();
        }

        public List<NewsArticle> GetByAuthor(short authorId)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.Tags)
                .Where(n => n.CreatedById == authorId)
                .ToList();
        }

        public NewsArticle? GetById(string id)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .FirstOrDefault(n => n.NewsArticleId == id);
        }

        public void Add(NewsArticle article, List<int>? tagIds = null)
        {
            using var context = new FunewsManagementContext();
            if (tagIds != null && tagIds.Count > 0)
            {
                var tags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                article.Tags = tags;
            }
            context.NewsArticles.Add(article);
            context.SaveChanges();
        }

        public void Update(NewsArticle article, List<int>? tagIds = null)
        {
            using var context = new FunewsManagementContext();
            var existing = context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefault(n => n.NewsArticleId == article.NewsArticleId);
            if (existing == null) return;

            existing.NewsTitle = article.NewsTitle;
            existing.Headline = article.Headline;
            existing.NewsContent = article.NewsContent;
            existing.NewsSource = article.NewsSource;
            existing.CategoryId = article.CategoryId;
            existing.NewsStatus = article.NewsStatus;
            existing.UpdatedById = article.UpdatedById;
            existing.ModifiedDate = article.ModifiedDate;

            if (tagIds != null)
            {
                existing.Tags.Clear();
                var tags = context.Tags.Where(t => tagIds.Contains(t.TagId)).ToList();
                foreach (var tag in tags)
                    existing.Tags.Add(tag);
            }

            context.SaveChanges();
        }

        public bool Delete(string id)
        {
            using var context = new FunewsManagementContext();
            var article = context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefault(n => n.NewsArticleId == id);
            if (article == null) return false;

            article.Tags.Clear();
            context.NewsArticles.Remove(article);
            context.SaveChanges();
            return true;
        }

        public bool Exists(string id)
        {
            using var context = new FunewsManagementContext();
            return context.NewsArticles.Any(n => n.NewsArticleId == id);
        }
    }
}
