namespace WebApi.DTOs
{
    public class QueueEventDto
    {
        public int Id { get; set; }
        public int AppointmentId { get; set; }
        public string EventType { get; set; }  
        public DateTime CreatedAt { get; set; }
    }
}
