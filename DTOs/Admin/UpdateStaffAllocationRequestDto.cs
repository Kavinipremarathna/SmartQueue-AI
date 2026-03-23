using System.ComponentModel.DataAnnotations;

namespace SmartQueueAPI.DTOs.Admin;

public class UpdateStaffAllocationRequestDto
{
    [Range(1, 20)]
    public int StaffCount { get; set; }
}
