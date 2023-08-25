namespace BeniceSoft.Abp.Auth.Core.Entity;

public interface IHaveOwnerId
{
    /// <summary>
    /// 拥有者Id
    /// </summary>
    public Guid OwnerId { get; }
}