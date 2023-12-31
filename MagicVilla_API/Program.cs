using MagicVilla_API;
using MagicVilla_API.Data;
using MagicVilla_API.Repository;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// ** Con el m�todo .AddNewtonSoftJson() agregamos el servicio para json patch

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ** Registrando DbContext y Connection String
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
	options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// ** Registrando el AutoMapper

builder.Services.AddAutoMapper( typeof(MappingConfig) );

// ** Registrando Repositorios

builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<INumeroVillaRepository, NumeroVillaRepository>();

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
