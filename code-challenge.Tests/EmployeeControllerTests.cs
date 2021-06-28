using challenge.Controllers;
using challenge.Data;
using challenge.Models;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using code_challenge.Tests.Integration.Extensions;

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using code_challenge.Tests.Integration.Helpers;
using System.Text;

namespace code_challenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer(WebHost.CreateDefaultBuilder()
                .UseStartup<TestServerStartup>()
                .UseEnvironment("Development"));

            _httpClient = _testServer.CreateClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }
        
        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void GetReportingStructureById_Returns_Ok()
        {
            //Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedNumberOfReports = 4;
            
            //Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reports");
            var response = getRequestTask.Result;
            
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, reportingStructure.employee.FirstName);
            Assert.AreEqual(expectedLastName, reportingStructure.employee.LastName);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.numberOfReports);
        }
        
        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [TestMethod]
        public void GetReportingStructureZeroReports_Returns_OK()
        {
            //Arrange
            var employeeId = "62c1084e-6e34-4630-93fd-9153afb65309";
            var expectedFirstName = "Pete";
            var expectedLastName = "Best";
            var expectedNumberOfReports = 0;
            
            //Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}/reports");
            var response = getRequestTask.Result;
                
            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, reportingStructure.employee.FirstName);
            Assert.AreEqual(expectedLastName, reportingStructure.employee.LastName);
            Assert.AreEqual(expectedNumberOfReports, reportingStructure.numberOfReports);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            //Arrange
            var id = "b7839309-3348-463b-a7e3-5de1c168beb3";
            var firstName = "Paul";
            var lastName = "McCartney";
            var compensation = new
            {
                Salary = 100000,
                EffectiveDate = "7/4/2021"
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            
            //Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{id}/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;
            
            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(firstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(lastName, newCompensation.Employee.LastName);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(compensation.EffectiveDate, newCompensation.EffectiveDate.ToString("M/d/yyyy"));
        }
        
        [TestMethod]
        public void CreateCompensationExisting_Returns_BadRequest()
        {
            //Arrange
            var id = "b7839309-3348-463b-a7e3-5de1c168beb3";
            var firstName = "Paul";
            var lastName = "McCartney";
            var compensation = new
            {
                Salary = 100000,
                EffectiveDate = "7/4/2021"
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            
            //Execute
            var postRequestTask1 = _httpClient.PostAsync($"api/employee/{id}/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));

            postRequestTask1.Wait();
            
            var postRequestTask2 = _httpClient.PostAsync($"api/employee/{id}/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            
            var response = postRequestTask2.Result;
            
            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [TestMethod]
        public void CreateCompensationNoSalary_Returns_BadRequest()
        {
            //Arrange
            var id = "b7839309-3348-463b-a7e3-5de1c168beb3";
            var compensation = new
            {
                EffectiveDate = "7/4/2021"
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            
            //Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{id}/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            
            var response = postRequestTask.Result;
            
            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
        
        [TestMethod]
        public void CreateCompensationNoDate_Returns_Created()
        {
            //Arrange
            var id = "c0c2293d-16bd-4603-8e08-638a9d18b22c";
            var firstName = "George";
            var lastName = "Harrison";
            var compensation = new
            {
                Salary = 12345
            };

            var requestContent = new JsonSerialization().ToJson(compensation);
            
            //Execute
            var postRequestTask = _httpClient.PostAsync($"api/employee/{id}/compensation",
                new StringContent(requestContent, Encoding.UTF8, "application/json"));
            
            var response = postRequestTask.Result;
            
            //Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(firstName, newCompensation.Employee.FirstName);
            Assert.AreEqual(lastName, newCompensation.Employee.LastName);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
            Assert.AreEqual(DateTime.Today.ToString("M/d/yyyy"), newCompensation.EffectiveDate.ToString("M/d/yyyy"));
        }
        
        [TestMethod]
        public void ReadCompensation_Returns_EmployeeNotFound()
        {
            //Arrange
            var id = "00000";
            
            //Execute
            var request = _httpClient.GetAsync($"api/employee/{id}/compensation");
            var response = request.Result;
            
            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
        
        [TestMethod]
        public void ReadCompensation_Returns_NotFound()
        {
            //Arrange
            var id = "62c1084e-6e34-4630-93fd-9153afb65309";
            
            //Execute
            var request = _httpClient.GetAsync($"api/employee/{id}/compensation");
            var response = request.Result;
            
            //Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
