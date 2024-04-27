using Game_Store.Application.Abstractions;
using Game_Store.Domain.Entities;
using Game_Store.Domain.Exceptions;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Game_Store.Application.useCases.Games.Commands
{
    public class CreateGameCommandHandler : IRequestHandler<CreateGameCommand, Game>
    {
        public readonly IAppDbContext _context;
        private readonly IHostEnvironment _hostEnvironment;

        public CreateGameCommandHandler(IAppDbContext context, IHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<Game> Handle(CreateGameCommand request, CancellationToken cancellationToken)
        {

            var gameChecker = await _context.Games.FirstOrDefaultAsync(x => x.Name == request.Name);

            if (gameChecker is not null)
            {
                throw new AlreadyExistsException("Game already exists");
            }

            var posterFile = request.Poster;
            var trailerFile = request.Trailer;
            List<IFormFile> photosFile = request.Photos;
            var ratingsGuideFile = request.RatingsGuide;

            string posterPath = "";
            string posterName = "";

            string trailerPath = "";
            string trailerName = "";

            string photoPath = "";
            string photoName = "";
            List<string> photosPaths = new List<string>(); // To hold all photo paths

            string ratingsGuidePath = "";
            string ratingsGuideName = "";

            try
            {
                // Poster file
                posterName = Guid.NewGuid().ToString() + Path.GetExtension(posterFile.FileName);
                posterPath = Path.Combine(_hostEnvironment.ContentRootPath, $"{request.Name}/Poster", posterName);

                using (var posterStream = new FileStream(posterPath, FileMode.Create))
                {
                    await posterFile.CopyToAsync(posterStream);
                }

                // Trailer file
                trailerName = Guid.NewGuid().ToString() + Path.GetExtension(trailerFile.FileName);
                trailerPath = Path.Combine(_hostEnvironment.ContentRootPath, $"{request.Name}/Trailer", trailerName);

                using (var trailerStream = new FileStream(trailerPath, FileMode.Create))
                {
                    await trailerFile.CopyToAsync(trailerStream);
                }

                // Photos file
                foreach (var photoFile in photosFile)
                {
                    photoName = Guid.NewGuid().ToString() + Path.GetExtension(photoFile.FileName);
                    photoPath = Path.Combine(_hostEnvironment.ContentRootPath, $"{request.Name}/Photos", photoName);

                    using (var photoStream = new FileStream(photoPath, FileMode.Create))
                    {
                        await photoFile.CopyToAsync(photoStream);
                    }

                    photosPaths.Add($"/{request.Name}/Photos/{photoName}"); // Add photo path to the list
                }

                // RatingsGuide file
                ratingsGuideName = Guid.NewGuid().ToString() + Path.GetExtension(ratingsGuideFile.FileName);
                ratingsGuidePath = Path.Combine(_hostEnvironment.ContentRootPath, $"{request.Name}/RatingsGuide", ratingsGuideName);

                using (var ratingsGuideStream = new FileStream(ratingsGuidePath, FileMode.Create))
                {
                    await ratingsGuideFile.CopyToAsync(ratingsGuideStream);
                }
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync($"Error: {ex.Message}");
            }

            var game = request.Adapt<Game>();
            game.Poster = $"/{request.Name}/Poster/{posterName}";
            game.Trailer = $"/{request.Name}/Trailer/{trailerName}";
            game.Photos = photosPaths; // Assign list of photo paths
            game.RatingsGuide = $"/{request.Name}/RatingsGuide/{ratingsGuideName}";

            var result = await _context.Games.AddAsync(game);

            await _context.SaveChangesAsync();

            return result.Entity;
        }
    }
}
