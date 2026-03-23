using SmartQueueAPI.DTOs.Appointments;

namespace SmartQueueAPI.Services.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentResponseDto> BookAsync(CreateAppointmentRequestDto request);
    Task<IReadOnlyList<AppointmentResponseDto>> GetAllAsync();
}
