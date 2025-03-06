using AutoMapper;
using Shared.DTOs.Basket;
using Shared.DTOs.Order;

namespace Saga.Orc;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<BasketCheckoutDto, CreateOrderDto>();
    }
}