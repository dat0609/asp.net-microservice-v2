using MediatR;

namespace Ordering.Application.Features.V1.Orders.Commands.CreateOrder;

public class CreateOrderCommand: IRequest<long>
{
    public long Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    //Address
    public string ShippingAddress { get; set; }
    public string InvoiceAddress { get; set; }
}