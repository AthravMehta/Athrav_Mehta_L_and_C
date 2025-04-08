using Microsoft.AspNetCore.Mvc;
using ProductManagement.Constants;
using ProductManagement.Entities;
using ProductManagement.Services.Contracts;

namespace ProductManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Product>>> GetAllProducts()
        {
            try
            {
                return await _productService.GetAllProducts();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            try
            {
                var product = await _productService.GetProduct(id);
                if (product == null)
                {
                    return NotFound();
                }
                return product;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult> AddProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest(ErrorConstants.ProductIsNull);
            }
            try
            {
                if (_productService.ProductExists(product.Name))
                {
                    return Conflict(ErrorConstants.ProductWithSimilarNameExists);
                }
                await _productService.AddProduct(product);
                return CreatedAtAction("GetProduct", new { id = product.Id }, product);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Product product)
        {
            if (product == null)
            {
                return BadRequest(ErrorConstants.ProductIsNull);
            }
            try
            {
                await _productService.UpdateProduct(product);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            if (id <= 0)
            {
                return BadRequest(ErrorConstants.InvalidId);
            }
            try
            {
                await _productService.DeleteProduct(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, ex.Message);
            }
        }
    }
}
