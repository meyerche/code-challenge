using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using challenge.Services;
using challenge.Models;

namespace challenge.Controllers
{
    [Route("api/employee")]
    public class EmployeeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;
        private readonly ICompensationService _compensationService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService, ICompensationService compensationService)
        {
            _logger = logger;
            _employeeService = employeeService;
            _compensationService = compensationService;
        }

        [HttpPost("{id}/compensation")]
        public IActionResult CreateCompensation(string id, [FromBody] Compensation compensation)
        {
            _logger.LogDebug($"Received compensation create request for {id}");

            compensation.Employee = _employeeService.GetById(id);
            
            //Salary must be specified
            if (compensation.Salary == 0) return BadRequest("Salary is required."); 
            
            //Default EffectiveDate is Today
            if (compensation.EffectiveDate == DateTime.MinValue) compensation.EffectiveDate = DateTime.Today;
            
            var result = _compensationService.Create(compensation);
            
            //Compensation already exists
            if (result == null) return BadRequest("Compensation already exists.");
            
            return CreatedAtRoute("getCompensationByEmployeeId", new { id = compensation.Employee.EmployeeId }, compensation);
        }
        
        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.Create(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}/reports")]
        public IActionResult GetReportingStructureByEmployeeId(String id)
        {
            _logger.LogDebug($"Received reporting structure get request for '{id}'");
            
            var reportingStructure = new ReportingStructure {employee = _employeeService.GetById(id)};
            reportingStructure.numberOfReports = _employeeService.CountAllReports(reportingStructure.employee);
            
            if (reportingStructure.employee == null)
                return NotFound();

            return Ok(reportingStructure);
        }
        
        [HttpGet("{id}/compensation", Name = "getCompensationByEmployeeId")]
        public IActionResult GetCompensationByEmployeeId(String id)
        {
            _logger.LogDebug($"Received compensation get request for '{id}'");

            var compensation = _compensationService.GetById(id);
            
            if (compensation == null)
                return NotFound();

            return Ok(compensation);
        }
        
        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetById(id);
            
            if (employee == null)
                return NotFound();

            return Ok(employee);
        }
        
        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Received employee update request for '{id}'");

            var existingEmployee = _employeeService.GetById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.Replace(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }
    }
}
