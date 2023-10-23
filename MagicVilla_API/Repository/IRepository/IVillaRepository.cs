using MagicVilla_API.Models;

namespace MagicVilla_API.Repository.IRepository;

public interface IVillaRepository : IRepository<Villa>
{
	Task<Villa> Update(Villa entidad);
}
