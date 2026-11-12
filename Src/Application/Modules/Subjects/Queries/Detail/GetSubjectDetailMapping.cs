using Application.Modules.Subjects.Bases.Responses;
using Application.Modules.Subjects.Entites;
using AutoMapper;

namespace Application.Modules.Subjects.Queries.Detail;

public class GetSubjectDetailMapping : Profile
{
    public GetSubjectDetailMapping()
    {
        CreateMap<Subject, SubjectDefaultResponse>();
    }
}