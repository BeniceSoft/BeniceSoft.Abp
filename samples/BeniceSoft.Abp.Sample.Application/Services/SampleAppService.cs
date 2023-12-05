using System.ComponentModel.DataAnnotations;
using BeniceSoft.Abp.Core.Attributes;
using BeniceSoft.Abp.OperationLogging.Abstractions;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BeniceSoft.Abp.Sample.Application.Services;

public class SampleAppService : ApplicationService
{
    [OperationLog(OperationType = "Create", BizModule = "Sample")]
    public virtual async Task<CreateDto> CreateAsync(CreateDto dto, [IgnoreBind] OperationLogContext? context = null)
    {
        context?.SetValue(Guid.NewGuid().ToString(), "BizCode0001", "", new()
        {
            { "key1", "value1" }
        });

        return dto;
    }

    public List<TestModel> List()
    {
        return new()
        {
            new() { Name = "zhangsan", PhoneNumber = "13000000001" },
            new() { Name = "lisi", PhoneNumber = "13000000002" },
        };
    }

    public PagedResultDto<TestModel> Page()
    {
        return new(10, new List<TestModel>
        {
            new() { Name = "zhangsan", PhoneNumber = "13000000001" },
            new() { Name = "lisi", PhoneNumber = "13000000002" },
        });
    }
}

public class TestModel
{
    public string Name { get; set; }

    [Desensitize(Type = DesensitizeType.PhoneNumber)]
    public string PhoneNumber { get; set; }

    public DateTime DateTime { get; set; }
}

public class CreateDto
{
    [Required] public string Name { get; set; }

    [Desensitize(Type = DesensitizeType.PhoneNumber)]
    public string PhoneNumber { get; set; }
}