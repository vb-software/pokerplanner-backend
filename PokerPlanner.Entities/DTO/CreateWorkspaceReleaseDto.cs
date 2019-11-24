using System;
using FluentValidation;

namespace PokerPlanner.Entities.DTO
{
    public class CreateWorkspaceReleaseDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CreateWorkspaceReleaseDtoValidator : AbstractValidator<CreateWorkspaceReleaseDto>
    {
        public CreateWorkspaceReleaseDtoValidator()
        {
            RuleFor(o => o.Name).NotEmpty();
            RuleFor(o => o.StartDate)
                .NotEqual(DateTime.MinValue)
                .NotEqual(o => o.EndDate)
                .LessThan(o => o.EndDate);
            RuleFor(o => o.EndDate)
                .NotEqual(DateTime.MinValue)
                .NotEqual(o => o.StartDate)
                .GreaterThan(o => o.StartDate);
        }
    }
}