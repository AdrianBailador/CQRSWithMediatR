using CQRSWithMediatR.Data;
using CQRSWithMediatR.Models;
using CQRSWithMediatR.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CQRSWithMediatR.Handlers
{
    public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Product>
    {
        private readonly ApplicationDbContext _context;

        public GetProductByIdHandler(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Product> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            return await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        }
    }
}
