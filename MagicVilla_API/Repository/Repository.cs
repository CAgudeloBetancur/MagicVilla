using MagicVilla_API.Data;
using MagicVilla_API.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace MagicVilla_API.Repository;

public class Repository<T> : IRepository<T> where T : class
{
	private readonly ApplicationDbContext dbContext;
	internal DbSet<T> dbSet;

	public Repository(ApplicationDbContext dbContext)
    {
		this.dbContext = dbContext;
		this.dbSet = dbContext.Set<T>();
	}

    public async Task Crear(T entity)
	{
		await dbSet.AddAsync(entity);
		await Grabar();
	}

	public async Task Grabar()
	{
		await dbContext.SaveChangesAsync();
	}

	public async Task<T> Obtener(Expression<Func<T, bool>>? filter = null, bool tracked = true)
	{
		IQueryable<T> query = dbSet;

		if(!tracked)
		{
			query = query.AsNoTracking();
		}

		if(filter != null)
		{
			query = query.Where(filter);
		}

		return await query.FirstOrDefaultAsync();
	}

	public async Task<List<T>> ObtenerTodos(Expression<Func<T, bool>>? filter = null)
	{
		IQueryable<T> query = dbSet;

		if(filter != null)
		{
			query = query.Where(filter);
		}

		return await query.ToListAsync();
	}

	public async Task Remover(T entity)
	{
		dbSet.Remove(entity);
		await Grabar();
	}
}
