using Microsoft.EntityFrameworkCore;
using SmartQueueAPI.Data;
using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public class AppointmentRepository : IAppointmentRepository
{
    private readonly AppDbContext _context;

    public AppointmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Appointment> AddAsync(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return appointment;
    }

    public Task<List<Appointment>> GetAllAsync()
    {
        return _context.Appointments
            .OrderBy(a => a.SlotStartUtc)
            .ToListAsync();
    }

    public Task<int> CountBookedBetweenAsync(DateTime fromUtc, DateTime toUtc)
    {
        return _context.Appointments.CountAsync(a =>
            a.Status == AppointmentStatus.Booked &&
            a.SlotStartUtc < toUtc &&
            a.SlotEndUtc > fromUtc);
    }
}
