using FluentValidation;
using MediatR;
using MoneyBee.Shared.Application.Validation;
using MoneyBee.Shared.Core.Exceptions;

namespace MoneyBee.Shared.Application.Request
{
    /// <summary>
    /// Base class for all request handlers
    /// </summary>
    /// <typeparam name="TIn">Request Type</typeparam>
    /// <typeparam name="TOut">Response Type</typeparam>
    public abstract class BaseRequestHandler<TIn, TOut> : BaseRequestHandler<TIn, TOut, NoValidationRequired<TIn>> where TIn : IRequest<TOut>
    {
        protected BaseRequestHandler() : base()
        {
        }
    }


    /// <summary>
    /// Base class for all request handlers includes validation.
    /// Validates the request using the given validator.
    /// </summary>
    /// <typeparam name="TIn">Request Type</typeparam>
    /// <typeparam name="TOut">Response Type</typeparam>
    /// <typeparam name="TValidator">Validator Type</typeparam>
    public abstract class BaseRequestHandler<TIn, TOut, TValidator> : IRequestHandler<TIn, TOut> where TIn : IRequest<TOut> where TValidator : AbstractValidator<TIn>, new()
    {
        protected BaseRequestHandler()
        {
        }

        public virtual async Task<TOut> Handle(TIn request, CancellationToken cancellationToken)
        {
            if (typeof(TValidator) == typeof(NoValidationRequired<TIn>))
                return await HandleInternal(request, cancellationToken);

            var result = await new TValidator().ValidateAsync(request, cancellationToken);
            if (result.IsValid)
                return await HandleInternal(request, cancellationToken);

            var message = string.Join(Environment.NewLine, result.Errors.Select(x => x.ErrorMessage));
            throw new ArgumentNotValidException(message);
        }

        protected abstract Task<TOut> HandleInternal(TIn request, CancellationToken cancellationToken);
    }
}