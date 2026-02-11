using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using MVCLearningsPOC.Data.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace MVCLearningsPOC.Models
{


    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Department is required")]
        public int DepartmentId { get; set; }

        // Navigation Property
        [ForeignKey("DepartmentId")]
        [ValidateNever]
        public Department Department { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Date of Joining is required")]
        [NotFutureDate(ErrorMessage = "Date of Joining cannot be in the future")]
        [DataType(DataType.Date)]
        public DateTime DateOfJoining { get; set; }

        [Required(ErrorMessage = "Salary is required")]
        public decimal Salary { get; set; }
    }

}
