public class UpdatePatientDto
{
    public int Id { get; set; }
    public string Fullname { get; set; }
    public string Phone { get; set; }
    public DateTime? Birthdate { get; set; }
    public bool IsActive { get; set; }
}
