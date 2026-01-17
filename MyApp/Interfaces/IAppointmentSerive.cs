using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Responses;

namespace WebApi.Interfaces
{
    public interface IAppointmentService
    {
        Task<Response<string>> AddAppointmentAsync(AddAppointmentDto appointmentDto);

        Task<Response<List<AppointmentDto>>> GetAppointmentsAsync();

        Task<Response<AppointmentDto>> GetAppointmentByIdAsync(int appointmentId);

        Task<Response<string>> CheckInAsync(int appointmentId);

        Task<Response<string>> StartAsync(int appointmentId);

        Task<Response<string>> FinishAsync(int appointmentId);

        Task<Response<string>> CancelAsync(int appointmentId);

        Task<Response<string>> DeleteAppointmentAsync(int appointmentId);

        Task<Response<string>> UpdateAppointmentAsync(AddAppointmentDto appointmentDto, int appointmentId);
    }
}
