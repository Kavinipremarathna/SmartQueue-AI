using SmartQueueAPI.DTOs.Admin;

namespace SmartQueueAPI.Services.Interfaces;

public interface IQueueConfigurationService
{
    Task<StaffAllocationResponseDto> GetAsync();
    Task<StaffAllocationResponseDto> UpdateStaffCountAsync(int staffCount);
}
