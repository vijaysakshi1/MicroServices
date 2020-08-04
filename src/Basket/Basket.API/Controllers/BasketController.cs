using AutoMapper;
using Basket.API.Entities;
using Basket.API.Repositories.Interfaces;
using EventBusRabbitMQ.Common;
using EventBusRabbitMQ.Events;
using EventBusRabbitMQ.Events.Producer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketController> _logger;
        private readonly IMapper _mapper;
        private readonly EventBusRabbitMQProducer _eventBus;

        public BasketController(IBasketRepository repository, ILogger<BasketController> logger, IMapper mapper, EventBusRabbitMQProducer eventBus)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper;
            _eventBus = eventBus;
        }

        [HttpGet]
        public async Task<ActionResult> GetBasket(string userName)
        {
            return Ok(await _repository.GetBasket(userName) ?? new BasketCart(userName));
        }

        [HttpPost]
        public async Task<ActionResult> UpdateBasket([FromBody] BasketCart basket)
        {
            return Ok(await _repository.UpdateBasket(basket));
        }

        [HttpDelete("{userName}")]
        public async Task<IActionResult> DeleteBasket(string userName)
        {
            return Ok(await _repository.DeleteBasket(userName));
        }

        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout checkout)
        {
            //get total price
            //remove basket
            //senc checkout event to rabbit
            BasketCart basket = await _repository.GetBasket(checkout.UserName);
            if (basket == null) return BadRequest();

            bool basketRemoved = await _repository.DeleteBasket(checkout.UserName);
            if (!basketRemoved) return BadRequest();

            var eventMessage = _mapper.Map<BasketCheckoutEvent>(checkout);
            eventMessage.RequestId = Guid.NewGuid();
            eventMessage.TotalPrice = checkout.TotalPrice;

            try
            {
                _eventBus.PublishBasketCheckout(EventBusConstants.BasketCheckoutQueue, eventMessage);
            }
            catch (Exception)
            {

                throw;
            }

            return Ok();
        }

    }
}
