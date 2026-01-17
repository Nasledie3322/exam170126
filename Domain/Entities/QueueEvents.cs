public class QueueEvent
{
    public int Id {get;set;}
    public int AppointmentId {get;set;}
    public QueueEventType EventType {get;set;}
    public DateTime CreatedAt {get;set;}
}