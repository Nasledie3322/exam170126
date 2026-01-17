public class ScheduleSlot
{
    public int Id { get; set; }
    public int DoctorId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}
