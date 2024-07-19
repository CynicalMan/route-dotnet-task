using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.APIs.DTO.InvoiceView;
using OrderSystem.APIs.DTO.OrderViews;
using OrderSystem.APIs.DTO.ProductViews;
using OrderSystem.APIs.Errors;
using OrderSystem.APIs.Exstentions;
using OrderSystem.Core;
using OrderSystem.Core.Entities.Core;
using OrderSystem.Core.Entities.Identity;
using OrderSystem.Core.Services;
using OrderSystem.Core.Specifications;

namespace OrderSystem.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InvoiceController : APIBaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _manager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceController(IMapper mapper, UserManager<User> manager, IUnitOfWork unitOfWork, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("")]
        public async Task<ActionResult<InvoiceDto>> GetInvoices()
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }

            var spec = new BaseSpecifications<Invoice>();
            var invoices = await _unitOfWork.Repository<Invoice>().GetAllWithSpecAsync(spec);

            if (invoices == null)
            {
                return NotFound(new ApiResponse(404, "no invoices found"));
            }

            var invoicesToReturn = _mapper.Map<IReadOnlyList<Invoice>, IReadOnlyList<InvoiceDto>>(invoices);
            return Ok(invoicesToReturn);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetInvoiceDetailsById(int id)
        {
            var user = await _manager.GetUserMainAsync(User);
            if (user is null)
            {
                return Unauthorized(new ApiResponse(401));
            }


            var spec = new BaseSpecifications<Invoice>(p => p.Id == id);
            var invoice = await _unitOfWork.Repository<Invoice>().GetEntityWithSpecAsync(spec);

            if (invoice == null)
            {
                return NotFound(new ApiResponse(404, "Invoice not found"));
            }

            var invoiceToReturn = _mapper.Map<Invoice, InvoiceDto>(invoice);
            return Ok(invoiceToReturn);
        }
    
    }
}
