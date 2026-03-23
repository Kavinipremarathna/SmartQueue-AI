namespace SmartQueueAPI.DTOs.Appointments;

public record AppointmentResponseDto(
    int Id,
    string CustomerName,
    DateTime SlotStartUtc,
    DateTime SlotEndUtc,
    string Status);
