using System.Threading.Tasks;
using BeniceSoft.Abp.Idempotent;
using BeniceSoft.Abp.Idempotent.Caching;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Idempotent.AspNetCore;

public class IdempotencyFilter : IAsyncActionFilter, IAsyncResultFilter, ITransientDependency
{
    private Idempotency? _idempotency;

    private readonly ILogger<IdempotencyFilter> _logger;
    private readonly IIdempotencyCache _cache;
    private readonly IdempotencyOptions _options;

    public IdempotencyFilter(ILogger<IdempotencyFilter> logger, IIdempotencyCache cache, IOptions<IdempotencyOptions> options)
    {
        _logger = logger;
        _cache = cache;
        _options = options.Value;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        _idempotency ??= new Idempotency(_logger, _cache, _options.IdempotentKeyName);
        await _idempotency.ApplyPreIdempotency(context);

        // 如果结果已经设置，直接退出
        if (context.Result is not null)
        {
            return;
        }

        var result = await next();
        if (result.Exception is not null)
        {
            await _idempotency.CancelIdempotency();
        }
    }

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        await next();

        if (_idempotency is null) return;

        await _idempotency.ApplyPostIdempotency(context);
    }
}