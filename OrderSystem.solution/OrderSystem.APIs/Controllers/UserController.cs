using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OrderSystem.APIs.DTO.UserViews;
using OrderSystem.APIs.Errors;
using OrderSystem.APIs.Exstentions;
using OrderSystem.Core;
using OrderSystem.Core.Entities.Identity;
using OrderSystem.Core.Services;

namespace OrderSystem.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : APIBaseController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _manager;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public UserController(IMapper mapper, UserManager<User> manager, IUnitOfWork unitOfWork, SignInManager<User> signInManager, ITokenService tokenService)
        {
            _mapper = mapper;
            _manager = manager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }


        [HttpPost("Register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto model)
        {

            if (checkDuplicateEmail(model.Email).Result.Value)
                return BadRequest(new ApiResponse(400, "Email ALready Exist"));

            var user = new User()
            {
                Email = model.Email,
                UserName = model.Email.Split('@')[0]
            };
            var result = await _manager.CreateAsync(user, model.Password);
            if (!result.Succeeded) return BadRequest(new ApiResponse(400));
            var returnedUser = new UserDto()
            {
                Id = user.Id,
                DisplayName = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user, _manager)
            };
            return Ok(returnedUser);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDto>> Login(LoginDto model)
        {
            var user1 = await _manager.GetUserMainAsync(User);
            var user = await _manager.FindByEmailAsync(model.Email);
            var users = _manager.Users;
            if (user is null) return Unauthorized(new ApiResponse(401));
            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false);
            if (!result.Succeeded) return Unauthorized(new ApiResponse(401));
            return Ok(new UserDto()
            {
                Id = user.Id,
                DisplayName = user.UserName,
                Token = await _tokenService.CreateTokenAsync(user, _manager)
            });

        }

        [HttpGet("EmailExist")]
        public async Task<ActionResult<bool>> checkDuplicateEmail(string email)
        {
            var user = await _manager.FindByEmailAsync(email);
            if (user is null) return false;
            else return true;
        }

    }
}
