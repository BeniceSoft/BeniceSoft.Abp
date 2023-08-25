namespace BeniceSoft.Abp.Extensions.Caching.Configurations;

/// <summary>
/// 配置项
/// </summary>
public class BeniceSoftCachingConfiguration
{
    /// <summary>
    /// 缓存键前缀
    /// </summary>
    public string CacheKeyPrefix { get; set; } = "BeniceSoft:Cached:";

    /// <summary>
    /// 默认过期秒数
    /// </summary>
    public int DefaultExpirationSeconds { get; set; } = 3600;


    private static BeniceSoftCachingConfiguration? _instance = null;
    private static readonly object Padlock = new object();

    private BeniceSoftCachingConfiguration()
    {
    }

    public static BeniceSoftCachingConfiguration Instance
    {
        get
        {
            if (_instance is null)
            {
                lock (Padlock)
                {
                    _instance ??= new BeniceSoftCachingConfiguration();
                }
            }

            return _instance;
        }
    }
}