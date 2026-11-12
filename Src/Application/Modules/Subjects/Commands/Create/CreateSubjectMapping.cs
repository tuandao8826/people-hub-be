using Application.Modules.Subjects.Entites;
using AutoMapper;

namespace Application.Modules.Subjects.Commands.Create;

public class CreateSubjectMapping : Profile
{
    public CreateSubjectMapping()
    {
        CreateMap<CreateSubjectCommand, Subject>();
    }
}