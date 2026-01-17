using System.Net;
using Dapper;
using Infrastructure.Data;
using WebApi.DTOs;
using WebApi.Interfaces;
using WebApi.Responses;

namespace WebApi.Services;

public class AppointmentService(ApplicationDbContext applicationDbContext) : IAppointmentService
{
    private readonly ApplicationDbContext _dbContext = applicationDbContext;

    public async Task<Response<string>> AddAppointmentAsync(AddAppointmentDto appointmentDto)
    {
        using var conn = _dbContext.Connection();
        await conn.OpenAsync();
        using var tran = conn.BeginTransaction();
        try
        {
            var patientActive = await conn.ExecuteScalarAsync<bool>(
                "SELECT isactive FROM patients WHERE id=@id", new { id = appointmentDto.PatientId }, tran);
            if (!patientActive)
                return new Response<string>(HttpStatusCode.BadRequest, "Patient is not active!");

            var slotActive = await conn.ExecuteScalarAsync<bool>(
                "SELECT isactive FROM schedule_slots WHERE id=@id", new { id = appointmentDto.SlotId }, tran);
            if (!slotActive)
                return new Response<string>(HttpStatusCode.BadRequest, "Slot is not active!");

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM appointments WHERE slotid=@slotid AND status!=5",
                new { slotid = appointmentDto.SlotId }, tran);
            if (exists > 0)
                return new Response<string>(HttpStatusCode.Conflict, "Slot already booked!");

            var query = @"INSERT INTO appointments(patientid, slotid, status, createdat, updatedat)
                          VALUES(@patientid, @slotid, @status, @createdat, @updatedat) RETURNING id";
            var appointmentId = await conn.ExecuteScalarAsync<int>(query, new
            {
                patientid = appointmentDto.PatientId,
                slotid = appointmentDto.SlotId,
                status = 1,
                createdat = DateTime.UtcNow,
                updatedat = DateTime.UtcNow
            }, tran);

            await conn.ExecuteAsync(
                "INSERT INTO queue_events(appointmentid, eventtype, createdat) VALUES(@appointmentid, @eventtype, @createdat)",
                new { appointmentid = appointmentId, eventtype = 1, createdat = DateTime.UtcNow }, tran);

            tran.Commit();
            return new Response<string>(HttpStatusCode.OK, "Appointment added successfully!");
        }
        catch
        {
            tran.Rollback();
            return new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!");
        }
    }

    public async Task<Response<List<AppointmentDto>>> GetAppointmentsAsync()
    {
        using var conn = _dbContext.Connection();
        var query = @"
            SELECT a.id as appointmentid, p.fullname as patientname, d.fullname as doctorname,
                   r.name as roomname, s.starttime, s.endtime,
                   CASE a.status WHEN 1 THEN 'booked' WHEN 2 THEN 'checked_in'
                                 WHEN 3 THEN 'in_progress' WHEN 4 THEN 'done' WHEN 5 THEN 'cancelled' END as status
            FROM appointments a
            JOIN patients p ON a.patientid=p.id
            JOIN schedule_slots s ON a.slotid=s.id
            JOIN doctors d ON s.doctorid=d.id
            JOIN rooms r ON s.roomid=r.id";
        var res = (await conn.QueryAsync<AppointmentDto>(query)).ToList();
        return new Response<List<AppointmentDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No appointments found" : "Appointments retrieved", res);
    }

    public async Task<Response<AppointmentDto>> GetAppointmentByIdAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        var query = @"
            SELECT a.id as appointmentid, p.fullname as patientname, d.fullname as doctorname,
                   r.name as roomname, s.starttime, s.endtime,
                   CASE a.status WHEN 1 THEN 'booked' WHEN 2 THEN 'checked_in'
                                 WHEN 3 THEN 'in_progress' WHEN 4 THEN 'done' WHEN 5 THEN 'cancelled' END as status
            FROM appointments a
            JOIN patients p ON a.patientid=p.id
            JOIN schedule_slots s ON a.slotid=s.id
            JOIN doctors d ON s.doctorid=d.id
            JOIN rooms r ON s.roomid=r.id
            WHERE a.id=@id";
        var res = (await conn.QueryAsync<AppointmentDto>(query, new { id = appointmentId })).ToList();
        if (res.Count == 0)
            return new Response<AppointmentDto>(HttpStatusCode.NotFound, "Appointment not found", new List<AppointmentDto>());
        return new Response<AppointmentDto>(HttpStatusCode.OK, "Appointment data:", res);
    }

    public async Task<Response<List<AppointmentDto>>> GetAppointmentsByDoctorAsync(int doctorId)
    {
        using var conn = _dbContext.Connection();
        var query = @"
            SELECT a.id as appointmentid, p.fullname as patientname, d.fullname as doctorname,
                   r.name as roomname, s.starttime, s.endtime,
                   CASE a.status WHEN 1 THEN 'booked' WHEN 2 THEN 'checked_in'
                                 WHEN 3 THEN 'in_progress' WHEN 4 THEN 'done' WHEN 5 THEN 'cancelled' END as status
            FROM appointments a
            JOIN patients p ON a.patientid=p.id
            JOIN schedule_slots s ON a.slotid=s.id
            JOIN doctors d ON s.doctorid=d.id
            JOIN rooms r ON s.roomid=r.id
            WHERE d.id=@doctorid";
        var res = (await conn.QueryAsync<AppointmentDto>(query, new { doctorid = doctorId })).ToList();
        return new Response<List<AppointmentDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No appointments found for this doctor" : "Appointments retrieved", res);
    }

    public async Task<Response<string>> UpdateAppointmentAsync(AddAppointmentDto appointmentDto, int appointmentId)
{
    using var conn = _dbContext.Connection();
    var res = await conn.ExecuteAsync(
        "UPDATE appointments SET patientid=@patientid, slotid=@slotid, updatedat=@updatedat WHERE id=@id",
        new
        {
            patientid = appointmentDto.PatientId,
            slotid = appointmentDto.SlotId,
            updatedat = DateTime.UtcNow,
            id = appointmentId
        });
    return res == 0
        ? new Response<string>(HttpStatusCode.NotFound, "Appointment not found")
        : new Response<string>(HttpStatusCode.OK, "Appointment updated successfully!");
}

    public async Task<Response<string>> DeleteAppointmentAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        var res = await conn.ExecuteAsync("DELETE FROM appointments WHERE id=@id", new { id = appointmentId });
        return res == 0
            ? new Response<string>(HttpStatusCode.NotFound, "Appointment not found")
            : new Response<string>(HttpStatusCode.OK, "Appointment deleted successfully!");
    }

    public async Task<Response<List<QueueEventDto>>> GetEventsAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        var res = (await conn.QueryAsync<QueueEventDto>(
            "SELECT id, appointmentid, eventtype, createdat FROM queue_events WHERE appointmentid=@id",
            new { id = appointmentId })).ToList();
        return new Response<List<QueueEventDto>>(HttpStatusCode.OK,
            res.Count == 0 ? "No events found" : "Events retrieved", res);
    }

    public async Task<Response<string>> CheckInAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        await conn.OpenAsync();
        using var tran = conn.BeginTransaction();
        try
        {
            var oldStatus = await conn.ExecuteScalarAsync<int>("SELECT status FROM appointments WHERE id=@id", new { id = appointmentId }, tran);
            if (oldStatus != 1)
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid status transition!");

            await conn.ExecuteAsync("UPDATE appointments SET status=2, updatedat=@updatedat WHERE id=@id",
                new { updatedat = DateTime.UtcNow, id = appointmentId }, tran);

            await conn.ExecuteAsync("INSERT INTO queue_events(appointmentid, eventtype, createdat) VALUES(@appointmentid, @eventtype, @createdat)",
                new { appointmentid = appointmentId, eventtype = 2, createdat = DateTime.UtcNow }, tran);

            tran.Commit();
            return new Response<string>(HttpStatusCode.OK, "Appointment checked in!");
        }
        catch
        {
            tran.Rollback();
            return new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!");
        }
    }

    public async Task<Response<string>> StartAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        await conn.OpenAsync();
        using var tran = conn.BeginTransaction();
        try
        {
            var oldStatus = await conn.ExecuteScalarAsync<int>("SELECT status FROM appointments WHERE id=@id", new { id = appointmentId }, tran);
            if (oldStatus != 2)
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid status transition!");

            await conn.ExecuteAsync("UPDATE appointments SET status=3, updatedat=@updatedat WHERE id=@id",
                new { updatedat = DateTime.UtcNow, id = appointmentId }, tran);

            await conn.ExecuteAsync("INSERT INTO queue_events(appointmentid, eventtype, createdat) VALUES(@appointmentid, @eventtype, @createdat)",
                new { appointmentid = appointmentId, eventtype = 3, createdat = DateTime.UtcNow }, tran);

            tran.Commit();
            return new Response<string>(HttpStatusCode.OK, "Appointment started!");
        }
        catch
        {
            tran.Rollback();
            return new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!");
        }
    }

    public async Task<Response<string>> FinishAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        await conn.OpenAsync();
        using var tran = conn.BeginTransaction();
        try
        {
            var oldStatus = await conn.ExecuteScalarAsync<int>("SELECT status FROM appointments WHERE id=@id", new { id = appointmentId }, tran);
            if (oldStatus != 3)
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid status transition!");

            await conn.ExecuteAsync("UPDATE appointments SET status=4, updatedat=@updatedat WHERE id=@id",
                new { updatedat = DateTime.UtcNow, id = appointmentId }, tran);

            await conn.ExecuteAsync("INSERT INTO queue_events(appointmentid, eventtype, createdat) VALUES(@appointmentid, @eventtype, @createdat)",
                new { appointmentid = appointmentId, eventtype = 4, createdat = DateTime.UtcNow }, tran);

            tran.Commit();
            return new Response<string>(HttpStatusCode.OK, "Appointment finished!");
        }
        catch
        {
            tran.Rollback();
            return new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!");
        }
    }

    public async Task<Response<string>> CancelAsync(int appointmentId)
    {
        using var conn = _dbContext.Connection();
        await conn.OpenAsync();
        using var tran = conn.BeginTransaction();
        try
        {
            var oldStatus = await conn.ExecuteScalarAsync<int>("SELECT status FROM appointments WHERE id=@id", new { id = appointmentId }, tran);
            if (oldStatus is not (1 or 2 or 3))
                return new Response<string>(HttpStatusCode.BadRequest, "Invalid status transition!");

            await conn.ExecuteAsync("UPDATE appointments SET status=5, updatedat=@updatedat WHERE id=@id",
                new { updatedat = DateTime.UtcNow, id = appointmentId }, tran);

            await conn.ExecuteAsync("INSERT INTO queue_events(appointmentid, eventtype, createdat) VALUES(@appointmentid, @eventtype, @createdat)",
                new { appointmentid = appointmentId, eventtype = 5, createdat = DateTime.UtcNow }, tran);

            tran.Commit();
            return new Response<string>(HttpStatusCode.OK, "Appointment cancelled!");
        }
        catch
        {
            tran.Rollback();
            return new Response<string>(HttpStatusCode.InternalServerError, "Something went wrong!");
        }
    }
}
