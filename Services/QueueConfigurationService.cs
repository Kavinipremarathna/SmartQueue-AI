using SmartQueueAPI.DTOs.Admin;
using SmartQueueAPI.Repositories;
using SmartQueueAPI.Services.Interfaces;

namespace SmartQueueAPI.Services;

public class QueueConfigurationService : IQueueConfigurationService
{
    private readonly IQueueConfigurationRepository _configurationRepository;

    public QueueConfigurationService(IQueueConfigurationRepository configurationRepository)
    {
        _configurationRepository = configurationRepository;
    }

    public async Task<StaffAllocationResponseDto> GetAsync()
    {
        var config = await _configurationRepository.GetOrCreateAsync();
        return new StaffAllocationResponseDto(config.StaffCount, config.AverageServiceMinutes);
    }

    public async Task<StaffAllocationResponseDto> UpdateStaffCountAsync(int staffCount)
    {
        var config = await _configurationRepository.GetOrCreateAsync();
        config.StaffCount = Math.Clamp(staffCount, 1, 20);
        await _configurationRepository.SaveChangesAsync();

        return new StaffAllocationResponseDto(config.StaffCount, config.AverageServiceMinutes);
    }
}
