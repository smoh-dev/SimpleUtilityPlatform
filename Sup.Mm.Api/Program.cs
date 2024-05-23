using Sup.Mm.Api.Repositories;
using Sup.Mm.Api.Services;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
const string configFileName = "appsettings.Development.json";
#else
const string configFileName = "appsettings.json";
#endif

builder.Configuration.AddJsonFile(configFileName, optional: true, reloadOnChange: true);

// Add services to the container.
builder.Services.AddScoped<IDbRepository, PostgresRepository>();

builder.Services.AddScoped<NoteService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.MapControllers();

app.Run();