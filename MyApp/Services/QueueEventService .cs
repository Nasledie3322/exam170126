using System.Net;
using Dapper;
using Infrastructure.Data;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Responses;

namespace WebApi.Services;

public class QueueEventService(ApplicationDbContext applicationDbContext) : IQueueEventService
{
    private readonly ApplicationDbContext _dbContext = applicationDbContext;

    public async Task<Response<string>> AddQueueEventAsync(int appointmentId, int eventType)
    {
        using var conn = _dbContext.Connection();
        var query = @"INSERT INTO queue_events(appointmentid, eventtype, createdat)
                      VALUES(@appointmentid, @eventtype, @createdat)";
        var res = await conn.ExecuteAsync(query, new
        {
            appointmentid = appointmentId,
            eventtype= eventType,
            createdat = DateTime.UtcNow
        });

        return res == 0
            ? new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!")
            : new Response<string>(HttpStatusCode.OK, "Queue event added successfully!");
    }

    public async Task<Response<List<QueueEventDto>>> GetQueueEventsAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<QueueEventDto>(
            "SELECT id, appointmentid, eventtype, createdat FROM queue_events WHERE appointmentid=@appointmentid",
            new { appointmentid = appointmentId })).ToList();

        return new Response<List<QueueEventDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No queue events found" : "Queue events retrieved", res);
    }

    public async Task<Response<string>> DeleteQueueEventAsync(int queueEventId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync(
            "DELETE FROM queue_events WHERE id=@id", new { id = queueEventId });

        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Queue event not found")
            : new Response<string>(HttpStatusCode.OK, "Queue event deleted successfully!");
    }
}
