using AutoMapper;
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
	private readonly IMapper mapper;

	public VillaController(
		ILogger<VillaController> logger,
		ApplicationDbContext dbContext,
		IMapper mapper
	)
    {
		this.logger = logger;
		this.dbContext = dbContext;
		this.mapper = mapper;
	}

    [HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task< ActionResult < IEnumerable<VillaDTO> > > GetVillas()
	{
		logger.LogInformation("Obtener todas las Villas");

		IEnumerable<Villa> villaList = await dbContext.Villas.ToListAsync();

		return Ok(mapper.Map< IEnumerable<VillaDTO> >(villaList));
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
		

		return Ok( mapper.Map<VillaDTO>(villa) );
	}

	[HttpPost]
	[ProducesResponseType(201)]
	[ProducesResponseType(400)]
	[ProducesResponseType(500)]
	public async Task< ActionResult<VillaDTO> > CrearVilla(
		[FromBody] CreateVillaDTO createVillaDTO
	)
	{
		if(!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		if (
			await dbContext.Villas.FirstOrDefaultAsync(
				x => x.Nombre.ToLower() == createVillaDTO.Nombre.ToLower()
			) != null
		)	
		{
			ModelState.AddModelError("NombreExistente", "Existe un registro con ese nombre");
			return BadRequest(ModelState);
		}

		if(createVillaDTO == null)
		{
			return BadRequest(createVillaDTO);
		}

		Villa modelo = mapper.Map<Villa>(createVillaDTO);

		await dbContext.Villas.AddAsync(modelo);
		await dbContext.SaveChangesAsync();

		// Retorno normal - Tradicional
		//return Ok(villaDTO);

		// Retorno recomendado

		return CreatedAtRoute("GetVilla", new { id = modelo.Id }, modelo);
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
	public async Task< IActionResult > UpdateVilla(
		int id, 
		[FromBody] UpdateVillaDTO updateVillaDTO
	)
	{
		if(updateVillaDTO == null || id != updateVillaDTO.Id)
		{
			return BadRequest();
		}

		Villa modelo = mapper.Map<Villa>(updateVillaDTO);

		dbContext.Villas.Update(modelo);
		await dbContext.SaveChangesAsync();

		return NoContent();
	}

	[HttpPatch("{id:int}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	public async Task< IActionResult > UpdatePartialVilla(
		int id, JsonPatchDocument<UpdateVillaDTO> patchDTO
	) 
	{
		if (patchDTO is null || id == 0)
		{
			return BadRequest();
		}

		// Atención al método AsNoTracking() - Investigarlo bien
		var villa = await dbContext.Villas.AsNoTracking().FirstOrDefaultAsync(v => v.Id == id);

		if(villa is null) return BadRequest();

		UpdateVillaDTO villaDto = mapper.Map<UpdateVillaDTO>(villa);

		patchDTO.ApplyTo(villaDto, ModelState);

		if(!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		Villa modelo = mapper.Map<Villa>(villaDto);

		dbContext.Villas.Update(modelo);
		await dbContext.SaveChangesAsync();

		return NoContent();
	}
}
