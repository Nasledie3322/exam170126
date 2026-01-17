using WebApi.DTOs;
using WebApi.Responses;

namespace WebApi.Interfaces;

public interface IScheduleSlotService
{
    Task<Response<string>> AddScheduleSlotAsync(AddScheduleSlotDto slotDto);

    Task<Response<List<ScheduleSlotDto>>> GetSlotsAsync();

    Task<Response<ScheduleSlotDto>> GetSlotByIdAsync(int slotId);

    Task<Response<List<ScheduleSlotDto>>> GetSlotsByDoctorAsync(int doctorId);

    Task<Response<string>> UpdateSlotAsync(AddScheduleSlotDto slotDto, int slotId);

    Task<Response<string>> DeactivateSlotAsync(int slotId);

    Task<Response<string>> DeleteSlotAsync(int slotId);
}
