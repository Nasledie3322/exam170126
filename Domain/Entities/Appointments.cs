public class Appointment
{
    public int Id { get; set; }
    public int PatientId { get; set; }
    public int SlotId { get; set; }
    public AppointmentStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
