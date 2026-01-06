using Microsoft.EntityFrameworkCore;
using PaceFriendsBackend.Core.services;
using PaceFriendsBackend.Data;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PaceFriendsDbContext>(options =>
    options.UseMySql(
        connectionString, 
        ServerVersion.AutoDetect(connectionString)
    ));
builder.Services.AddScoped<PaceFriendsRepository>();

builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("EndOfDayJob");
    q.AddJob<EndOfDayJob>(opts => opts.WithIdentity(jobKey));

    q.AddTrigger(opts => opts
        .ForJob(jobKey)
        .WithIdentity("EndOfDayTrigger")
        .WithCronSchedule("59 59 00 * * ?", x => x.InTimeZone(TimeZoneInfo.Utc)));
    
    var weeklyJobKey = new JobKey("EndOfWeekJob");
    q.AddJob<EndOfWeekJob>(opts => opts.WithIdentity(weeklyJobKey));

    q.AddTrigger(opts => opts
        .ForJob(weeklyJobKey)
        .WithIdentity("EndOfWeekTrigger")
        .WithCronSchedule("59 59 00 ? * MON", x => x.InTimeZone(TimeZoneInfo.Utc))); 
});

builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});
builder.Services.AddOpenApi(); 

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.Run();