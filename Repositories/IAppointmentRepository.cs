using SmartQueueAPI.Entities;

namespace SmartQueueAPI.Repositories;

public interface IAppointmentRepository
{
    Task<Appointment> AddAsync(Appointment appointment);
    Task<List<Appointment>> GetAllAsync();
    Task<int> CountBookedBetweenAsync(DateTime fromUtc, DateTime toUtc);
}
