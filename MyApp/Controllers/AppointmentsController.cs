using Microsoft.AspNetCore.Mvc;
using WebApi.Interfaces;
namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _service;

    public AppointmentsController(IAppointmentService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] AddAppointmentDto appointmentDto)
    {
        var res = await _service.AddAppointmentAsync(appointmentDto);
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var res = await _service.GetAppointmentsAsync();
        return StatusCode(res.StatusCode, res);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var res = await _service.GetAppointmentByIdAsync(id);
        return StatusCode(res.StatusCode, res);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AddAppointmentDto appointmentDto)
    {
        var res = await _service.UpdateAppointmentAsync(appointmentDto, id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("checkin/{id}")]
    public async Task<IActionResult> CheckIn(int id)
    {
        var res = await _service.CheckInAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("start/{id}")]
    public async Task<IActionResult> Start(int id)
    {
        var res = await _service.StartAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("finish/{id}")]
    public async Task<IActionResult> Finish(int id)
    {
        var res = await _service.FinishAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpPatch("cancel/{id}")]
    public async Task<IActionResult> Cancel(int id)
    {
        var res = await _service.CancelAsync(id);
        return StatusCode(res.StatusCode, res);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _service.DeleteAppointmentAsync(id);
        return StatusCode(res.StatusCode, res);
    }
}
