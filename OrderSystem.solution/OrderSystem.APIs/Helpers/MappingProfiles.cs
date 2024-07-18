using AutoMapper;
using Microsoft.Extensions.Hosting;
using System.Net;
using OrderSystem.Core.Entities.Identity;
using OrderSystem.APIs.DTO.UserViews;
using OrderSystem.APIs.DTO.OrderViews;
using OrderSystem.Core.Entities.Core;
using OrderSystem.APIs.DTO.CustomerViews;
using OrderSystem.APIs.DTO.ProductViews;
using OrderSystem.APIs.DTO.InvoiceView;

namespace OrderSystem.APIs.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {

            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();

            CreateMap<OrderDto, Order>();
            CreateMap<Order, OrderDto>();

            CreateMap<CustomerDto, Customer>();
            CreateMap<Customer, CustomerDto>();

            CreateMap <ProductDto, Product>();
            CreateMap<Product, ProductDto>();

            CreateMap<InvoiceDto, Invoice>();
            CreateMap<Invoice, InvoiceDto>();

        }
    }
}
