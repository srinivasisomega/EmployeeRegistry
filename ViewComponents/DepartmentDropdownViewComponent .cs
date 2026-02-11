using Microsoft.AspNetCore.Mvc.Rendering;
using MVCLearningsPOC.Models;
using Microsoft.AspNetCore.Mvc;
using MVCLearningsPOC.Repository.Interface;

namespace MVCLearningsPOC.ViewModels
{

    public class DepartmentDropdownViewComponent : ViewComponent
    {
        private readonly IRepository<Department> _departmentRepository;

        public DepartmentDropdownViewComponent(IRepository<Department> departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public IViewComponentResult Invoke(int? selectedDepartmentId)
        {
            var departments = _departmentRepository.GetAll()
                .Select(d => new SelectListItem
                {
                    Value = d.DepartmentId.ToString(),
                    Text = d.DepartmentName,
                    Selected = d.DepartmentId == selectedDepartmentId
                }).ToList();

            return View(departments);
        }

    }

}
