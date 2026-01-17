using System.Net;
using Dapper;
using Infrastructure.Data;
using WebApi.Responses;

namespace WebApi.Services;

public class DoctorService(ApplicationDbContext applicationDbContext) : IDoctorService
{
    private readonly ApplicationDbContext _dbContext = applicationDbContext;

    public async Task<Response<string>> AddDoctorAsync(AddDoctorDto doctorDto)
    {
        using var conn = _dbContext.Connection();
        var query = @"INSERT INTO doctors(fullname, specialty, isactive, hiredat)
                      VALUES(@Fullname, @Specialty, @IsActive, @HiredAt)";
        var res = await conn.ExecuteAsync(query, new
        {
            fullname = doctorDto.Fullname,
            specialty = doctorDto.Specialty,
            IsActive = true,
            hiredAt = doctorDto.HiredAt
        });
        return res == 0
            ? new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!")
            : new Response<string>(HttpStatusCode.OK, "Doctor added successfully!");
    }

    public async Task<Response<List<DoctorDto>>> GetDoctorsAsync()
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<DoctorDto>(
            "SELECT id, fullname, specialty, isactive, hiredat FROM doctors")).ToList();
        return new Response<List<DoctorDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No doctors found" : "Doctors retrieved", res);
    }

    public async Task<Response<DoctorDto>> GetDoctorByIdAsync(int doctorId)
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<DoctorDto>(
            "SELECT id, fullname, specialty, isactive, hiredat FROM doctors WHERE id=@Id",
            new { Id = doctorId })).ToList();
        if (res.Count == 0)
            return new Response<DoctorDto>(HttpStatusCode.NotFound, "Doctor not found", new List<DoctorDto>());
        return new Response<DoctorDto>(HttpStatusCode.OK, "Doctor data:", res);
    }

    public async Task<Response<string>> UpdateDoctorAsync(UpdateDoctorDto doctorDto)
    {
        using var conn = _dbContext.Connection();
        var query = @"UPDATE doctors
                      SET fullname=@Fullname, specialty=@Specialty, isactive=@IsActive
                      WHERE id=@Id";
        var res = await conn.ExecuteAsync(query, new
        {
            id = doctorDto.Id,
            fullname = doctorDto.Fullname,
            specialty = doctorDto.Specialty,
            isActive = doctorDto.IsActive
        });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Doctor not found")
            : new Response<string>(HttpStatusCode.OK, "Doctor updated successfully!");
    }

    public async Task<Response<string>> DeactivateDoctorAsync(int doctorId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync("UPDATE doctors SET isactive=false WHERE id=@Id", new { Id = doctorId });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Doctor not found")
            : new Response<string>(HttpStatusCode.OK, "Doctor deactivated successfully!");
    }

    public async Task<Response<string>> DeleteDoctorAsync(int doctorId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync("DELETE FROM doctors WHERE id=@Id", new { Id = doctorId });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Doctor not found")
            : new Response<string>(HttpStatusCode.OK, "Doctor deleted successfully!");
    }
}
