using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Responses;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("patients")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _service;
        public PatientsController(IPatientService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> AddPatient(AddPatientDto dto)
        {
            var res = await _service.AddPatientAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        public async Task<IActionResult> GetPatients()
        {
            var res = await _service.GetPatientsAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPatientById(int id)
        {
            var res = await _service.GetPatientByIdAsync(id);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePatient(int id, UpdatePatientDto dto)
        {
            dto.Id = id;
            var res = await _service.UpdatePatientAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivatePatient(int id)
        {
            var res = await _service.DeactivatePatientAsync(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
