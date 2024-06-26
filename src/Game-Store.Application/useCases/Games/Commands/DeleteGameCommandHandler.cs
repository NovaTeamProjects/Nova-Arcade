﻿using Game_Store.Application.Abstractions;
using Game_Store.Domain.Entities;
using Game_Store.Domain.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Game_Store.Application.useCases.Games.Commands
{
    public class DeleteGameCommandHandler : IRequestHandler<DeleteGameCommand, Game>
    {
        private readonly IAppDbContext _appDbContext;

        public DeleteGameCommandHandler(IAppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public async Task<Game> Handle(DeleteGameCommand request, CancellationToken cancellationToken)
        {
            var game = await _appDbContext.Games.FirstOrDefaultAsync(x => x.Id == request.Id);
            if (game is null)
                throw new NotFoundException("Game not found");

            _appDbContext.Games.Remove(game);

            await _appDbContext.SaveChangesAsync();

            return game;
        }
    }
}
