using EmployeeManagement.Core.Entities;

namespace EmployeeManagement.Web.Models;

public class CreateEmployeeViewModel
{
    public Employee Employee { get; set; } = new();
    public IEnumerable<Department> Departments { get; set; } = new List<Department>();
    public IEnumerable<Position> Positions { get; set; } = new List<Position>();
}
