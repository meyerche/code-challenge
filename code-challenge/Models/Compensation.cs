using System;
using System.ComponentModel.DataAnnotations;

namespace challenge.Models
{
    public class Compensation
    {
        [Key]
        public String EmployeeId { get; set; }  //an id was not in the brief, but I needed it to use as a primary key
        public Employee Employee { get; set; }
        public int Salary { get; set; }
        public DateTime EffectiveDate { get; set; }
    }
}