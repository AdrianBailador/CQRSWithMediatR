using CQRSWithMediatR.Commands;
using CQRSWithMediatR.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using CQRSWithMediatR.Queries;


namespace CQRSWithMediatR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(CreateProductCommand command)
        {
            var productId = await _mediator.Send(command);
            return Ok(productId);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id));
            return product is not null ? Ok(product) : NotFound();
        }
    }
}
