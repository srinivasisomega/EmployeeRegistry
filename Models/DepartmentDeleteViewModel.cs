namespace MVCLearningsPOC.Models
{
    public class DepartmentDeleteViewModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public int EmployeeCount { get; set; }
        public bool DeleteEmployees { get; set; }
    }

}
