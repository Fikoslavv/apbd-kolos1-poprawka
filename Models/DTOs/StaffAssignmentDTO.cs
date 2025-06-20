namespace apbd_kolos1_poprawka.Models.DTOs;

public class StaffAssignmentDTO
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
    public string Role { get; set; } = string.Empty;
}
