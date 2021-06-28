using System;
using challenge.Models;

namespace challenge.Services
{
    public interface ICompensationService
    {
        Compensation GetById(string id);
        Compensation Create(Compensation compensation);
    }
}