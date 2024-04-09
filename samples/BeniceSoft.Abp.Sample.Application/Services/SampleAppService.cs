using System.ComponentModel.DataAnnotations;
using BeniceSoft.Abp.Core.Attributes;
using BeniceSoft.Abp.OperationLogging.Abstractions;
using BeniceSoft.Abp.Sample.Domain.Shared.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace BeniceSoft.Abp.Sample.Application.Services;

public class SampleAppService : ApplicationService
{
    public void Exception()
    {
        throw new BizException(999, "this is biz exception");
    }

    [OperationLog(OperationType = "Create", BizModule = "Sample")]
    public virtual async Task<CreateDto> CreateAsync(CreateDto dto, [IgnoreBind] OperationLogContext? context = null)
    {
        context?.SetValue(Guid.NewGuid().ToString(), "BizCode0001", "", new()
        {
            { "key1", "value1" }
        });

        return dto;
    }

    public IEnumerable<string> GetStrings()
    {
        return Enumerable.Range(1, 100).Select(x => x.ToString());
    }

    public List<TestModel> List()
    {
        return new()
        {
            new() { Name = "zhangsan", PhoneNumber = "13000000001" },
            new() { Name = "lisi", PhoneNumber = "13000000002" },
        };
    }

    [Authorize(AuthenticationSchemes = "Bearer")]
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