using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

//Consider using versioning for the API. This can allow for future extension
[Route("api/[controller]")]
[ApiController]
public class InboxController : ControllerBase
{
    [HttpPost("[action]")]
    //Rather than taking a raw object as a payload, it's preferable to have a type that can be mapped to, even if this is a generic wrapper.
    //This will allow for stronger usage of middleware in the .NET eco-system (attribute checking flags etc)
    
    public IActionResult Receive(object payload)
    {
        return Ok("This is a placeholder to represent how we would receive incoming messages such as product price and stock updates");
    }

    //Since the application is going to deal with I/O operations, we should try and handle an asynchronous process, passing in cancellation tokens,
    //and dealing with dB operations asynchronously where appropiate. As an entry point example:
    //public async Task<IActionResult> Receive(object payload, CancellationToken cancellationToken)
    //{
    //    //For any awaitable methods not called by an assembly or the runtime, I'd post-fix the method name woth async
    //    await HandleOperationAsync(cancellationToken);
    //    return Ok();
    //
    //}
    //
    //private async Task HandleOperationAsync(CancellationToken cancellationToken)
    //{
    //    await Task.Delay(100, cancellationToken);
    //}

    //Commented out entrypoint as to not confuse swagger doc gen
}
