using Application.Modules.Courses.Bases.Responses.Courses;
using Application.Modules.Courses.Commands.Update;
using Application.Modules.Courses.Entities;
using Application.Modules.Courses.Bases.Responses.CourseParticipant;
using AutoMapper;

namespace Application.Modules.Courses.Bases.Profiles.Courses;

public class CourseMappingProfile : Profile
{
    public CourseMappingProfile()
    {
        CreateMap<Course, CourseDefaultResponse>();
        CreateMap<CourseParticipant, CourseParticipantResponse>()
            .ForMember(destination => destination.StaffCode,
                options => options.MapFrom(source => source.StaffMember!.Code))
            .ForMember(destination => destination.StaffName,
                options => options.MapFrom(source => source.StaffMember!.Name));
        CreateMap<UpdateCourseRequest, Course>();
    }
}
