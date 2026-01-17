using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
using WebApi.Responses;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScheduleSlotsController : ControllerBase
{
    private readonly IScheduleSlotService _service;

    public ScheduleSlotsController(IScheduleSlotService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<Response<string>> Add([FromBody] AddScheduleSlotDto slotDto)
    {
        return await _service.AddScheduleSlotAsync(slotDto);
    }

    [HttpGet]
    public async Task<Response<List<ScheduleSlotDto>>> Get()
    {
        return await _service.GetSlotsAsync();
    }

    [HttpGet("{id}")]
    public async Task<Response<ScheduleSlotDto>> GetById(int id)
    {
        return await _service.GetSlotByIdAsync(id);
    }

    [HttpGet("doctor/{doctorId}")]
    public async Task<Response<List<ScheduleSlotDto>>> GetByDoctor(int doctorId)
    {
        return await _service.GetSlotsByDoctorAsync(doctorId);
    }

    [HttpPut("{id}")]
    public async Task<Response<string>> Update(int id, [FromBody] AddScheduleSlotDto slotDto)
    {
        return await _service.UpdateSlotAsync(slotDto, id);
    }

    [HttpPatch("{id}/deactivate")]
    public async Task<Response<string>> Deactivate(int id)
    {
        return await _service.DeactivateSlotAsync(id);
    }

    [HttpDelete("{id}")]
    public async Task<Response<string>> Delete(int id)
    {
        return await _service.DeleteSlotAsync(id);
    }
}
