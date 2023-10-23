using MagicVilla_API.Models;

namespace MagicVilla_API.Repository.IRepository;

public interface INumeroVillaRepository : IRepository<NumeroVilla>
{
	Task<NumeroVilla> Update(NumeroVilla entidad);
}
