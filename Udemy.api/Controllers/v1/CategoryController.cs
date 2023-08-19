using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Udemy.api.Models.Domain;
using Udemy.api.Models.Dto;
using Udemy.api.Repositories;

namespace Udemy.api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoryController(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var category = await _repo.GetAll();
        return Ok(_mapper.Map<List<CategoryDto>>(category));
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var category = await _repo.GetCategory(id);
        var res = _mapper.Map<CategoryResponse>(category);
        if (res is null)
            return NotFound();
        return Ok(res);
    }

    [HttpPost]
    [ValidateModel]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CategoryDto model)
    {
        var prod = await _repo.Create(_mapper.Map<Category>(model));
        return CreatedAtAction("GetById", new { id = prod.Id }, prod);
    }

    [HttpPut]
    [Route("{id}")]
    [ValidateModel]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryDto model)
    {
        var prod = await _repo.Update(id, _mapper.Map<Category>(model));
        if (prod is not null)
        {
            return Ok(prod);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        var Category = await _repo.Delete(id);
        if (Category is null)
            return NotFound();
        return Ok(Category);
    }
}