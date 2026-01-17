public class AppointmentDto
{
    public int AppointmentId { get; set; }
    public string PatientName { get; set; }
    public string DoctorName { get; set; }
    public string RoomName { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; }
}
