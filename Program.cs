using Microsoft.EntityFrameworkCore;
using PaceFriendsBackend.Data;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<PaceFriendsDbContext>(options =>
    options.UseMySql(
        connectionString, 
        ServerVersion.AutoDetect(connectionString)
    ));
builder.Services.AddScoped<PaceFriendsRepository>();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
{
    // This line enables "Jogging" -> Activity.Jogging conversion
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