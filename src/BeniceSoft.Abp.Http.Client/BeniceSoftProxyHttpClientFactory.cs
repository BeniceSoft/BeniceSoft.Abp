using BeniceSoft.Abp.Core.Constants;
using Microsoft.AspNetCore.Http;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Http.Client.Proxying;

namespace BeniceSoft.Abp.Http.Client;

[ExposeServices(typeof(IProxyHttpClientFactory))]
public class BeniceSoftProxyHttpClientFactory : IProxyHttpClientFactory, ITransientDependency
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BeniceSoftProxyHttpClientFactory(IHttpClientFactory httpClientFactory, IHttpContextAccessor httpContextAccessor)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
    }

    public HttpClient Create()
    {
        return Create(Microsoft.Extensions.Options.Options.DefaultName);
    }

    public HttpClient Create(string name)
    {
        var httpClient = _httpClientFactory.CreateClient();

        // 不做全局的响应格式化
        httpClient.DefaultRequestHeaders.Add(BeniceSoftHttpConstant.IgnoreJsonFormat, bool.TrueString);
        
        // 标识请求来自远程服务调用
        httpClient.DefaultRequestHeaders.Add(BeniceSoftHttpConstant.RequestedFrom, BeniceSoftHttpConstant.RequestedFromRemoteServiceCall);
        
        // Authorization
        if (!httpClient.DefaultRequestHeaders.Contains(BeniceSoftHttpConstant.Authorization) &&
            _httpContextAccessor.HttpContext?.Request.Headers.TryGetValue(BeniceSoftHttpConstant.Authorization, out var accessToken) == true)
        {
            httpClient.DefaultRequestHeaders.Add(BeniceSoftHttpConstant.Authorization, (string?)accessToken ?? string.Empty);
        }

        return httpClient;
    }
}