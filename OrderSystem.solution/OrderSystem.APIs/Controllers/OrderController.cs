﻿using AutoMapper;
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
        private readonly IUnitOfWork _unitOfWork;

        public OrderController(IMapper mapper, UserManager<User> manager, IUnitOfWork unitOfWork, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult<OrderDto>> CreateOrder(OrderDto newOrder)
        {
            var user = await _manager.GetUserMainAsync(User);

            if (user is null)
                return Unauthorized(new ApiResponse(401));


            var order = _mapper.Map<OrderDto, Order>(newOrder);

            if (order is null)
            {
                return BadRequest(new ApiResponse(400));
            }


            var result = _unitOfWork.Repository<Order>().Add(order);
            _unitOfWork.Repository<Order>().SaveChanges();

            if (!result.IsCompletedSuccessfully)
                return BadRequest(new ApiResponse(400));

            return Ok(order);
        }

        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<OrderDto>> GetOrders()
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var spec = new BaseSpecifications<Order>();
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            if (orders == null)
            {
                return NotFound(new ApiResponse(404, "no orders found"));
            }

            var ordersToReturn = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderDto>>(orders);
            return Ok(ordersToReturn);
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderDetailsById(int id)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }


            var spec = new BaseSpecifications<Order>(p => p.Id == id);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            var orderToReturn = _mapper.Map<Order, OrderDto>(order);
            return Ok(orderToReturn);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatusById(OrderDto newOrder, int id)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var spec = new BaseSpecifications<Order>(p => p.Id == id);
            var order = await _unitOfWork.Repository<Order>().GetEntityWithSpecAsync(spec);

            if (order == null)
            {
                return NotFound(new ApiResponse(404, "Order not found"));
            }

            _mapper.Map(newOrder, order);
            _unitOfWork.Repository<Order>().Update(order);
            _unitOfWork.Repository<Order>().SaveChanges();

            var orderToReturn = _mapper.Map<Order, OrderDto>(order);
            return Ok(orderToReturn);
        }



    }
}