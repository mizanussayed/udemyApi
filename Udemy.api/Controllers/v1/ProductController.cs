using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Udemy.api.Models.Domain;
using Udemy.api.Models.Dto;
using Udemy.api.Repositories;
using Udemy.api.Services;

namespace Udemy.api.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly IMapper _mapper;
    private readonly IImageRepository _imgRep;

    public ProductController(IProductRepository repo, IMapper mapper, IImageRepository imgRep)
    {
        _repo = repo;
        _mapper = mapper;
        _imgRep = imgRep;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int pageNo = 1, [FromQuery] int pageSize = 10)
    {
        var product = await _repo.GetAll(pageNo, pageSize);
        return Ok(_mapper.Map<List<ProductResponse>>(product));
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        var product = await _repo.GetProduct(id);
        var productDto = _mapper.Map<ProductResponse>(product);
        if (productDto is null)
            return NotFound();
        return Ok(productDto);
    }

    [HttpPost]
    [ValidateModel]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromForm] ProductDto model)
    {
        var mapModel = _mapper.Map<Product>(model);
        if (model.ImageFile is not null)
        {
            var fileUrl = await _imgRep.UpladImageToDir(model.ImageFile);
            if (string.IsNullOrEmpty(fileUrl))
            {
                ModelState.AddModelError("file", "File type is not supported or size is more than 5 mb");
            }
            else
            {
                mapModel.ImgUrl = fileUrl;
            }
        }
        var prod = await _repo.Create(mapModel);
        return CreatedAtAction("GetById", new { id = prod.Id }, prod);
    }

    [HttpPut]
    [Route("{id}")]
    [ValidateModel]
    public async Task<IActionResult> Update(int id, [FromBody] ProductDto model)
    {
        var prod = await _repo.Update(id, _mapper.Map<Product>(model));
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
        var product = await _repo.Delete(id);
        if (product is null)
            return NotFound();
        return Ok(product);
    }
}