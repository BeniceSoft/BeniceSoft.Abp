using System;
using System.Linq;
using System.Threading.Tasks;
using BeniceSoft.Abp.Idempotent.Caching;
using BeniceSoft.Abp.Idempotent.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Volo.Abp;

namespace BeniceSoft.Abp.Idempotent.AspNetCore;

public class Idempotency
{
    private readonly ILogger _logger;
    private readonly IIdempotencyCache _cache;
    private readonly string _headerKeyName;

    public Idempotency(ILogger logger, IIdempotencyCache cache, string headerKeyName)
    {
        _logger = logger;
        _cache = cache;
        _headerKeyName = Check.NotNullOrWhiteSpace(headerKeyName, nameof(headerKeyName));
    }

    private bool _isPreIdempotencyApplied = false;
    private bool _isPreIdempotencyCacheReturned = false;
    private string _idempotencyKey = string.Empty;

    public async Task ApplyPreIdempotency(ActionExecutingContext context)
    {
        _logger.LogInformation(
            "{0} [Before Controller execution]: Request for {1}: {2} received ({3} bytes)",
            nameof(IdempotencyFilter), context.HttpContext.Request.Method, context.HttpContext.Request.Path, context.HttpContext.Request.ContentLength);

        // Check if Idempotency can be applied:
        if (!CanPerformIdempotency(context.HttpContext.Request))
        {
            return;
        }

        // Try to get the IdempotencyKey value from header:
        if (!TryGetIdempotencyKey(context.HttpContext.Request, out _idempotencyKey))
        {
            // context.Result = null;
            return;
        }

        // Check if idempotencyKey exists in cache and return value:
        var uniqueRequestId = Guid.NewGuid().ToString("N");
        IdempotencyCacheEntry cacheEntry;
        try
        {
            cacheEntry = await _cache.GetOrSetAsync(
                    _idempotencyKey,
                    GenerateRequestInFlightCacheEntry(uniqueRequestId))
                .ConfigureAwait(false);
        }
        catch (DistributedLockNotAcquiredException distributedLockNotAcquiredException)
        {
            LogDistributedLockNotAcquiredException("Before Controller", distributedLockNotAcquiredException);
        
            context.Result = new ConflictResult();
            return;
        }
        
        // RPG - 2021-07-05 - Check if there is a copy of this request in flight,
        // if so return a 409 Http Conflict response.
        if (uniqueRequestId != cacheEntry.RequestInflight)
        {
            context.Result = new ConflictResult();
            return;
        }
        
        // if (!cacheData.ContainsKey("Request.Inflight"))
        // {
        //     // 2019-07-06: Evaluate the "Request.DataHash" in order to be sure that the cached
        //     // response is returned for the same combination of IdempotencyKey and Request
        //     string cachedRequestDataHash = cacheData["Request.DataHash"].ToString();
        //     string currentRequestDataHash = await GetRequestsDataHash(context.HttpContext.Request)
        //         .ConfigureAwait(false);
        //     if (cachedRequestDataHash != currentRequestDataHash)
        //     {
        //         context.Result = new BadRequestObjectResult($"The Idempotency header key value '{_idempotencyKey}' was used in a different request.");
        //         return;
        //     }
        //
        //     // Set the StatusCode and Response result (based on the IActionResult type)
        //     // The response body will be created from a .NET middle-ware in a following step.
        //     int responseStatusCode = Convert.ToInt32(cacheData["Response.StatusCode"]);
        //
        //     var actionResult = cacheData.ActionResult;
        //     var contextResultType = Type.GetType(actionResult.ResultType);
        //     if (contextResultType == null)
        //     {
        //         throw new NotImplementedException($"ApplyPreIdempotency, ResultType {actionResult.ResultType} is not recognized");
        //     }
        //
        //     // Initialize the IActionResult based on its type:
        //     if (contextResultType == typeof(CreatedAtRouteResult))
        //     {
        //         object value = resultObjects["ResultValue"];
        //         string routeName = (string)resultObjects["ResultRouteName"];
        //         Dictionary<string, string> RouteValues = (Dictionary<string, string>)resultObjects["ResultRouteValues"];
        //
        //         context.Result = new CreatedAtRouteResult(routeName, RouteValues, value);
        //     }
        //     else if (contextResultType.BaseType == typeof(ObjectResult)
        //              || contextResultType == typeof(ObjectResult))
        //     {
        //         object value = resultObjects["ResultValue"];
        //         ConstructorInfo ctor = contextResultType.GetConstructor(new[] { typeof(object) });
        //         if (ctor != null && ctor.DeclaringType != typeof(ObjectResult))
        //         {
        //             context.Result = (IActionResult)ctor.Invoke(new object[] { value });
        //         }
        //         else
        //         {
        //             context.Result = new ObjectResult(value) { StatusCode = responseStatusCode };
        //         }
        //     }
        //     else if (contextResultType.BaseType == typeof(StatusCodeResult)
        //              || contextResultType.BaseType == typeof(ActionResult))
        //     {
        //         ConstructorInfo ctor = contextResultType.GetConstructor(Array.Empty<Type>());
        //         if (ctor != null)
        //         {
        //             context.Result = (IActionResult)ctor.Invoke(Array.Empty<object>());
        //         }
        //     }
        //     else
        //     {
        //         throw new NotImplementedException($"ApplyPreIdempotency is not implemented for IActionResult type {contextResultType}");
        //     }
        //
        //     // Include cached headers (if does not exist) at the response:
        //     Dictionary<string, List<string>> headerKeyValues = (Dictionary<string, List<string>>)cacheData["Response.Headers"];
        //     if (headerKeyValues != null)
        //     {
        //         foreach (KeyValuePair<string, List<string>> headerKeyValue in headerKeyValues)
        //         {
        //             if (!context.HttpContext.Response.Headers.ContainsKey(headerKeyValue.Key))
        //             {
        //                 context.HttpContext.Response.Headers.Add(headerKeyValue.Key, headerKeyValue.Value.ToArray());
        //             }
        //         }
        //     }
        //
        //     _logger.LogInformation("IdempotencyFilterAttribute [Before Controller]: Return result from idempotency cache (of type {contextResultType})",
        //         contextResultType.ToString());
        //
        //     _isPreIdempotencyCacheReturned = true;
        // }

        _logger.LogInformation($"{nameof(IdempotencyFilter)} [Before Controller]: End");

        _isPreIdempotencyApplied = true;
    }

