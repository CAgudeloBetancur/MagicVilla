﻿using AutoMapper;
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
public class VillaController : ControllerBase
{
	private readonly ILogger<VillaController> logger;
	private readonly IVillaRepository villaRepository;
	private readonly IMapper mapper;
	protected APIResponse apiResponse;

	public VillaController(
		ILogger<VillaController> logger,
		IVillaRepository villaRepository,
		IMapper mapper
	)
    {
		this.logger = logger;
		this.villaRepository = villaRepository;
		this.mapper = mapper;
		this.apiResponse = new();
	}

    [HttpGet]
	[ProducesResponseType(StatusCodes.Status200OK)]
	public async Task< ActionResult<APIResponse> > GetVillas()
	{
		try
		{
			logger.LogInformation("Obtener todas las Villas");

			IEnumerable<Villa> villaList = await villaRepository.ObtenerTodos();

			apiResponse.Resultado = mapper.Map<IEnumerable<VillaDTO>>(villaList);
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

	[HttpGet("{id:int}", Name = "GetVilla")]
	[ProducesResponseType(StatusCodes.Status200OK)] // Una forma de indicar status
	[ProducesResponseType(400)] // Otra forma de indicar los status
	[ProducesResponseType(404)]
	public async Task< ActionResult<APIResponse> > GetVilla(int id)
	{
		try
		{
			if(id == 0)
			{
				logger.LogError($"Error al intentar traer la Villa con el Id: {id}");
				apiResponse.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(apiResponse);
			}

			var villa = await villaRepository.Obtener(x => x.Id == id);

			if(villa == null)
			{
				apiResponse.StatusCode = HttpStatusCode.NotFound; 
				return NotFound(apiResponse);
			}

			apiResponse.StatusCode = HttpStatusCode.OK;
			apiResponse.Resultado = mapper.Map<VillaDTO>(villa);

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
	public async Task< ActionResult<APIResponse> > CrearVilla(
		[FromBody] CreateVillaDTO createVillaDTO
	)
	{
		try
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			if (
				await villaRepository.Obtener(
					x => x.Nombre.ToLower() == createVillaDTO.Nombre.ToLower()
				) != null
			)
			{
				ModelState.AddModelError("NombreExistente", "Existe un registro con ese nombre");
				return BadRequest(ModelState);
			}

			if (createVillaDTO == null)
			{
				return BadRequest(createVillaDTO);
			}

			Villa modelo = mapper.Map<Villa>(createVillaDTO);

			modelo.FechaCreacion = DateTime.Now;
			modelo.FechaActualizacion =	DateTime.Now;

			await villaRepository.Crear(modelo);

			apiResponse.Resultado = modelo;
			apiResponse.StatusCode = HttpStatusCode.Created;

			// Retorno normal - Tradicional
			//return Ok(villaDTO);

			// Retorno recomendado

			return CreatedAtRoute("GetVilla", new { id = modelo.Id }, apiResponse);
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
	public async Task< IActionResult > DeleteVilla(int id)
	{
		try
		{
			if (id == 0)
			{
				apiResponse.IsExitoso = false;
				apiResponse.StatusCode = HttpStatusCode.BadRequest;
				return BadRequest(apiResponse);
			}

			var villa = await villaRepository.Obtener(v => v.Id == id);

			if (villa is null)
			{
				apiResponse.IsExitoso = false;
				apiResponse.StatusCode = HttpStatusCode.NotFound;
				return NotFound(apiResponse);
			}

			await villaRepository.Remover(villa);

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
		[FromBody] UpdateVillaDTO updateVillaDTO
	)
	{
		if(updateVillaDTO == null || id != updateVillaDTO.Id)
		{
			apiResponse.StatusCode = HttpStatusCode.BadRequest;
			apiResponse.IsExitoso = false;
			return BadRequest(apiResponse);
		}

		Villa modelo = mapper.Map<Villa>(updateVillaDTO);

		await villaRepository.Update(modelo);

		apiResponse.StatusCode = HttpStatusCode.NoContent;

		return Ok(apiResponse);
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
			apiResponse.StatusCode = HttpStatusCode.BadRequest;
			apiResponse.IsExitoso = false;
			return BadRequest(apiResponse);
		}

		// Atención al método AsNoTracking() - Investigarlo bien
		var villa = await villaRepository.Obtener(v => v.Id == id, tracked:false);

		if (villa is null)
		{
			apiResponse.StatusCode = HttpStatusCode.BadRequest;
			apiResponse.IsExitoso = false;
			return BadRequest(apiResponse);
		}

		UpdateVillaDTO villaDto = mapper.Map<UpdateVillaDTO>(villa);

		patchDTO.ApplyTo(villaDto, ModelState);

		if(!ModelState.IsValid)
		{
			return BadRequest(ModelState);
		}

		Villa modelo = mapper.Map<Villa>(villaDto);

		await villaRepository.Update(modelo);

		apiResponse.StatusCode = HttpStatusCode.NoContent;

		return Ok(apiResponse);
	}
}
