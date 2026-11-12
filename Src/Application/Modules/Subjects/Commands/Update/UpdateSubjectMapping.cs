using Application.Modules.Subjects.Entites;
using AutoMapper;

namespace Application.Modules.Subjects.Commands.Update;

public class UpdateSubjectMapping : Profile
{
    public UpdateSubjectMapping()
    {
        CreateMap<UpdateSubjectRequest, Subject>();
    }
}