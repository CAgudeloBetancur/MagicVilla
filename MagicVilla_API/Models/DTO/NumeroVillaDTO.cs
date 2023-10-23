using System.ComponentModel.DataAnnotations;

namespace MagicVilla_API.Models.DTO;

public class NumeroVillaDTO
{
    [Required]
    public int VillaNo { get; set; }

    [Required]
    public int VillaId { get; set; }

    public string DetalleEspecial { get; set; }
}
