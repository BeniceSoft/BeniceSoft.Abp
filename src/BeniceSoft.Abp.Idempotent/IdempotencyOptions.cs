using BeniceSoft.Abp.Core.Constants;

namespace BeniceSoft.Abp.Idempotent;

public class IdempotencyOptions
{
    public string IdempotentKeyName { get; set; } = BeniceSoftHttpConstant.RequestId;
}