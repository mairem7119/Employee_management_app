namespace EmployeeManagement.Core.Entities;

public class Department 
{
  public int Id { get; set; }
  public string Code {get; set;} = string.Empty;
  public string Name {get; set;} = string.Empty;
  public string Description {get; set;} = string.Empty;
  public string IsActive {get; set;} = string.Empty;
  public DateTime CreatedAt {get; set;}
  public DateTime UpdatedAt {get; set;}
}