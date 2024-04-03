namespace BeniceSoft.Abp.Idempotent.Caching;

public class IdempotencyCacheEntry
{
    public string RequestInflight { get; set; } = string.Empty;

    public ActionResultInfo ActionResult { get; set; } = new();
}

public class ActionResultInfo
{
    public string ResultType { get; set; }
}