using System.Net;
using Dapper;
using Infrastructure.Data;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Responses;

namespace WebApi.Services;

public class ScheduleSlotService(ApplicationDbContext applicationDbContext) : IScheduleSlotService
{
    private readonly ApplicationDbContext _dbContext = applicationDbContext;

    public async Task<Response<string>> AddScheduleSlotAsync(AddScheduleSlotDto slotDto)
    {
        using var conn = _dbContext.Connection();
        var query = @"INSERT INTO schedule_slots(doctorid, roomid, starttime, endtime, isactive, createdat)
                      VALUES(@doctorid, @roomid, @starttime, @endtime, @isactive, @createdat)";
        var res = await conn.ExecuteAsync(query, new
        {
            doctorid = slotDto.DoctorId,
            roomid = slotDto.RoomId,
            starttime = slotDto.StartTime,
            endtime = slotDto.EndTime,
            isactive = true,
            createdat = DateTime.UtcNow
        });
        return res == 0
            ? new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!")
            : new Response<string>(HttpStatusCode.OK, "Schedule slot added successfully!");
    }

    public async Task<Response<List<ScheduleSlotDto>>> GetSlotsAsync()
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<ScheduleSlotDto>(
            "SELECT id, doctorid, roomid, starttime, endtime, isactive FROM schedule_slots")).ToList();
        return new Response<List<ScheduleSlotDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No slots found" : "Slots retrieved", res);
    }

    public async Task<Response<ScheduleSlotDto>> GetSlotByIdAsync(int slotId)
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<ScheduleSlotDto>(
            "SELECT id, doctorid, roomid, starttime, endtime, isactive FROM schedule_slots WHERE id=@id",
            new { id = slotId })).ToList();
        if (res.Count == 0)
            return new Response<ScheduleSlotDto>(HttpStatusCode.NotFound, "Slot not found", new List<ScheduleSlotDto>());
        return new Response<ScheduleSlotDto>(HttpStatusCode.OK, "Slot data:", res);
    }

    public async Task<Response<List<ScheduleSlotDto>>> GetSlotsByDoctorAsync(int doctorId)
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<ScheduleSlotDto>(
            "SELECT id, doctorid, roomid, starttime, endtime, isactive FROM schedule_slots WHERE doctorid=@doctorid",
            new { doctorid = doctorId })).ToList();
        return new Response<List<ScheduleSlotDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No slots found for this doctor" : "Slots retrieved", res);
    }

    public async Task<Response<string>> UpdateSlotAsync(AddScheduleSlotDto slotDto, int slotId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync(
            @"UPDATE schedule_slots SET doctorid=@doctorid, roomid=@roomid, starttime=@starttime, endtime=@endtime 
              WHERE id=@id",
            new
            {
                doctorid = slotDto.DoctorId,
                roomid = slotDto.RoomId,
                starttime = slotDto.StartTime,
                endtime = slotDto.EndTime,
                id = slotId
            });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Slot not found")
            : new Response<string>(HttpStatusCode.OK, "Schedule slot updated successfully!");
    }

    public async Task<Response<string>> DeactivateSlotAsync(int slotId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync("UPDATE schedule_slots SET isactive=false WHERE id=@id", new { id = slotId });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Slot not found")
            : new Response<string>(HttpStatusCode.OK, "Slot deactivated successfully!");
    }

    public async Task<Response<string>> DeleteSlotAsync(int slotId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync("DELETE FROM schedule_slots WHERE id=@id", new { id = slotId });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Slot not found")
            : new Response<string>(HttpStatusCode.OK, "Slot deleted successfully!");
    }
}
