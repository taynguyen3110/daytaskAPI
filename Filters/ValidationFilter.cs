using System.Text.Json;
using FluentValidation;
using daytask.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace daytask.Filters
{
    public class ValidationFilter : IAsyncActionFilter, IFilterMetadata
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ActionArguments.Any())
            {
                await next();
                return;
            }

            var errors = new List<string>();
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;

                if (validator != null)
                {
                    var validationContext = new ValidationContext<object>(argument);
                    var validationResult = await validator.ValidateAsync(validationContext);

                    if (!validationResult.IsValid)
                    {
                        errors.AddRange(validationResult.Errors.Select(x => x.ErrorMessage));
                    }
                }
            }

            if (errors.Any())
            {
                var response = ApiResponse<object>.ErrorResponse(
                    "Validation failed",
                    400,
                    errors
                );

                context.Result = new BadRequestObjectResult(response);
                return;
            }

            await next();
        }
    }
} 