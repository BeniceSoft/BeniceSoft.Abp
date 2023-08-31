using System.ComponentModel.DataAnnotations;
using BeniceSoft.Abp.Core.Attributes;
using BeniceSoft.Abp.OperationLogging.Abstractions;
using Volo.Abp.Application.Services;

namespace BeniceSoft.Abp.Sample.Application.Services;

public class SampleAppService : ApplicationService
{
    // [OperationLog(OperationType = "Create", BizModule = "Sample")]
    public virtual async Task<CreateDto> CreateAsync(CreateDto dto, [IgnoreBind] OperationLogContext? context = null)
    {
        context?.SetValue(Guid.NewGuid().ToString(), "BizCode0001", "", new()
        {
            { "key1", "value1" }
        });

        return dto;
    }
}

public class CreateDto
{
    [Required] public string Name { get; set; }

    [Desensitize(Type = DesensitizeType.PhoneNumber)]
    public string PhoneNumber { get; set; }
}