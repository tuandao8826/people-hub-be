using Application.Modules.Courses.Entities;
using AutoMapper;

namespace Application.Modules.Courses.Commands.Update;

public class UpdateCourseMapping : Profile
{
    public UpdateCourseMapping()
    {
        CreateMap<UpdateCourseRequest, Course>();
    }
}