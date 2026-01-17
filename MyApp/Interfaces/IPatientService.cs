using System.Collections.Generic;
using System.Threading.Tasks;
using WebApi.DTOs;
using WebApi.Responses;

namespace WebApi.Interfaces;

public interface IPatientService
{
    Task<Response<string>> AddPatientAsync(AddPatientDto patientDto);
    Task<Response<List<PatientDto>>> GetPatientsAsync();
    Task<Response<PatientDto>> GetPatientByIdAsync(int patientId);
    Task<Response<string>> UpdatePatientAsync(UpdatePatientDto patientDto);
    Task<Response<string>> DeactivatePatientAsync(int patientId);
    Task<Response<string>> DeletePatientAsync(int patientId);
}
