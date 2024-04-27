using Game_Store.Domain.Exceptions;

namespace GameStore.API.Middlewares
{
    public class GlobalExceptionHandle
    {
        public RequestDelegate _requestDelegate;
        public ILogger<GlobalExceptionHandle> _logger;

        public GlobalExceptionHandle(RequestDelegate requestDelegate, ILogger<GlobalExceptionHandle> logger)
        {
            _requestDelegate = requestDelegate;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _requestDelegate(context);
            }
            catch (NotFoundException notFound)
            {
                throw new Exception();
            }
            catch (AlreadyExistsException alreadyExists)
            {
                int code = 404;
                await HandleException(code, alreadyExists, context);
            }
            catch (Exception ex)
            {
                _logger.LogError($"{ex}\n\n- - - - - - - - - - - - - - - - - - -\n\n");
                int code = 500;
                await HandleException(code, ex, context);
            }
        }

        private async Task HandleException(int code, Exception ex, HttpContext context)
        {
            context.Response.Redirect($"/Exceptions/Error?message={ex.Message}&code={code}");
            return;
        }
    }
}
