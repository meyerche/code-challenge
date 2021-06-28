using System;
using System.Threading.Tasks;
using challenge.Models;

namespace challenge.Repositories
{
    public interface ICompensationRepository
    {
        Compensation GetById(String id);
        Compensation Add(Compensation compensation);
        Task SaveAsync();
    }
}