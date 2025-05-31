using AutoMapper;
using Museum.BLL.Mapping;
using Museum.BLL.Services;
using Museum.DAL.Context;
using Museum.DAL.Interfaces;
using Museum.DAL.UoW;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// DbContext
builder.Services.AddDbContext<MuseumContext>();

// AutoMapper
builder.Services.AddAutoMapper(typeof(MuseumProfile));

// Unit of Work and Services
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ExhibitionService>();
builder.Services.AddScoped<ScheduleService>();
builder.Services.AddScoped<VisitService>();
builder.Services.AddScoped<TourService>();
builder.Services.AddScoped<ReportService>();

// Swagger configuration
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Museum API",
        Version = "v1",
        Description = "API for Museum Management System",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@example.com"
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Museum API V1");
        c.RoutePrefix = "swagger"; // Set Swagger UI at the root
    });
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();