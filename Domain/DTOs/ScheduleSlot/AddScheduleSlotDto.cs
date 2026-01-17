public class AddScheduleSlotDto
{
    public int DoctorId { get; set; }
    public int RoomId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}
