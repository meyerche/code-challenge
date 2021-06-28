using challenge.Models;
using Microsoft.EntityFrameworkCore;

namespace challenge.Data
{
    public class CompensationContext : DbContext
    {
        public DbSet<Compensation> EmployeesCompensation { get; set; }
    }
}