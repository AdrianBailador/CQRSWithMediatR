using MediatR;

namespace CQRSWithMediatR.Commands
{
    public record CreateProductCommand(string Name, decimal Price) : IRequest<int>;
}
