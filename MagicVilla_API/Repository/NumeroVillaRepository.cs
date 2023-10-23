using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;

namespace MagicVilla_API.Repository;

public class NumeroVillaRepository : Repository<NumeroVilla>, INumeroVillaRepository
{
	private readonly ApplicationDbContext dbContext;

    public NumeroVillaRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
		this.dbContext = dbContext;
	}

	public async Task<NumeroVilla> Update(NumeroVilla entidad)
	{
		entidad.FechaActualizacion = DateTime.Now;
		dbContext.NumeroVillas.Update(entidad);
		await dbContext.SaveChangesAsync(); 
		return entidad;
	}
}
