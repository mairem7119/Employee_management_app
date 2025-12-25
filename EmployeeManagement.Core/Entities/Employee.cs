namespace EmployeeManagement.Core.Entities;

public class Employee
{
  public int Id {get; set;}
  public string FirstName{get; set;}
  public string LastName{get; set;}
  public string Email{get; set;}
  public string PhoneNumber{get; set;}
  public string Department{get; set;}
  public string Position{get; set;}
  public string Salary{get; set;}
  public string HireDate{get; set;}
  public DateTime CreatedAt{get; set;}
  public DateTime UpdatedAt{get; set;}
}