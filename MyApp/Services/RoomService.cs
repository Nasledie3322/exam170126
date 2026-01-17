using System.Net;
using Dapper;
using Infrastructure.Data;
using WebApi.Responses;

namespace WebApi.Services;

public class RoomService(ApplicationDbContext applicationDbContext) : IRoomService
{
    private readonly ApplicationDbContext _dbContext = applicationDbContext;

    public async Task<Response<string>> AddRoomAsync(AddRoomDto roomDto)
    {
        using var conn = _dbContext.Connection();
        var query = @"INSERT INTO rooms(name, isactive) VALUES(@name, @isactive)";
        var res = await conn.ExecuteAsync(query, new
        {
            name = roomDto.Name,
            isactive = true
        });
        return res == 0
            ? new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!")
            : new Response<string>(HttpStatusCode.OK, "Room added successfully!");
    }

    public async Task<Response<List<RoomDto>>> GetRoomsAsync()
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<RoomDto>(
            "SELECT id, name, isactive FROM rooms")).ToList();
        return new Response<List<RoomDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No rooms found" : "Rooms retrieved", res);
    }

    public async Task<Response<RoomDto>> GetRoomByIdAsync(int roomId)
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<RoomDto>(
            "SELECT id, name, isactive FROM rooms WHERE id=@id",
            new { id = roomId })).ToList();
        if (res.Count == 0)
            return new Response<RoomDto>(HttpStatusCode.NotFound, "Room not found", new List<RoomDto>());
        return new Response<RoomDto>(HttpStatusCode.OK, "Room data:", res);
    }

    public async Task<Response<string>> UpdateRoomAsync(UpdateRoomDto roomDto)
    {
        using var conn = _dbContext.Connection();
        var query = @"UPDATE rooms SET name=@name, isactive=@isactive WHERE id=@id";
        var res = await conn.ExecuteAsync(query, new
        {
            id = roomDto.Id,
            name = roomDto.Name,
            isactive = roomDto.IsActive
        });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Room not found")
            : new Response<string>(HttpStatusCode.OK, "Room updated successfully!");
    }

    public async Task<Response<string>> DeactivateRoomAsync(int roomId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync("UPDATE rooms SET isactive=false WHERE id=@id", new { id = roomId });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Room not found")
            : new Response<string>(HttpStatusCode.OK, "Room deactivated successfully!");
    }

    public async Task<Response<string>> DeleteRoomAsync(int roomId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync("DELETE FROM rooms WHERE id=@id", new { id = roomId });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Room not found")
            : new Response<string>(HttpStatusCode.OK, "Room deleted successfully!");
    }
}
