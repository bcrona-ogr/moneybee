using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyBee.Customer.Application.Requests.Commands.CreateCustomer;
using MoneyBee.Customer.Application.Requests.Commands.DeleteCustomer;
using MoneyBee.Customer.Application.Requests.Commands.UpdateCustomer;
using MoneyBee.Customer.Application.Requests.Queries.GetCustomerById;
using MoneyBee.Customer.Application.Requests.Queries.SearchCustomers;
using MoneyBee.Customer.Contracts.Requests.Create;
using MoneyBee.Customer.Contracts.Requests.Update;
using MoneyBee.Customer.Contracts.Responses;
using MoneyBee.Customer.Contracts.Responses.Search;

namespace MoneyBee.Customer.API.Controllers
{
    [ApiController]
    [Route("api/customers")]
    [Authorize]
    public  class CustomersController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(CustomerHttpResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(
            [FromBody] CreateCustomerHttpRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new CreateCustomerRequestModel
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                IdentityNumber = request.IdentityNumber,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(MapCustomer(result));
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(CustomerHttpResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(
            [FromRoute] Guid id,
            [FromBody] UpdateCustomerHttpRequest request,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new UpdateCustomerRequestModel
            {
                Id = id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                DateOfBirth = request.DateOfBirth,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(new CustomerHttpResponse
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                PhoneNumber = result.PhoneNumber,
                Address = result.Address,
                DateOfBirth = result.DateOfBirth,
                IdentityNumber = result.IdentityNumber
            });
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerHttpResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetCustomerByIdRequestModel
            {
                Id = id,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(new CustomerHttpResponse
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                PhoneNumber = result.PhoneNumber,
                Address = result.Address,
                DateOfBirth = result.DateOfBirth,
                IdentityNumber = result.IdentityNumber
            });
        }

        [HttpGet]
        [ProducesResponseType(typeof(SearchCustomersHttpResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Search(
            [FromQuery] string query,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new SearchCustomersRequestModel
            {
                Query = query,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(new SearchCustomersHttpResponse
            {
                Items = result.Items.Select(x => new CustomerHttpResponse
                {
                    Id = x.Id,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    PhoneNumber = x.PhoneNumber,
                    Address = x.Address,
                    DateOfBirth = x.DateOfBirth,
                    IdentityNumber = x.IdentityNumber
                }).ToList()
            });
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(
            [FromRoute] Guid id,
            [FromServices] IMediator mediator,
            CancellationToken cancellationToken)
        {
            await mediator.Send(new DeleteCustomerRequestModel
            {
                Id = id,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return NoContent();
        }

        private static CustomerHttpResponse MapCustomer(CreateCustomerResponseModel result)
        {
            return new CustomerHttpResponse
            {
                Id = result.Id,
                FirstName = result.FirstName,
                LastName = result.LastName,
                PhoneNumber = result.PhoneNumber,
                Address = result.Address,
                DateOfBirth = result.DateOfBirth,
                IdentityNumber = result.IdentityNumber
            };
        }
    }
}