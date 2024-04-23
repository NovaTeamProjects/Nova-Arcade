using Game_Store.Domain.Entities;
using MediatR;

namespace Game_Store.Application.useCases.Games.Queries
{
    public class GetGameByNameQuery : IRequest<Game>
    {
        public string GameName { get; set; }
    }
}
