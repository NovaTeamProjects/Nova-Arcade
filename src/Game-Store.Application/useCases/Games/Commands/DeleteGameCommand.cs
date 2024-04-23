using Game_Store.Domain.Entities;
using MediatR;

namespace Game_Store.Application.useCases.Games.Commands
{
    public class DeleteGameCommand : IRequest<Game>
    {
        public Guid Id { get; set; }
    }
}
