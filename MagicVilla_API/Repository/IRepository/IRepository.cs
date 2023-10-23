using System.Linq.Expressions;

namespace MagicVilla_API.Repository.IRepository;

public interface IRepository<T> where T : class
{
	Task Crear(T entity);

	Task< List<T> > ObtenerTodos(Expression< Func<T,bool> >? filter = null);

	Task<T> Obtener(Expression< Func<T, bool> >? filter = null, bool tracked = true);

	Task Remover(T entity);

	Task Grabar();
}
