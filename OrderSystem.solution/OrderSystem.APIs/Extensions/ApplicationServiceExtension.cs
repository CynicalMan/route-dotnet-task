using Microsoft.AspNetCore.Mvc;
using OrderSystem.Core.Repositories;
using OrderSystem.Core;
using OrderSystem.Repository;
using OrderSystem.APIs.Errors;
using OrderSystem.APIs.Helpers;
using OrderSystem.Core.Services;
using OrderSystem.Service.Services;
using OrderSystem.Service.DiscountStrategies;
using OrderSystem.Service;
using Stripe;

namespace OrderSystem.APIs.Exstentions
{
    public static class ApplicationServiceExtention
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection Services)
        {
            Services.AddScoped<IUnitOfWork, UnitOfWork>();
            Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            Services.AddAutoMapper(typeof(MappingProfiles));
            Services.AddScoped<IOrderService, OrderService>();
            Services.AddScoped<IEmailService, EmailService>();
            Services.AddScoped<PayPalPaymentService>();
            Services.AddSingleton<IDiscountStrategy, HighTierDiscount>();
            Services.AddSingleton<IDiscountStrategy, LowTierDiscount>();
            Services.AddSingleton<IDiscountStrategy, NoDiscount>();
            Services.AddSingleton<DiscountStrategySelector>();
            Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState.Where(p => p.Value.Errors.Count() > 0).
                                                        SelectMany(p => p.Value.Errors).
                                                        Select(e => e.ErrorMessage).ToArray();
                    var ValidationErorResponse = new ApiValidationErrorResponse()
                    {
                        Errors = errors
                    };
                    return new BadRequestObjectResult(ValidationErorResponse);
                };
            });
            return Services;
        }
    }
}
