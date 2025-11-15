using FluentValidation;
using MediatR;
using OrderManagement.Application.Exceptions;

namespace OrderManagement.Application.Pipelines;

/// <summary>
/// Validation Behavior
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public sealed class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    /// <summary>
    /// Pipeline handler. Perform any additional behavior and await the <paramref name="next" /> delegate as necessary
    /// </summary>
    /// <param name="request">Incoming request</param>
    /// <param name="next">Awaitable delegate for the next action in the pipeline. Eventually this delegate represents the handler.</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>
    /// Awaitable task returning the <typeparamref name="TResponse" />
    /// </returns>
    /// <exception cref="ValidationFailedException"></exception>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if (validators.Any()) await ExecuteValidations(request, cancellationToken);

        return await next(cancellationToken);
    }

    private async Task ExecuteValidations(TRequest request, CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);

        var failures = (
            await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken)))
            )
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(x => x.ErrorMessage)
            .ToList();

        if (failures.Count != 0) throw new ValidationFailedException(failures);
    }
}