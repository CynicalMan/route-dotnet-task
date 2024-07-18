using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Core.Entities.Identity;
using OrderSystem.Core.Services;
using OrderSystem.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using OrderSystem.APIs.Errors;
using OrderSystem.APIs.Exstentions;
using OrderSystem.Core.Entities.Core;
using OrderSystem.Core.Specifications.Core;
using OrderSystem.APIs.DTO.CustomerViews;
using OrderSystem.APIs.DTO.OrderViews;

namespace OrderSystem.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : APIBaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _manager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public CustomerController(IMapper mapper, UserManager<User> manager, IUnitOfWork unitOfWork, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult<CustomerDto>> CreateCustomer(CustomerDto newCustomer)
        {
            var user = await _manager.GetUserMainAsync(User);

            if (user is null)
                return Unauthorized(new ApiResponse(401));


            var customer = _mapper.Map<CustomerDto, Customer>(newCustomer);

            if (customer is null)
            {
                return BadRequest(new ApiResponse(400));
            }


            var result = _unitOfWork.Repository<Customer>().Add(customer);
            _unitOfWork.Repository<Customer>().SaveChanges();

            if (!result.IsCompletedSuccessfully)
                return BadRequest(new ApiResponse(400));

            return Ok(customer);

        }

        [Authorize]
        [HttpGet("{id}/orders")]
        public async Task<ActionResult<CustomerDto>> GetOrdersByCustomerId(int id)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var spec = new OrdersByCustomerIdSpecification(id);
            var orders = await _unitOfWork.Repository<Order>().GetAllWithSpecAsync(spec);

            if (orders == null)
            {
                return NotFound(new ApiResponse(404, "Orders not found for the customer"));
            }

            var ordersToReturn = _mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderDto>>(orders);
            return Ok(ordersToReturn);
        }



    }
}