    public async Task ApplyPostIdempotency(ResultExecutingContext context)
    {
    }

    public async Task CancelIdempotency()
    {
    }

    private bool CanPerformIdempotency(HttpRequest httpRequest)
    {
        // Idempotency is applied on Post & Patch Http methods:
        if (httpRequest.Method != HttpMethods.Post
            && httpRequest.Method != HttpMethods.Patch)
        {
            _logger.LogInformation("{0} [Before Controller execution]: Idempotency SKIPPED, httpRequest Method is: {1}",
                nameof(IdempotencyFilter), httpRequest.Method);

            return false;
        }

        // For multiple executions of the PreStep:
        if (_isPreIdempotencyApplied)
        {
            return false;
        }

        return true;
    }

    private bool TryGetIdempotencyKey(HttpRequest httpRequest, out string idempotencyKey)
    {
        idempotencyKey = string.Empty;
        
        if (httpRequest.Headers.TryGetValue(_headerKeyName, out var idempotencyKeys) &&
            !string.IsNullOrWhiteSpace(idempotencyKeys.FirstOrDefault()))
        {
            idempotencyKey = idempotencyKeys.FirstOrDefault()!;
            return true;
        }

        return false;
    }

    private void LogDistributedLockNotAcquiredException(string message, DistributedLockNotAcquiredException exception)
    {
        if (exception.InnerException is not null)
        {
            _logger.LogError(
                exception.InnerException,
                $"{nameof(IdempotencyFilter)} [{message}]: DistributedLockNotAcquired. {exception.Message}");
        }
        else
        {
            _logger.LogWarning($"{nameof(IdempotencyFilter)} [{message}]: DistributedLockNotAcquired. {exception.Message}");
        }
    }

    private IdempotencyCacheEntry GenerateRequestInFlightCacheEntry(string guid)
    {
        var entry = new IdempotencyCacheEntry()
        {
            RequestInflight = guid
        };

        return entry;
    }
}