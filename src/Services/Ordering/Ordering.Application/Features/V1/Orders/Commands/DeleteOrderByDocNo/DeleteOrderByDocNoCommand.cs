using MediatR;

namespace Ordering.Application.Features.V1.Orders.Commands.DeleteOrderByDocNo;

public class DeleteOrderByDocNoCommand : IRequest<bool>
{
    public string DocNo { get; set; }

    public DeleteOrderByDocNoCommand(string docNo)
    {
        DocNo = docNo;
    }
}