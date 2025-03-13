using Dapper;
using FluentValidation;
using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using RESTfulAPI_Technical_Task_.Composable;
using RESTfulAPI_Technical_Task_.CQRS;
using RESTfulAPI_Technical_Task_.DbContext;
using RESTfulAPI_Technical_Task_.FluentValidator;
using Serilog;
using System.Data;
using System.Text;
using System.Text.Json.Serialization;
using RESTfulAPI_Technical_Task_.BackgroundJobService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHangfireServer();
builder.Services.AddSingleton<JobService>();

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Secret"]))
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddSignalR();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Твой API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Введите JWT токен в формате: Bearer YOUR_TOKEN"
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
            new string[] { }
        }
    });
});


builder.Services.AddScoped<JwtService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration) 
        .ReadFrom.Services(services) 
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
        .MinimumLevel.Information();
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<GetAllTasksQuery>());

builder.Services.AddValidatorsFromAssemblyContaining<TaskValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateTaskValidator>();

builder.Services.AddScoped<IDbConnection>(sp =>
    new NpgsqlConnection(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<DapperConnection>();

builder.Services.AddControllers();

var app = builder.Build();

app.MapGet("/checkPostgreSQLConnection", async (IDbConnection db) =>
{
    var result = await db.QueryAsync<string>("SELECT version();");
    return Results.Ok(result);
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var exception = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
        if (exception != null)
        {
            Log.Error(exception, "Произошла ошибка");
        }

        context.Response.StatusCode = 500;
        await context.Response.WriteAsync("Внутренняя ошибка сервера");
    });
});

app.UseHangfireDashboard("/hangfire");
var jobService = app.Services.GetRequiredService<JobService>();
BackgroundJob.Schedule(() => jobService.RunTask(), TimeSpan.FromMinutes(5));
RecurringJob.AddOrUpdate("MyJob", () => jobService.RunTask(), Cron.MinuteInterval(10));

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
