using client.Utilities;
using FluentValidation;

namespace client.Requests
{
    public class RequestInputValidator : AbstractValidator<RequestInput>
    {
        public RequestInputValidator()
        {
            RuleFor(r => r.Description).NotNull().NotEmpty().MinimumLength(20).MaximumLength(800);
            RuleFor(r => r.Email).EmailAddress();
            RuleFor(r => r.PhoneNumber).Matches(ValidationHelper.PhoneRegex);
            RuleFor(r => r.Name).NotNull().NotEmpty().MinimumLength(5).MaximumLength(50);
            RuleFor(r => r.Region).NotNull().NotEmpty();
            RuleFor(r => r.PostCode).NotNull().NotEmpty();
            RuleFor(r => r.AreaInRegion).NotNull().NotEmpty();
            RuleFor(r => r.Topic).IsInEnum();
        }
    }
}