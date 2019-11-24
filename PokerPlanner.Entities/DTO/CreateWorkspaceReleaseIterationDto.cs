using System;
using FluentValidation;

namespace PokerPlanner.Entities.DTO
{
    public class CreateWorkspaceReleaseIterationDto
    {
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CreateWorkspaceReleaseIterationDtoValidator : AbstractValidator<CreateWorkspaceReleaseIterationDto>
    {
        public CreateWorkspaceReleaseIterationDtoValidator()
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