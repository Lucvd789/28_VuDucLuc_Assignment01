using _28_VuDucLuc_Assignment01_BackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace _28_VuDucLuc_Assignment01_BackEnd.DataAccessObjects
{
    public class SystemAccountDAO
    {
        private static SystemAccountDAO? _instance;
        private static readonly object _lock = new object();

        private SystemAccountDAO() { }

        public static SystemAccountDAO Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new SystemAccountDAO();
                    return _instance;
                }
            }
        }

        public List<SystemAccount> GetAll()
        {
            using var context = new FunewsManagementContext();
            return context.SystemAccounts.ToList();
        }

        public SystemAccount? GetById(short id)
        {
            using var context = new FunewsManagementContext();
            return context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);
        }

        public SystemAccount? GetByEmail(string email)
        {
            using var context = new FunewsManagementContext();
            return context.SystemAccounts.FirstOrDefault(a => a.AccountEmail == email);
        }

        public SystemAccount? Login(string email, string password)
        {
            using var context = new FunewsManagementContext();
            return context.SystemAccounts
                .FirstOrDefault(a => a.AccountEmail == email && a.AccountPassword == password);
        }

        public void Add(SystemAccount account)
        {
            using var context = new FunewsManagementContext();
            context.SystemAccounts.Add(account);
            context.SaveChanges();
        }

        public void Update(SystemAccount account)
        {
            using var context = new FunewsManagementContext();
            context.SystemAccounts.Update(account);
            context.SaveChanges();
        }

        public bool Delete(short id)
        {
            using var context = new FunewsManagementContext();
            // Kiểm tra ràng buộc: account đã tạo bài viết nào chưa
            bool hasArticles = context.NewsArticles.Any(n => n.CreatedById == id);
            if (hasArticles) return false;

            var account = context.SystemAccounts.FirstOrDefault(a => a.AccountId == id);
            if (account == null) return false;

            context.SystemAccounts.Remove(account);
            context.SaveChanges();
            return true;
        }

        public bool Exists(short id)
        {
            using var context = new FunewsManagementContext();
            return context.SystemAccounts.Any(a => a.AccountId == id);
        }
    }
}