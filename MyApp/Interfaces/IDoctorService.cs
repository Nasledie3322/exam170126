using WebApi.Responses;

namespace WebApi.Services;

public interface IDoctorService
{
    Task<Response<string>> AddDoctorAsync(AddDoctorDto doctorDto);
    Task<Response<List<DoctorDto>>> GetDoctorsAsync();
    Task<Response<DoctorDto?>> GetDoctorByIdAsync(int id);
    Task<Response<string>> UpdateDoctorAsync(UpdateDoctorDto doctorDto);
    Task<Response<string>> DeleteDoctorAsync(int id);
}
