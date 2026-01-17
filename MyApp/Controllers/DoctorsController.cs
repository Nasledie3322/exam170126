using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Responses;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("doctors")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorService _service;
        public DoctorsController(IDoctorService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> AddDoctor(AddDoctorDto dto)
        {
            var res = await _service.AddDoctorAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        public async Task<IActionResult> GetDoctors()
        {
            var res = await _service.GetDoctorsAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDoctorById(int id)
        {
            var res = await _service.GetDoctorByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDoctor(int id, UpdateDoctorDto dto)
        {
            dto.Id = id;
            var res = await _service.UpdateDoctorAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateDoctor(int id)
        {
            var res = await _service.DeleteDoctorAsync(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
