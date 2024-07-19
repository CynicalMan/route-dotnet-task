using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.APIs.DTO.OrderViews;
using OrderSystem.APIs.Errors;
using OrderSystem.Core.Entities.Core;
using OrderSystem.Core.Entities.Identity;
using OrderSystem.Core.Services;
using OrderSystem.Core.Specifications;
using OrderSystem.Core;
using OrderSystem.APIs.DTO.OrderViews;
using OrderSystem.APIs.Exstentions;
using OrderSystem.Core.Specifications.Core;

namespace OrderSystem.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : APIBaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _manager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IOrderService _orderService;
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IMapper mapper, UserManager<User> manager, IUnitOfWork unitOfWork, SignInManager<User> signInManager, ITokenService tokenService, IOrderService orderService)
        {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _orderService = orderService;
        }

        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult<CreateOrderDTO>> CreateOrder(CreateOrderDTO newOrder)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
                return Unauthorized(new ApiResponse(401));
            var order = _mapper.Map<CreateOrderDTO, Order>(newOrder);
            if (order is null)
            {
                return BadRequest(new ApiResponse(400));
            }

            try
            {
                order.Customer = await _unitOfWork.Repository<Customer>().GetEntityWithSpecAsync(new BaseSpecifications<Customer>(c => c.Id == order.CustomerId));

                if (order.Customer is null)
                    return BadRequest(new ApiResponse(400, "An unexpected error occurred while placing the order."));
                var placedOrder = await _orderService.PlaceOrderAsync(order);

                return Ok(placedOrder);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ApiResponse(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse(500, "An unexpected error occurred while placing the order. " + ex.Message));
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDTO>> GetOrderDetailsById(int id)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }


            var spec = new OrderWithOrderItemsSpecification(id);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            var orderToReturn = _mapper.Map<Order, OrderDTO>(order);
            return Ok(orderToReturn);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<ActionResult<OrderDTO>> GetOrders()
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var spec = new OrderWithOrderItemsSpecification();
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            if (orders == null)
            {
                return NotFound(new ApiResponse(404, "no orders found"));
            }

            var ordersToReturn = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderDTO>>(orders);
            return Ok(ordersToReturn);
        }

        

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/Status")]
        public async Task<ActionResult<OrderDTO>> UpdateOrderStatusById(int id, [FromBody] UpdateOrderStatusRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Status))
            {
                return BadRequest(new ApiResponse(400, "The status field is required."));
            }

            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var order = await _orderService.UpdateOrderStatusAsync(id, request.Status);

            if (order == null)
            {
                return BadRequest(new ApiResponse(400, "The Order status was not updated"));
            }

            var spec = new OrderWithOrderItemsSpecification(order.Id);
            var orderWithOrderItems = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);
            var orderToReturn = _mapper.Map<Order, OrderDTO>(orderWithOrderItems);

            return Ok(orderToReturn);
        }




    }
}
