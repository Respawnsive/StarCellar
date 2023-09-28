using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.OpenApi.Models;
using StarCellar.Api.Data;
using StarCellar.Api.Handlers;
using StarCellar.Api.Utils;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

// Set
var isProduction = builder.Environment.IsProduction();
var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
var version = $"{versionInfo.FileMajorPart}.{versionInfo.ProductMinorPart}.{versionInfo.ProductBuildPart}";

// Configure
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.Converters.Add(new UserConverter());
});

// Add
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc($"v1", new OpenApiInfo
    {
        Title = "Apizr - StarCellar demo",
        Contact = new OpenApiContact
        {
            Name = "Respawnsive",
            Email = "contact@respawnsive.com",
            Url = new Uri("https://respawnsive.com")
        },
        Description = "This is a demo api for the Apizr client library. Do not use this API for production. For more information please visit https://apizr.net",
        Version = version
    });

    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.ApiKey,
            In = ParameterLocation.Header,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            Description =
                "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        }
    );

    options.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Name = "Bearer",
                    In = ParameterLocation.Header,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        }
    );
});

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDbContext<UserRefreshTokenDbContext>(optionsBuilder => optionsBuilder.UseInMemoryDatabase("UserRefreshTokens"));

builder.Services.AddSingleton<TokenGenerator>();
builder.Services.AddSingleton<TokenValidator>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services
    .AddIdentityCore<User>(options =>
    {
        options.User.RequireUniqueEmail = true;
        options.Password.RequireDigit = isProduction;
        options.Password.RequireLowercase = isProduction;
        options.Password.RequireNonAlphanumeric = isProduction;
        options.Password.RequireUppercase = isProduction;
        if (isProduction)
        {
            options.Password.RequiredLength = 8;
            options.Password.RequiredUniqueChars = 3;
        }
    })
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:AccessTokenSecret"]!)),
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
        options.SaveToken = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(
        Constants.Policies.Admin,
        policy => policy.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, Constants.Roles.Admin)
    );
    options.AddPolicy(
        Constants.Policies.User,
        policy => policy.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, Constants.Roles.User)
    );
    options.AddPolicy(
        Constants.Policies.Any,
        policy => policy.RequireAuthenticatedUser().RequireClaim(ClaimTypes.Role, Constants.Roles.Admin, Constants.Roles.User)
    );
});

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

var app = builder.Build();

// Register
app.Lifetime.ApplicationStarted.Register(() =>
{
    if (Directory.Exists(Constants.DirectoryPath))
        Directory.Delete(Constants.DirectoryPath, true);

    Directory.CreateDirectory(Constants.DirectoryPath);
});

// Use
app.UseHttpsRedirection();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(app.Environment.ContentRootPath, "Uploads")),
    RequestPath = "/uploads"
});

// Map
app.Map("/", async context =>
{
    await Task.CompletedTask;
    context.Response.Redirect("/swagger");
});

app.MapPost("signup", UsersHandler.SignUpAsync);
app.MapPost("signin", UsersHandler.SignInAsync);
app.MapPost("refresh", UsersHandler.RefreshTokenAsync);
app.MapPost("signout", UsersHandler.SignOutAsync).RequireAuthorization(Constants.Policies.Any);
app.MapGet("profile", UsersHandler.GetProfileAsync).RequireAuthorization(Constants.Policies.Any);

app.MapPost("/upload", FilesHandler.UploadAsync);

var wineRoutes = app.MapGroup("/wines");//.RequireAuthorization(Constants.Policies.Any);
wineRoutes.MapGet("/", WinesHandler.GetAllWines).WithOpenApi();
wineRoutes.MapGet("/{id}", WinesHandler.GetWine).WithOpenApi();
wineRoutes.MapPost("/", WinesHandler.CreateWine).WithOpenApi();
wineRoutes.MapPut("/{id}", WinesHandler.UpdateWine).WithOpenApi();
wineRoutes.MapDelete("/{id}", WinesHandler.DeleteWine).WithOpenApi();

// Run
app.Run();