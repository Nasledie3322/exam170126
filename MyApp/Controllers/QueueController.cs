using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Responses;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QueueEventsController : ControllerBase
{
    private readonly IQueueEventService _queueEventService;

    public QueueEventsController(IQueueEventService queueEventService)
    {
        _queueEventService = queueEventService;
    }

    [HttpPost("{appointmentId}/{eventType}")]
    public async Task<ActionResult<Response<string>>> AddQueueEvent(int appointmentId, int eventType)
    {
        var res = await _queueEventService.AddQueueEventAsync(appointmentId, eventType);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpGet("{appointmentId}")]
    public async Task<ActionResult<Response<List<QueueEventDto>>>> GetQueueEvents(int appointmentId)
    {
        var res = await _queueEventService.GetQueueEventsAsync(appointmentId);
        return StatusCode((int)res.StatusCode, res);
    }

    [HttpDelete("{queueEventId}")]
    public async Task<ActionResult<Response<string>>> DeleteQueueEvent(int queueEventId)
    {
        var res = await _queueEventService.DeleteQueueEventAsync(queueEventId);
        return StatusCode((int)res.StatusCode, res);
    }
}
