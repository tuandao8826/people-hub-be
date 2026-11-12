using Application.Modules.Courses.Entities;
using AutoMapper;

namespace Application.Modules.Courses.Commands.Create;

public class CreateCourseMapping : Profile
{
    public CreateCourseMapping()
    {
        CreateMap<CreateCourseCommand, Course>();
    }
}