using Game_Store.Application.useCases.Games.Commands;
using Game_Store.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly IMediator _mediatR;

        public GameController(IMediator mediator)
        {
            _mediatR = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(Game game)
        {
            var query = new CreateGameCommand()
            {
                SysReqId = game.SysReqId,

            };
            var res = await _mediatR.Send(query);

            return Ok(res);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid uuid)
        {
            var query = new DeleteGameCommand()
            {
                Id = uuid

            };
            var res = await _mediatR.Send(query);

            return Ok(res);
        }
    }
}
