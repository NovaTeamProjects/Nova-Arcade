using Game_Store.Application.Abstractions;
using Game_Store.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Game_Store.Application.useCases.Games.Queries
{
    public class GetAllGamesQueryHandler : IRequestHandler<GetAllGamesQuery, List<Game>>
    {
        private readonly IAppDbContext _appDbContext;

        public GetAllGamesQueryHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<List<Game>> Handle(GetAllGamesQuery request, CancellationToken cancellationToken)
        {
            return await _appDbContext.Games.ToListAsync(cancellationToken);
        }
    }
}
