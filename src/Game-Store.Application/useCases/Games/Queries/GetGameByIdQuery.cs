using Game_Store.Domain.Entities;
using MediatR;

namespace Game_Store.Application.useCases.Games.Queries
{
    public class GetGameByIdQuery : IRequest<Game>
    {
        public Guid Id { get; set; }
    }
}
