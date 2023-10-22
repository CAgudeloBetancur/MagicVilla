using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VillaController : ControllerBase
{
	private readonly ILogger<VillaController> logger;
	private readonly ApplicationDbContext dbContext;

	public VillaController(
		ILogger<VillaController> logger,
		ApplicationDbContext dbContext
	)
    {
		this.logger = logger;
		this.dbContext = dbContext;
	}

    [HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task< ActionResult < IEnumerable<VillaDTO> > > GetVillas()
	{
		logger.LogInformation("Obtener todas las Villas");
		return Ok(await dbContext.Villas.ToListAsync());
	}

	[HttpGet("{id:int}", Name = "GetVilla")]
	[ProducesResponseType(StatusCodes.Status200OK)] // Una forma de indicar status
	[ProducesResponseType(400)] // Otra forma de indicar los status
	[ProducesResponseType(404)]
	public async Task< ActionResult<VillaDTO> > GetVilla(int id)
	{
		if(id == 0)
		{
			logger.LogError($"Error al intentar traer la Villa con el Id: {id}");
			return BadRequest();
		}

		var villa = await dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);

		if(villa == null)
		{
			return NotFound();
		}
		

		return Ok(villa);
	}

	[HttpPost]
	[ProducesResponseType(201)]
	[ProducesResponseType(400)]
	[ProducesResponseType(500)]
	public async Task< ActionResult<VillaDTO> > CrearVilla([FromBody] VillaDTO villaDTO)
	{
		if(!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		if (
			await dbContext.Villas.FirstOrDefaultAsync(
				x => x.Nombre.ToLower() == villaDTO.Nombre.ToLower()
			) != null
		)	
		{
			ModelState.AddModelError("NombreExistente", "Existe un registro con ese nombre");
			return BadRequest(ModelState);
		}

		if(villaDTO == null)
		{
			return BadRequest();
		}

		if(villaDTO.Id > 0)
		{
			return StatusCode(StatusCodes.Status500InternalServerError);
		}

		Villa modelo = new()
		{
			Nombre = villaDTO.Nombre,
			Detalle = villaDTO.Detalle,
			Ocupantes = villaDTO.Ocupantes,
			Area = villaDTO.Area,
			Tarifa = villaDTO.Tarifa,
			ImageUrl = villaDTO.ImageUrl,
			Amenidad = villaDTO.Amenidad
		};

		await dbContext.Villas.AddAsync(modelo);
		await dbContext.SaveChangesAsync();

		// Retorno normal - Tradicional
		//return Ok(villaDTO);

		// Retorno recomendado

		return CreatedAtRoute("GetVilla", new { id = villaDTO.Id }, villaDTO);
	}

	[HttpDelete("{id:int}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task<IActionResult> DeleteVilla(int id)
	{
		if (id == 0)
		{
			return BadRequest();
		}

		var villa = await dbContext.Villas.FirstOrDefaultAsync(v => v.Id == id);

		if (villa is null)
		{
			return NotFound();
		}

		dbContext.Villas.Remove(villa);
		await dbContext.SaveChangesAsync();

		return NoContent();

	}

	[HttpPut("{id:int}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	public async Task< IActionResult > UpdateVilla(int id, [FromBody] VillaDTO villaDTO)
	{
		if(villaDTO == null || id != villaDTO.Id)
		{
			return BadRequest();
		}

		Villa modelo = new()
		{
			Id = villaDTO.Id,
			Nombre = villaDTO.Nombre,
			Detalle = villaDTO.Detalle,
			Ocupantes = villaDTO.Ocupantes,
			Tarifa = villaDTO.Tarifa,
			Area = villaDTO.Area,
			ImageUrl = villaDTO.ImageUrl,
			Amenidad = villaDTO.Amenidad,
		};

		dbContext.Villas.Update(modelo);
		await dbContext.SaveChangesAsync();

		return NoContent();
	}

	[HttpPatch("{id:int}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	public async Task< IActionResult > UpdatePartialVilla(int id, JsonPatchDocument<VillaDTO> patchDTO) 
	{
		if (patchDTO is null || id == 0)
		{
			return BadRequest();
		}

		var villa = await dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

		if(villa is null) return BadRequest();

		VillaDTO villaDto = new()
		{
			Id = villa.Id,
			Nombre = villa.Nombre,
			Detalle = villa.Detalle,
			Ocupantes = villa.Ocupantes,
			Tarifa = villa.Tarifa,
			Area = villa.Area,
			ImageUrl = villa.ImageUrl,
			Amenidad = villa.Amenidad
		};


		patchDTO.ApplyTo(villaDto, ModelState);

		if(!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		Villa modelo = new()
		{
			Id = villaDto.Id,
			Nombre = villaDto.Nombre,
			Detalle = villaDto.Detalle,
			Ocupantes = villaDto.Ocupantes,
			Tarifa = villaDto.Tarifa,
			Area = villaDto.Area,
			ImageUrl = villaDto.ImageUrl,
			Amenidad = villaDto.Amenidad
		};

		dbContext.Villas.Update(modelo);
		await dbContext.SaveChangesAsync();

		return NoContent();
	}
}
