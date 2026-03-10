using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoneyBee.Shared.API.Response;
using MoneyBee.Transfer.Application.Requests.Commands.CancelTransfer;
using MoneyBee.Transfer.Application.Requests.Commands.CompleteTransfer;
using MoneyBee.Transfer.Application.Requests.Commands.CreateTransfer;
using MoneyBee.Transfer.Application.Requests.Queries.GetTransferByCode;
using MoneyBee.Transfer.Application.Requests.Queries.GetTransferHistory;
using MoneyBee.Transfer.Contracts.Requests.Cancel;
using MoneyBee.Transfer.Contracts.Requests.Complete;
using MoneyBee.Transfer.Contracts.Requests.Create;
using MoneyBee.Transfer.Contracts.Responses;
using MoneyBee.Transfer.Contracts.Responses.History;

namespace MoneyBee.Transfer.API.Controllers
{
    [ApiController]
    [Route("api/transfers")]
    [Authorize]
    public class TransfersController : ControllerBase
    {
        [HttpPost]
        [ProducesResponseType(typeof(TransferHttpResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status422UnprocessableEntity)]
        [ProducesResponseType(typeof(ErrorHttpResponse), StatusCodes.Status500InternalServerError)]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTransferHttpRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            var employeeIdClaim = User.FindFirstValue("employee_id");
            if (!Guid.TryParse(employeeIdClaim, out var employeeId))
                return Unauthorized();

            var idempotencyKey = Request.Headers["Idempotency-Key"].FirstOrDefault();

            if (string.IsNullOrEmpty(idempotencyKey))
            {
                idempotencyKey = Guid.NewGuid().ToString();
            }

            var result = await mediator.Send(new CreateTransferRequestModel
            {
                SenderCustomerId = request.SenderCustomerId,
                ReceiverCustomerId = request.ReceiverCustomerId,
                Amount = request.Amount,
                EmployeeId = employeeId,
                IdempotencyKey = idempotencyKey,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(MapTransfer(result));
        }

        [HttpGet("by-code/{transactionCode}")]
        public async Task<IActionResult> GetByCode([FromRoute] string transactionCode, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetTransferByCodeRequestModel
            {
                TransactionCode = transactionCode,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(new TransferHttpResponse
            {
                Id = result.Id,
                TransactionCode = result.TransactionCode,
                Amount = result.Amount,
                Fee = result.Fee,
                Currency = result.Currency,
                Status = result.Status,
                SenderCustomerId = result.SenderCustomerId,
                ReceiverCustomerId = result.ReceiverCustomerId,
                CreatedAtUtc = result.CreatedAtUtc
            });
        }

        [HttpPost("complete")]
        public async Task<IActionResult> Complete([FromBody] CompleteTransferHttpRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new CompleteTransferRequestModel
            {
                TransactionCode = request.TransactionCode,
                CorrelationId = HttpContext.TraceIdentifier,
                ReceiverCustomerId = request.ReceiverCustomerId
            }, cancellationToken);

            return Ok(result);
        }

        [HttpPost("cancel")]
        public async Task<IActionResult> Cancel([FromBody] CancelTransferHttpRequest request, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new CancelTransferRequestModel
            {
                TransactionCode = request.TransactionCode,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(result);
        }

        [HttpGet("history")]
        public async Task<IActionResult> History([FromQuery] Guid customerId, [FromQuery] string role, [FromServices] IMediator mediator, CancellationToken cancellationToken)
        {
            var result = await mediator.Send(new GetTransferHistoryRequestModel
            {
                CustomerId = customerId,
                Role = role,
                CorrelationId = HttpContext.TraceIdentifier
            }, cancellationToken);

            return Ok(new TransferHistoryHttpResponse
            {
                Items = result.Items.Select(x => new TransferHistoryItemHttpResponse
                {
                    Id = x.Id,
                    TransactionCode = x.TransactionCode,
                    Amount = x.Amount,
                    Fee = x.Fee,
                    Currency = x.Currency,
                    Status = x.Status,
                    SenderCustomerId = x.SenderCustomerId,
                    ReceiverCustomerId = x.ReceiverCustomerId,
                    CreatedAtUtc = x.CreatedAtUtc,
                    CompletedAtUtc = x.CompletedAtUtc,
                    CancelledAtUtc = x.CancelledAtUtc
                }).ToList()
            });
        }

        private static TransferHttpResponse MapTransfer(CreateTransferResponseModel result)
        {
            return new TransferHttpResponse
            {
                Id = result.Id,
                TransactionCode = result.TransactionCode,
                Amount = result.Amount,
                Fee = result.Fee,
                Currency = result.Currency,
                Status = result.Status,
                SenderCustomerId = result.SenderCustomerId,
                ReceiverCustomerId = result.ReceiverCustomerId,
                CreatedAtUtc = result.CreatedAtUtc
            };
        }
    }
}