using Game_Store.Domain.Entities;
using MediatR;

namespace Game_Store.Application.useCases.Games.Queries
{
    public class GetAllGamesQuery : IRequest<List<Game>>
    {
    }
}
