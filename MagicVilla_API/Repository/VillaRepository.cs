using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Repository.IRepository;

namespace MagicVilla_API.Repository;

public class VillaRepository : Repository<Villa>, IVillaRepository
{
	private readonly ApplicationDbContext dbContext;

	public VillaRepository(ApplicationDbContext dbContext) : base(dbContext)
	{
		this.dbContext = dbContext;
	}

	public async Task<Villa> Update(Villa entidad)
	{
		entidad.FechaActualizacion = DateTime.Now;
		dbContext.Update(entidad);
		await dbContext.SaveChangesAsync(); 
		return entidad;
	}
}
