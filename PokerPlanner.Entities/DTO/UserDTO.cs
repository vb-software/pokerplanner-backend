using System;
using FluentValidation;

namespace PokerPlanner.Entities.DTO
{
    public class UserDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
    }

    public class UserDTOValidator : AbstractValidator<UserDto>
    {
        public UserDTOValidator()
        {
            RuleFor(o => o.FirstName).NotEmpty();
            RuleFor(o => o.LastName).NotEmpty();
            RuleFor(o => o.DateOfBirth).NotEmpty();
        }
    }
}