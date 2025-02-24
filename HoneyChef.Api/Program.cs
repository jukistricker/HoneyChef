using IOITCore.Repositories.Interfaces;
using IOITCore.Repositories;
using Newtonsoft.Json.Serialization;
using IOITCore.Services;
using IOITCore.Services.Interfaces;
using IOITCore.Persistence;
using IOITCore.Mappings;
using System.Reflection;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using IOITCore.Services.Common;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.ResponseCompression;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = long.MaxValue;
});
builder.Services.Configure<FormOptions>(options =>
{
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartBodyLengthLimit = long.MaxValue; // <-- ! long.MaxValue
    options.MultipartBoundaryLengthLimit = int.MaxValue;
    options.MultipartHeadersCountLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
});
//builder.Services.Configure<CookiePolicyOptions>(options =>
//{
//    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
//    options.OnAppendCookie = cookieContext =>
//        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
//    options.OnDeleteCookie = cookieContext =>
//        CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
//});
// Add services to the container.
builder.Services.AddControllersWithViews(options =>
                options.Filters.Add(typeof(IOITCore.Filters.GlobalExceptionFilterAttribute))).AddNewtonsoftJson(
                (options =>
                {
                    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                }));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(600);//You can set Time  
    options.Cookie.HttpOnly = false;
    options.Cookie.IsEssential = true;
});

//
builder.Services.AddTransient<IFunctionRepository, FunctionRepository>();
builder.Services.AddTransient<IFunctionRoleRepository, FunctionRoleRepository>();
builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserRoleRepository, UserRoleRepository>();
builder.Services.AddTransient<ILogActionRepository, LogActionRepository>();
//
builder.Services.AddTransient<IUnitOfWork, UnitOfWork>();
//
builder.Services.AddTransient<IFunctionRoleService, FunctionRoleService>();
builder.Services.AddTransient<IFunctionService, FunctionService>();
builder.Services.AddTransient<IUserRoleService, UserRoleService>();
builder.Services.AddTransient<IUserService, UserService>();
//builder.Services.AddTransient<IOfficeService, OfficeService>();
//
builder.Services.AddScoped<ICacheService, CacheService>();

builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
//builder.Services.AddScoped<HttpContextAccessor>();
// Mssql Configuration
builder.Services.AddDbContext<IOITDbContext>(options =>
    options.ConfigureWarnings(b => b.Log(CoreEventId.ManyServiceProvidersCreatedWarning))
    .UseSqlServer(builder.Configuration.GetConnectionString("DbConnection")));

// General Configuration
builder.Services.AddAutoMapper(typeof(Program));
var coreMappingAssembly = typeof(AutoMapping).Assembly;
builder.Services.AddAutoMapper(coreMappingAssembly);
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());
//
builder.Services.AddCors((options => {
    options.AddPolicy("addcors", builder =>
    builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader()
    .AllowCredentials());
}));
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HoneyChef", Version = "v1", Description = "APis are built for IOITCore system by IOIT" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}
                    }
                });
});

string domain = builder.Configuration["AppSettings:JwtIssuer"];
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

}).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = false;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = domain,
        ValidAudience = domain,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["AppSettings:JwtKey"])),
        ClockSkew = TimeSpan.Zero // remove delay of token when expire
    };
});
//
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true; // Bật nén cả với HTTPS
    options.Providers.Add<GzipCompressionProvider>(); // Sử dụng Gzip
});

builder.Services.Configure<GzipCompressionProviderOptions>(options =>
{
    options.Level = System.IO.Compression.CompressionLevel.Fastest; // Cấu hình mức nén
});
//
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseHsts();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IOITCore API By IOIT");
});

app.UseCors("addcors");
app.UseResponseCompression();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCookiePolicy();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllers();

app.Run();

//void CheckSameSite(HttpContext httpContext, CookieOptions options)
//{
//    if (options.SameSite == SameSiteMode.None)
//    {
//        var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
//        if (DisallowsSameSiteNone(userAgent))
//        {
//            options.SameSite = SameSiteMode.Unspecified;
//        }
//    }
//}

//static bool DisallowsSameSiteNone(string userAgent)
//{
//    // Check if a null or empty string has been passed in, since this
//    // will cause further interrogation of the useragent to fail.
//    if (String.IsNullOrWhiteSpace(userAgent))
//        return false;

//    // Cover all iOS based browsers here. This includes:
//    // - Safari on iOS 12 for iPhone, iPod Touch, iPad
//    // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
//    // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
//    // All of which are broken by SameSite=None, because they use the iOS networking
//    // stack.
//    if (userAgent.Contains("CPU iPhone OS 12") ||
//        userAgent.Contains("iPad; CPU OS 12"))
//    {
//        return true;
//    }

//    // Cover Mac OS X based browsers that use the Mac OS networking stack. 
//    // This includes:
//    // - Safari on Mac OS X.
//    // This does not include:
//    // - Chrome on Mac OS X
//    // Because they do not use the Mac OS networking stack.
//    if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") &&
//        userAgent.Contains("Version/") && userAgent.Contains("Safari"))
//    {
//        return true;
//    }

//    // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
//    // and none in this range require it.
//    // Note: this covers some pre-Chromium Edge versions, 
//    // but pre-Chromium Edge does not require SameSite=None.
//    if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
//    {
//        return true;
//    }

//    return false;
//}