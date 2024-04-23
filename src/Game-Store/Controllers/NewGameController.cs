using Game_Store.Application.useCases.Games.Commands;
using Game_Store.Application.useCases.Games.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Game_Store.Controllers
{
    public class NewGameController : Controller
    {
        private readonly IMediator _mediator;

        public NewGameController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var query = new GetAllGamesQuery();
            var result = await _mediator.Send(query);
            return View(result);
        }
        public IActionResult SysReqCreate()
        {
            return View();
        }

        public IActionResult CreateGameAsync()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateGameAsync(CreateGameCommand game)
        {
            var result = await _mediator.Send(game);

            return View("Details", result);
        }

        public async Task<IActionResult> DeleteGameAsync(Guid id)
        {
            var query = new DeleteGameCommand()
            {
                Id = id
            };

            var result = await _mediator.Send(query);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> UpdateGameAsync(Guid id)
        {
            var result = _mediator.Send(new GetGameByIdQuery() { Id = id });

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateGameAsync(UpdateGameCommand game)
        {
            return View(game);
        }
    }
}
