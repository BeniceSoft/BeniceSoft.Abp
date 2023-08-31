using BeniceSoft.Abp.AspNetCore;
using BeniceSoft.Abp.AspNetCore.Middlewares;
using BeniceSoft.Abp.Auth;
using BeniceSoft.Abp.Auth.Extensions;
using BeniceSoft.Abp.OperationLogging.EventBus;
using BeniceSoft.Abp.Sample.Application;
using BeniceSoft.Abp.Sample.EntityFrameworkCore;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using Volo.Abp;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.AntiForgery;
using Volo.Abp.Auditing;
using Volo.Abp.Autofac;
using Volo.Abp.EventBus.RabbitMq;
using Volo.Abp.Modularity;

namespace BeniceSoft.Abp.Sample.Host;

[DependsOn(
    typeof(BeniceSoftAbpAspNetCoreModule),
    // typeof(AbpEventBusRabbitMqModule),
    typeof(AbpAutofacModule),
    typeof(SampleApplicationModule),
    typeof(SampleEntityFrameworkCoreModule),
    typeof(BeniceSoftAbpAuthModule),
    typeof(BeniceSoftAbpOperationLoggingEventBusModule)
)]
public class SampleHostModule : AbpModule
{
    public override void ConfigureServices(ServiceConfigurationContext context)
    {
        IdentityModelEventSource.ShowPII = true;
        context.Services.AddHttpClient();
        //动态路由
        Configure<AbpAspNetCoreMvcOptions>(options =>
        {
            options.ConventionalControllers
                .Create(typeof(SampleApplicationModule).Assembly);
        });

        Configure<AbpAuditingOptions>(options =>
        {
            options.IsEnabledForGetRequests = false;
            options.IsEnabled = false;
            options.EntityHistorySelectors.AddAllEntities();
            options.ApplicationName = "PermissionCenter";
        });

        ConfigureSwaggerServices(context.Services);

        Configure<AbpAntiForgeryOptions>(options =>
        {
            options.TokenCookie.Expiration = TimeSpan.Zero;
            options.AutoValidate = false; //表示不验证防伪令牌
        });

        context.Services.AddBeniceSoftAuthentication();
        // context.Services.AddBeniceSoftAuthorization();
    }

    public override void OnApplicationInitialization(ApplicationInitializationContext context)
    {
        var app = context.GetApplicationBuilder();

        app.UseCorrelationId();

        // 路由
        app.UseRouting();

        // 跨域
        app.UseCors(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());

        app.UseBeniceSoftExceptionHandlingMiddleware();

        app.UseAbpRequestLocalization();

        // 身份验证
        app.UseBeniceSoftAuthentication();

        // 认证授权
        // app.UseBeniceSoftAuthorization();

        // 用户权限
        // app.UseBeniceSoftUserPermission();

        app.UseAuditing();
        app.UseSwagger();
        app.UseSwaggerUI(options => { options.SwaggerEndpoint("/swagger/v1/swagger.json", "PermissionCenter API"); });

        // 路由映射
        app.UseConfiguredEndpoints();
    }

    private void ConfigureSwaggerServices(IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo { Title = "Sample API", Version = "0.1" });
            options.DocInclusionPredicate((doc, description) => true);
            options.CustomSchemaIds(type => type.FullName);
            foreach (var item in GetXmlCommentsFilePath())
            {
                options.IncludeXmlComments(item, true);
            }

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                Scheme = "Bearer",
                Description = "Specify the authorization token.",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                    },
                    Array.Empty<string>()
                },
            });
        });
    }

    private List<string> GetXmlCommentsFilePath()
    {
        var basePath = PlatformServices.Default.Application.ApplicationBasePath;
        DirectoryInfo d = new DirectoryInfo(basePath);
        FileInfo[] files = d.GetFiles("*.xml");
        return files.Select(a => Path.Combine(basePath, a.FullName)).ToList();
    }
}