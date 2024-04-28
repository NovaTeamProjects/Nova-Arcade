using Game_Store.Application.useCases.Games.Queries;
using Game_Store.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using MediatR;

namespace Game_Store.Controllers
{
    public class HomeController : Controller
    {
        private readonly MediatR.IMediator _mediator;
        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IActionResult> Index()
        {
            var query = new GetAllGamesQuery();
            var result = await _mediator.Send(query);
            return View(result);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
