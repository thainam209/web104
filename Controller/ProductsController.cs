using Microsoft.AspNetCore.Mvc;
using ProductApi.Model;
using ProductApi.Service;

namespace ProductApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 2)
        {
            var products = await _productService.GetAllProductsAsync();
            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(pagedProducts);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }
        [HttpGet("search")]
        public async Task<IActionResult> SearchByName([FromQuery] string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return BadRequest("Name parameter is required.");
            }

            var products = await _productService.SearchProductsByNameAsync(name);
            return Ok(products);
        }
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Product product)
        {
            if (string.IsNullOrEmpty(product.Name)) return BadRequest("Name is required");
            var createdProduct = await _productService.CreateProductAsync(product);
            return CreatedAtAction(nameof(GetById), new { id = createdProduct.ProductId }, createdProduct);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Product product)
        {
            var updatedProduct = await _productService.UpdateProductAsync(id, product);
            if (updatedProduct == null) return NotFound();
            return Ok(updatedProduct);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteProductAsync(id);
            if (!result) return NotFound();
            return NoContent();
        }
    }
}