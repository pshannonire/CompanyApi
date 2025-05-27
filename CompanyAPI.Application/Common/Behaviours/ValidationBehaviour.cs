using CompanyAPI.Application.Common.Models;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace CompanyAPI.Application.Common.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task.WhenAll(
                    _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

                var failures = validationResults
                    .Where(r => r.Errors.Any())
                    .SelectMany(r => r.Errors)
                    .ToList();

                if (failures.Any())
                {
                    var errors = failures.Select(f => f.ErrorMessage).ToList();
                    var errorMessage = string.Join("; ", errors);

                    if (typeof(TResponse).IsGenericType &&
                        typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                    {
                        var resultType = typeof(TResponse).GetGenericArguments()[0];
                        var failedResult = CreateFailedResult(resultType, errorMessage);
                        return (TResponse)failedResult;
                    }

                    throw new ValidationException(failures);
                }
            }
            return await next();
        }

        private static object CreateFailedResult(Type resultType, string errorMessage)
        {
            var resultGenericType = typeof(Result<>).MakeGenericType(resultType);

            return Activator.CreateInstance(resultGenericType,
                BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                new object[] { null, false, errorMessage },
                null)!;
        }
    }
}