using _28_VuDucLuc_Assignment01_BackEnd.DataAccessObjects;
using _28_VuDucLuc_Assignment01_BackEnd.Models;

namespace _28_VuDucLuc_Assignment01_BackEnd.Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        public List<SystemAccount> GetAll() => SystemAccountDAO.Instance.GetAll();
        public SystemAccount? GetById(short id) => SystemAccountDAO.Instance.GetById(id);
        public SystemAccount? GetByEmail(string email) => SystemAccountDAO.Instance.GetByEmail(email);
        public SystemAccount? Login(string email, string password) => SystemAccountDAO.Instance.Login(email, password);
        public void Add(SystemAccount account) => SystemAccountDAO.Instance.Add(account);
        public void Update(SystemAccount account) => SystemAccountDAO.Instance.Update(account);
        public bool Delete(short id) => SystemAccountDAO.Instance.Delete(id);
        public bool Exists(short id) => SystemAccountDAO.Instance.Exists(id);
    }
}
