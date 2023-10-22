using MagicVilla_API.Models.DTO;

namespace MagicVilla_API.Data
{
	public static class VillaStore
	{
		public static List<VillaDTO> villaList = new List<VillaDTO>
		{
			new VillaDTO{ Id = 1, Nombre = "Vista a la Piscina", Ocupantes = 5, Area = 75},
			new VillaDTO{ Id = 2, Nombre = "Vista a la playa", Ocupantes = 10, Area = 120}
		};
	}
}
