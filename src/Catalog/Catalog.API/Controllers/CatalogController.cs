using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.API.Entities;
using Catalog.API.Entities.Repositories.interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Catalog.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CatalogController : Controller
    {
        private readonly IProductRepository productRepository;
        private readonly ILogger<CatalogController> logger;

        public CatalogController(IProductRepository productRepository, ILogger<CatalogController> logger)
        {
            this.productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            return Ok(await productRepository.GetProducts());
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetProduct(string id)
        {
            return Ok(await productRepository.GetProduct(id));
        }

        [HttpGet]
        [Route("[action]/{categoryName}")]
        public async Task<IActionResult> GetProductCategory(string categoryName)
        {
            return Ok(await productRepository.GetProductCategory(categoryName));
        }

        [HttpGet]
        [Route("[action]/{name}")]
        public async Task<IActionResult> GetProductByName(string name)
        {
            return Ok(await productRepository.GetProductByName(name));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]Product product)
        {
            await productRepository.Create(product);
            return Ok(product);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            await productRepository.Delete(id);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody]Product product)
        {
            await productRepository.Update(product);
            return Ok(product);
        }
    }
}
