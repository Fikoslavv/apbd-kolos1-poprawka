namespace apbd_kolos1_poprawka.Models.DTOs;

public class StaffDTO
{
    public int StaffId { get; set; } = -1;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime HireDate { get; set; }
}
