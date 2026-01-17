using System.Net;
using Dapper;
using WebApi.Interfaces;
using WebApi.Responses;
using Infrastructure.Data;

namespace WebApi.Services;

public class PatientService(ApplicationDbContext applicationDbContext) : IPatientService
{
    private readonly ApplicationDbContext _dbContext = applicationDbContext;

    public async Task<Response<string>> AddPatientAsync(AddPatientDto patientDto)
    {
        using var conn = _dbContext.Connection();
        var query = @"
            INSERT INTO patients(fullname, phone, birthdate, isactive, createdat)
            VALUES(@Fullname, @Phone, @Birthdate, @IsActive, @CreatedAt)";
        var res = await conn.ExecuteAsync(query, new
        {
            fullname = patientDto.Fullname,
            phone = patientDto.Phone,
            birthdate = patientDto.Birthdate,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        });

        return res == 0
            ? new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!")
            : new Response<string>(HttpStatusCode.OK, "Patient added successfully!");
    }

    public async Task<Response<List<PatientDto>>> GetPatientsAsync()
    {
        using var conn = _dbContext.Connection();
        var query = "SELECT id, fullname, phone, birthdate, isactive, createdat FROM patients";
        var res = (await conn.QueryAsync<PatientDto>(query)).ToList();

        return new Response<List<PatientDto>>(
            HttpStatusCode.OK,
            res.Count == 0 ? "No patients found" : "Patients retrieved",
            res
        );
    }

    public async Task<Response<PatientDto>> GetPatientByIdAsync(int patientId)
    {
        using var conn = _dbContext.Connection();
        var query = "SELECT id, fullname, phone, birthdate, isactive, createdat FROM patients WHERE id=@Id";
        var res = (await conn.QueryAsync<PatientDto>(query, new { Id = patientId })).ToList();

        if (res.Count == 0)
            return new Response<PatientDto>(HttpStatusCode.NotFound, "Patient not found", new List<PatientDto>());

        return new Response<PatientDto>(HttpStatusCode.OK, "Patient data:", res);
    }

    public async Task<Response<string>> UpdatePatientAsync(UpdatePatientDto patientDto)
    {
        using var conn = _dbContext.Connection();
        var query = @"
            UPDATE patients
            SET fullname=@Fullname, phone=@Phone, birthdate=@Birthdate, isactive=@IsActive
            WHERE id=@Id";
        var res = await conn.ExecuteAsync(query, new
        {
            id = patientDto.Id,
            fullname = patientDto.Fullname,
            phone = patientDto.Phone,
            birthdate = patientDto.Birthdate,
            isActive = patientDto.IsActive
        });

        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Patient not found")
            : new Response<string>(HttpStatusCode.OK, "Patient updated successfully!");
    }

    public async Task<Response<string>> DeactivatePatientAsync(int patientId)
    {
        using var conn = _dbContext.Connection();
        var query = "UPDATE patients SET isactive=false WHERE id=@Id";
        var res = await conn.ExecuteAsync(query, new { Id = patientId });

        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Patient not found")
            : new Response<string>(HttpStatusCode.OK, "Patient deactivated successfully!");
    }

    public async Task<Response<string>> DeletePatientAsync(int patientId)
    {
        using var conn = _dbContext.Connection();
        var query = "DELETE FROM patients WHERE id=@Id";
        var res = await conn.ExecuteAsync(query, new { Id = patientId });

        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Patient not found")
            : new Response<string>(HttpStatusCode.OK, "Patient deleted successfully!");
    }
}
