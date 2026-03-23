using SmartQueueAPI.DTOs.Appointments;
using SmartQueueAPI.Entities;
using SmartQueueAPI.Repositories;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Services;

public class AppointmentService : IAppointmentService
{
    private const int MaxParallelAppointmentsPerWindow = 4;

    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IQueueConfigurationRepository _configurationRepository;

    public AppointmentService(
        IAppointmentRepository appointmentRepository,
        IQueueConfigurationRepository configurationRepository)
    {
        _appointmentRepository = appointmentRepository;
        _configurationRepository = configurationRepository;
    }

    public async Task<AppointmentResponseDto> BookAsync(CreateAppointmentRequestDto request)
    {
        var requestedStart = request.SlotStartUtc.ToUniversalTime();
        var duration = Math.Clamp(request.DurationMinutes, 10, 120);
        var requestedEnd = requestedStart.AddMinutes(duration);

        var config = await _configurationRepository.GetOrCreateAsync();
        var loadCap = Math.Max(1, config.StaffCount) * MaxParallelAppointmentsPerWindow;

        // Auto-adjust to next available slot when load is too high.
        while (await _appointmentRepository.CountBookedBetweenAsync(requestedStart, requestedEnd) >= loadCap)
        {
            requestedStart = requestedStart.AddMinutes(15);
            requestedEnd = requestedEnd.AddMinutes(15);
        }

        var appointment = new Appointment
        {
            CustomerName = request.CustomerName.Trim(),
            SlotStartUtc = requestedStart,
            SlotEndUtc = requestedEnd,
            Status = AppointmentStatus.Booked
        };

        var saved = await _appointmentRepository.AddAsync(appointment);

        return new AppointmentResponseDto(
            saved.Id,
            saved.CustomerName,
            saved.SlotStartUtc,
            saved.SlotEndUtc,
            saved.Status);
    }

    public async Task<IReadOnlyList<AppointmentResponseDto>> GetAllAsync()
    {
        var appointments = await _appointmentRepository.GetAllAsync();
        return appointments
            .Select(a => new AppointmentResponseDto(a.Id, a.CustomerName, a.SlotStartUtc, a.SlotEndUtc, a.Status))
            .ToList();
    }
}
