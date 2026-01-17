using WebApi.Responses;

namespace WebApi.Services;

public interface IRoomService
{
    Task<Response<string>> AddRoomAsync(AddRoomDto roomDto);
    Task<Response<List<RoomDto>>> GetRoomsAsync();
    Task<Response<RoomDto?>> GetRoomByIdAsync(int id);
    Task<Response<string>> UpdateRoomAsync(UpdateRoomDto roomDto);
    Task<Response<string>> DeleteRoomAsync(int id);
}
