using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Responses;

namespace WebApi.Interfaces
{
    public interface IQueueEventService
    {
        Task<Response<string>> AddQueueEventAsync(int appointmentId, int eventType);

        Task<Response<List<QueueEventDto>>> GetQueueEventsAsync(int appointmentId);

        Task<Response<string>> DeleteQueueEventAsync(int queueEventId);
    }
}
