using AutoMapper;
using MagicVilla_API.Data;
using MagicVilla_API.Models;
using MagicVilla_API.Models.DTO;
using MagicVilla_API.Repository.IRepository;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace MagicVilla_API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NumeroVillaController : ControllerBase
{
	private readonly ILogger<NumeroVillaController> logger;
	private readonly IVillaRepository villaRepository;
	private readonly INumeroVillaRepository numeroVillaRepository;
	private readonly IMapper mapper;
	protected APIResponse apiResponse;

	public NumeroVillaController(
		ILogger<NumeroVillaController> logger,
		IVillaRepository villaRepository,
		INumeroVillaRepository numeroVillaRepository,
		IMapper mapper
	)
    {
		this.logger = logger;
		this.villaRepository = villaRepository;
		this.numeroVillaRepository = numeroVillaRepository;
		this.mapper = mapper;
		this.apiResponse = new();
	}

    [HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task< ActionResult<APIResponse> > GetNumeroVillas()
	{
		try
		{
			logger.LogInformation("Obtener Número de Villas");

			IEnumerable<NumeroVilla> numeroVillaList = await numeroVillaRepository.ObtenerTodos();

			apiResponse.Resultado = mapper.Map<IEnumerable<NumeroVillaDTO>>(numeroVillaList);
			apiResponse.StatusCode = HttpStatusCode.OK;

			return Ok(apiResponse);
		}
		catch (Exception ex)
		{
			apiResponse.IsExitoso = false;
			apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return apiResponse;

	}

	[HttpGet("{id:int}", Name = "GetNumeroVilla")]
	[ProducesResponseType(StatusCodes.Status200OK)] // Una forma de indicar status
	[ProducesResponseType(400)] // Otra forma de indicar los status
	[ProducesResponseType(404)]
	public async Task< ActionResult<APIResponse> > GetNumeroVilla(int id)
	{
		try
		{
			if(id == 0)
			{
				logger.LogError($"Error al intentar traer Numero de Villa con el Id: {id}");
				apiResponse.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(apiResponse);
			}

			var numeroVilla = await numeroVillaRepository.Obtener(x => x.VillaNo == id);

			if(numeroVilla == null)
			{
				apiResponse.StatusCode = HttpStatusCode.NotFound; 
				return NotFound(apiResponse);
			}

			apiResponse.StatusCode = HttpStatusCode.OK;
			apiResponse.Resultado = mapper.Map<NumeroVillaDTO>(numeroVilla);

			return Ok(apiResponse);
		}
		catch (Exception ex)
		{
			apiResponse.IsExitoso = false;
			apiResponse.ErrorMessages = new List<string> { ex.ToString() };
		}

		return apiResponse;
	}

	[HttpPost]
	[ProducesResponseType(201)]
	[ProducesResponseType(400)]
	[ProducesResponseType(500)]
	public async Task< ActionResult<APIResponse> > CrearNumeroVilla(
		[FromBody] CreateNumeroVillaDTO createNumeroVillaDTO
	)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (
				await numeroVillaRepository.Obtener(
					x => x.VillaNo == createNumeroVillaDTO.VillaNo
				) != null
			)
			{
				ModelState.AddModelError(
					"NombreExistente", 
					"Existe un registro con ese Número de Villa"
				);
				return BadRequest(ModelState);
			}

			if(await villaRepository.Obtener(v => v.Id == createNumeroVillaDTO.VillaId ) == null )
			{
				ModelState.AddModelError(
					"ClaveForanea",
					"El Id de la Villa no existe"
				);
				return BadRequest(ModelState);
			}

			if (createNumeroVillaDTO == null)
			{
				return BadRequest(createNumeroVillaDTO);
			}

			NumeroVilla modelo = mapper.Map<NumeroVilla>(createNumeroVillaDTO);

			modelo.FechaCreacion = DateTime.Now;
			modelo.FechaActualizacion =	DateTime.Now;

			await numeroVillaRepository.Crear(modelo);

			apiResponse.Resultado = modelo;
			apiResponse.StatusCode = HttpStatusCode.Created;

			// Retorno normal - Tradicional
			//return Ok(villaDTO);

			// Retorno recomendado

			return CreatedAtRoute("GetNumeroVilla", new { id = modelo.VillaNo }, apiResponse);
		}
		catch (Exception ex)
		{
			apiResponse.IsExitoso = false;
			apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return apiResponse;
	}

	[HttpDelete("{id:int}")]
	[ProducesResponseType(StatusCodes.Status204NoContent)]
	[ProducesResponseType(StatusCodes.Status400BadRequest)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	public async Task< IActionResult > DeleteNumeroVilla(int id)
	{
		try
		{
			if (id == 0)
			{
				apiResponse.IsExitoso = false;
				apiResponse.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(apiResponse);
			}

			var numeroVilla = await numeroVillaRepository.Obtener(v => v.VillaNo == id);

			if (numeroVilla is null)
			{
				apiResponse.IsExitoso = false;
				apiResponse.StatusCode = HttpStatusCode.NotFound;
				return NotFound(apiResponse);
			}

			await numeroVillaRepository.Remover(numeroVilla);

			apiResponse.StatusCode = HttpStatusCode.NoContent;

			return Ok(apiResponse);
		}
		catch (Exception ex)
		{
			apiResponse.IsExitoso = false;
			apiResponse.ErrorMessages = new List<string>() { ex.ToString() };
		}

		return BadRequest(apiResponse);
	}

	[HttpPut("{id:int}")]
	[ProducesResponseType(400)]
	[ProducesResponseType(204)]
	public async Task< IActionResult > UpdateVilla(
		int id, 
		[FromBody] UpdateNumeroVillaDTO updateNumeroVillaDTO
	)
	{
		if(updateNumeroVillaDTO == null || id != updateNumeroVillaDTO.VillaNo)
		{
			apiResponse.StatusCode = HttpStatusCode.BadRequest;
			apiResponse.IsExitoso = false;
			return BadRequest(apiResponse);
		}

		if(await villaRepository.Obtener(v => v.Id == updateNumeroVillaDTO.VillaId) == null)
		{
			ModelState.AddModelError(
				"ClaveForanea",
				"El Id de la villa no existe"
			);
			return BadRequest(ModelState);
		}

		NumeroVilla modelo = mapper.Map<NumeroVilla>(updateNumeroVillaDTO);

		await numeroVillaRepository.Update(modelo);

		apiResponse.StatusCode = HttpStatusCode.NoContent;

		return Ok(apiResponse);
	}

	// No se implementará el patch
}
