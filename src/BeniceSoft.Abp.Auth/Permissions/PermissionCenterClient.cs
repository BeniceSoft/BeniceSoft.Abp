using System.Net.Http.Json;
using BeniceSoft.Abp.Auth.Core;
using BeniceSoft.Abp.Auth.Core.Models;
using Volo.Abp;
using Volo.Abp.DependencyInjection;

namespace BeniceSoft.Abp.Auth.Permissions;

public class PermissionCenterClient : IPermissionCenterClient, ITransientDependency
{
    public const string PermissionCenterHttpClientName = "UFX.Auth.PermissionCenter";

    private readonly AuthOptions _authOptions;
    private readonly IHttpClientFactory _httpClientFactory;

    public PermissionCenterClient(AuthOptions options, IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _authOptions = options;
    }

    public async Task<List<RowPermission>?> GetUserRowPermissions(Guid userId, string accessToken)
    {
        using var httpClient = CreateHttpClient();
        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);
        }

        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        httpClient.Timeout = TimeSpan.FromSeconds(30);
        var url = BuildRequestUri($"api/app/am-role-data-auth/role-row-data-auth/{userId}");
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"获取行权限错误 {response.StatusCode}:{response.ReasonPhrase}");
        }

        var result = await response.Content.ReadFromJsonAsync<GetRowDataAuthResponse>();
        if (result?.Code != 200)
        {
            throw new HttpRequestException($"获取行数据权限接口异常 {result?.Code}:{result?.Message}");
        }

        return result.Data.Select(c => new RowPermission()
        {
            TableName = c.TableName,
            ConditionGroups = c.ConditionGroups.Select(d => new RowPermissionConditionGroup()
            {
                LogicalOperator = d.GroupOperator,
                Conditions = d.Conditions.Select(x => new RowPermissionCondition()
                {
                    IsDataSuperAdmin = x.IsDataSuperAdmin,
                    ColumnName = x.ColumnName,
                    Operator = x.Operator.ToString(),
                    Values = x.Values,
                    LogicalOperator = x.LogicalOperator,
                }).ToList()
            }).ToList()
        }).ToList();
    }

    public async Task<List<ColumnPermission>?> GetUserColumnPermissions(Guid userId, string accessToken)
    {
        // 调用am地址，获取数据权限配置
        using var httpClient = CreateHttpClient();

        if (!httpClient.DefaultRequestHeaders.Contains("Authorization"))
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", accessToken);
        }

        httpClient.DefaultRequestHeaders.Add("Connection", "keep-alive");

        httpClient.Timeout = TimeSpan.FromSeconds(30);
        var url = BuildRequestUri($"api/app/am-role-data-auth/role-column-data-auth/{userId}");
        var response = await httpClient.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"获取列数据权限接口异常,错误码{response.StatusCode}:{response.ReasonPhrase}");
        }

        var result = await response.Content.ReadFromJsonAsync<GetColumnDataAuthResponse>();
        if (result?.Code != 200)
        {
            throw new HttpRequestException($"列权限数据Json转换失败");
        }

        return result.Data.Select(c => new ColumnPermission()
        {
            TableName = c.TableName,
            ColumnName = c.ColumnName,
            ColumnAuthLevel = c.ColumnAuthLevel,
            IsDisplay = c.IsDisplay,
        }).ToList();
    }

    private HttpClient CreateHttpClient()
    {
        return _httpClientFactory.CreateClient(PermissionCenterHttpClientName);
    }

    private string BuildRequestUri(string uri)
    {
        Check.NotNullOrWhiteSpace(_authOptions.PermissionCenterUrl, nameof(_authOptions.PermissionCenterUrl));

        return _authOptions.PermissionCenterUrl.EnsureEndsWith('/') + uri;
    }

    class GetRowDataAuthResponse : BaseResponse
    {
        public List<AmRoleAuthRowDataDto> Data { get; set; }
    }

    class AmRoleAuthRowDataDto
    {
        /// <summary>
        /// ����Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// ��ɫ����Ȩ��ID
        /// </summary>
        public string DataMenuId { get; set; }

        /// <summary>
        /// ҵ�����
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// �����鼯��
        /// </summary>
        public List<DataConditionGroup> ConditionGroups { get; set; }
    }

    class DataConditionGroup
    {
        /// <summary>
        /// ��֮��Ĳ�����
        /// and or
        /// </summary>
        public string GroupOperator { get; set; }

        /// <summary>
        /// ���ڵ���������
        /// </summary>
        public List<DataCondition> Conditions { get; set; }
    }

    class DataCondition
    {
        public bool IsDataSuperAdmin { get; set; }

        /// <summary>
        /// �ֶ����ֶ�֮����߼������
        /// and or
        /// </summary>
        public string LogicalOperator { get; set; }

        /// <summary>
        /// �����ֶ�
        /// ҵ�������
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// ����������
        /// ����ȥ���ɱ��ʽ��
        /// </summary>
        public int Operator { get; set; }

        /// <summary>
        /// ����ֵ
        /// </summary>
        public List<string> Values { get; set; }
    }

    class GetColumnDataAuthResponse : BaseResponse
    {
        public List<AmRoleAuthColumnDataDto> Data { get; set; }
    }

    class AmRoleAuthColumnDataDto
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }

        /// <summary>
        /// 列权限等级
        /// 0：隐藏  1：只读  2：读写
        /// </summary>
        public int ColumnAuthLevel { get; set; }

        /// <summary>
        /// 当前列是否显示
        /// </summary>
        public bool IsDisplay { get; set; }
    }
}