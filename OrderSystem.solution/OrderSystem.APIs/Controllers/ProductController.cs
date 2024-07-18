using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.Core.Entities.Identity;
using OrderSystem.Core.Services;
using OrderSystem.Core;
using Microsoft.AspNetCore.Authorization;
using OrderSystem.APIs.DTO.CustomerViews;
using OrderSystem.APIs.DTO.OrderViews;
using OrderSystem.APIs.Errors;
using OrderSystem.APIs.Exstentions;
using OrderSystem.Core.Entities.Core;
using OrderSystem.Core.Specifications.Core;
using OrderSystem.Core.Specifications;
using OrderSystem.APIs.DTO.ProductViews;
using Microsoft.Extensions.Hosting;

namespace OrderSystem.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : APIBaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _manager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IMapper mapper, UserManager<User> manager, IUnitOfWork unitOfWork, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        [HttpPost("")]
        public async Task<ActionResult<ProductDto>> CreateProduct(ProductDto newProduct)
        {
            var user = await _manager.GetUserMainAsync(User);

            if (user is null)
                return Unauthorized(new ApiResponse(401));


            var product = _mapper.Map<ProductDto, Product>(newProduct);

            if (product is null)
            {
                return BadRequest(new ApiResponse(400));
            }


            var result = _unitOfWork.Repository<Product>().Add(product);
            _unitOfWork.Repository<Product>().SaveChanges();

            if (!result.IsCompletedSuccessfully)
                return BadRequest(new ApiResponse(400));

            return Ok(product);
        }

        [Authorize]
        [HttpGet("")]
        public async Task<ActionResult<ProductDto>> GetProducts()
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var spec = new BaseSpecifications<Product>();
            var products = await _unitOfWork.Repository<Product>().GetAllWithSpecAsync(spec);

            if (products == null)
            {
                return NotFound(new ApiResponse(404, "no products found"));
            }

            var productsToReturn = _mapper.Map<IReadOnlyList<Product>, IReadOnlyList<ProductDto>>(products);
            return Ok(productsToReturn);
        }


        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetProductById(int id)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }


            var spec = new BaseSpecifications<Product>(p => p.Id == id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);

            if (product == null)
            {
                return NotFound(new ApiResponse(404, "Product not found"));
            }

            var productToReturn = _mapper.Map<Product,ProductDto>(product);
            return Ok(productToReturn);
        }


        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProductById(ProductDto newProduct, int id)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }
            var spec = new BaseSpecifications<Product>(p => p.Id == id);
            var product = await _unitOfWork.Repository<Product>().GetEntityWithSpecAsync(spec);
            newProduct.Id = id;

            if (product == null)
            {
                return NotFound(new ApiResponse(404, "Product not found"));
            }

            _mapper.Map(newProduct, product);
            _unitOfWork.Repository<Product>().Update(product);
            _unitOfWork.Repository<Product>().SaveChanges();

            var productToReturn = _mapper.Map<Product, ProductDto>(product);
            return Ok(productToReturn);
        }



    }
}
