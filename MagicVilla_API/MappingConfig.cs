using AutoMapper;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;

namespace MagicVilla_API;

public class MappingConfig : Profile
{
    public MappingConfig()
    {
        // < Fuente, Destino >
        
        // Forma explícita de indicar los mapeos
        CreateMap<Villa, VillaDTO>(); 
        CreateMap<VillaDTO, Villa>(); // Inverso

        // Forma corta de indicar mapeos en una línea
        CreateMap<Villa, CreateVillaDTO>().ReverseMap();
        CreateMap<Villa, UpdateVillaDTO>().ReverseMap();

    }
}
