using System;
using FluentValidation;
using PokerPlanner.Entities.Domain.Mongo;

namespace PokerPlanner.Entities.DTO
{
    public class CreateWorkspaceDto
    {
        public string Name { get; set; }
        public Guid Guid { get; set; }
        public Configuration Configuration { get; set; }
    }

    public class CreateWorkspaceDtoValidator : AbstractValidator<CreateWorkspaceDto>
    {
        public CreateWorkspaceDtoValidator()
        {
            RuleFor(o => o.Name).NotEmpty();
        }
    }
}