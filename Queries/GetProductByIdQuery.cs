using CQRSWithMediatR.Models;
using MediatR;

namespace CQRSWithMediatR.Queries
{
    public record GetProductByIdQuery(int Id) : IRequest<Product>;
}
