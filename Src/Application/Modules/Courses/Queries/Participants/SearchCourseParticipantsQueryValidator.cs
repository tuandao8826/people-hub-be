using FluentValidation;

namespace Application.Modules.Courses.Queries.Participants;

public class SearchCourseParticipantsQueryValidator : AbstractValidator<SearchCourseParticipantsQuery>
{
    public SearchCourseParticipantsQueryValidator()
    {
        RuleFor(query => query.CourseId)
            .NotEmpty()
            .WithMessage("Phải cung cấp ID khóa học.");
    }
}
