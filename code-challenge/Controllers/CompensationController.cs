using System;
using challenge.Models;
using challenge.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace challenge.Controllers
{
    [Route("api/compensation")]
    public class CompensationController : Controller
    {
        private readonly ILogger _logger;
        private readonly ICompensationService _compensationService;
        private readonly IEmployeeService _employeeService;

        public CompensationController(ILogger<CompensationController> logger, ICompensationService compensationService, IEmployeeService employeeService)
        {
            _logger = logger;
            _compensationService = compensationService;
            _employeeService = employeeService;
        }

        [HttpPost("{id}")]
        public IActionResult CreateCompensation(string id, [FromBody] Compensation compensation)
        {
            compensation.Employee = _employeeService.GetById(id);

            _compensationService.Create(compensation);
            
            return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Employee.EmployeeId }, compensation);
        }

        
    }
}