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
    [Route("/api/reports")]
    public class ReportingStructureController : Controller
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }
        
        [HttpGet("{id}")]
        public IActionResult GetReportsByEmployeeId(String id)
        {
            _logger.LogDebug($"Received reporting structure get request for '{id}'");
            
            var reportingStructure = new ReportingStructure {employee = _employeeService.GetById(id)};
            reportingStructure.numberOfReports = _employeeService.CountAllReports(reportingStructure.employee);
            
            if (reportingStructure.employee == null)
                return NotFound();

            return Ok(reportingStructure);
        }
        
    }
}