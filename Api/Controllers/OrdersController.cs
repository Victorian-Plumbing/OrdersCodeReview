using Api.SwaggerExamples;
using Application.Exceptions;
using Application.Orders;
using Client.Dtos;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderReader _orderReader;
    private readonly IOrderCreator _orderCreator;
    private readonly IOrderUpdater _orderUpdater;

    public OrdersController(IOrderReader orderReader,
                            IOrderCreator orderCreator,
                            IOrderUpdater orderUpdater)
    {
        _orderReader = orderReader;
        _orderCreator = orderCreator;
        _orderUpdater = orderUpdater;
    }

    [HttpGet("[action]")]
    //I've only seen wrapping tasks like this where somebody is trying to use mediatr without asynchronous calls.
    //Wrapping in Task.FromResult here is unnecessary overhead
    //For exceptions, you can annotate your methods with XML comments, with information about expected exceptions.

    //For the data binding here, I'd err on the side of using FromRoute to follow REST standards, which would use the GET as part of the Path / URI
    //For example, victoriaplumbing.test.com/orders/1234 , where 1234 is your order number

    //For the input parameter, I would still try and wrap this in an object, even if it only has a single property
    //(which you can use attribute annotation for FromQuery / FromRoute)
    //This allows for future extension, and allows for the developer to provide an example of the property in an XML comment
    //Currently, I don't know if the orderNumber is a string, where fyuegfuyegsf is valid, if it follows a numeric convention, or if it's based on a uuid
    public Task<IActionResult> Get([FromQuery]string orderNumber)
    {
        try
        {
            var response = _orderReader.ReadOrder(orderNumber);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            //Consider wrapping errors in ProblemDetails objects, https://datatracker.ietf.org/doc/html/rfc7807
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    [HttpPost("[action]")]
    [SwaggerRequestExample(typeof(CreateOrderRequestDto), typeof(CreateOrderExample))]
    public Task<IActionResult> Create([FromBody]CreateOrderRequestDto request)
    {
        try
        {
            var response = _orderCreator.CreateOrder(request);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            //With throwing errors, I would suggest either having some type of wrapper (like the problem details) which is handled at the controller layer,
            //or some type of validation layer. Otherwise, there is middleware that can be used before a response is sent back to the client.
            //This can be used to format exception data.

            //Depending if the API is internal or external, we might want a more human-friendly exception message, and some type of background
            //process / service which is serviced the entire stack trace for future troubleshooting
            throw new Exception(ex.Message);
        }
    }

    //Following rest convention, I feel this should be a HttpPatch rather than a PUT, unless this method is supposed to allow for
    //the creation of an order, if it doesn't exist

    //Following that, I would move the "OrderNumber" to the Route, similar to the GET comment above
    [HttpPut("[action]")]
    [SwaggerRequestExample(typeof(UpdateOrderRequestDto), typeof(UpdateOrderExample))]
    public Task<IActionResult> Update([FromBody] UpdateOrderRequestDto request)
    {
        try
        {
            var response = _orderUpdater.UpdateOrder(request);

            return Task.FromResult<IActionResult>(Ok(response));
        }
        catch (ValidationException ex)
        {
            return Task.FromResult<IActionResult>(BadRequest(ex.Errors));
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
