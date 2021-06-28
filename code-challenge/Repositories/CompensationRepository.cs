using System.Linq;
using System.Threading.Tasks;
using challenge.Data;
using challenge.Models;
using Microsoft.Extensions.Logging;

namespace challenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, EmployeeContext employeeContext)
        {
            _employeeContext = employeeContext;
            _logger = logger;
        }

        public Compensation GetById(string id)
        {
            return _employeeContext.EmployeeCompensations.SingleOrDefault(c => c.Employee.EmployeeId == id);
        }

        public Compensation Add(Compensation compensation)
        {
            // Can't add new compensation if one already exists
            var currentCompensation = _employeeContext.Find(typeof(Compensation), compensation.Employee.EmployeeId);
            if (currentCompensation != null) return null;
            
            compensation.EmployeeId = compensation.Employee.EmployeeId;
            
            _employeeContext.Add(compensation);
            return compensation;
        }

        public Task SaveAsync()
        {
            return _employeeContext.SaveChangesAsync();
        }
    }
}