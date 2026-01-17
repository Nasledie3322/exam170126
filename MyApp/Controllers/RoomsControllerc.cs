using Microsoft.AspNetCore.Mvc;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Responses;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _service;
        public RoomsController(IRoomService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> AddRoom(AddRoomDto dto)
        {
            var res = await _service.AddRoomAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var res = await _service.GetRoomsAsync();
            return StatusCode(res.StatusCode, res);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, UpdateRoomDto dto)
        {
            dto.Id = id;
            var res = await _service.UpdateRoomAsync(dto);
            return StatusCode(res.StatusCode, res);
        }

        [HttpPatch("{id}/deactivate")]
        public async Task<IActionResult> DeactivateRoom(int id)
        {
            var res = await _service.DeleteRoomAsync(id);
            return StatusCode(res.StatusCode, res);
        }
    }
}
