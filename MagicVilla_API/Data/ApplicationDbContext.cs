using MagicVilla_API.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base( options )
    {
        
    }
    public DbSet<Villa> Villas { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
        modelBuilder.Entity<Villa>().HasData(
            new Villa() 
            {
                Id = 1, 
                Nombre = "Villa Real", 
                Detalle = "Detalle Villa Real",
                ImageUrl = "",
                Ocupantes = 5,
                Area = 60,
                Tarifa = 200,
                Amenidad = "",
                FechaCreacion = DateTime.Now,
                FechaActualizacion = DateTime.Now,
            },
			new Villa()
			{
				Id = 2,
				Nombre = "Villa Maria",
				Detalle = "Detalle Villa Maria",
				ImageUrl = "",
				Ocupantes = 3,
				Area = 45,
				Tarifa = 150,
				Amenidad = "",
				FechaCreacion = DateTime.Now,
				FechaActualizacion = DateTime.Now,
			}
		);
	}
}
