using _28_VuDucLuc_Assignment01_BackEnd.Models;

namespace _28_VuDucLuc_Assignment01_BackEnd.Repositories
{
    public interface ISystemAccountRepository
    {
        List<SystemAccount> GetAll();
        SystemAccount? GetById(short id);
        SystemAccount? GetByEmail(string email);
        SystemAccount? Login(string email, string password);
        void Add(SystemAccount account);
        void Update(SystemAccount account);
        bool Delete(short id);
        bool Exists(short id);
    }
}
