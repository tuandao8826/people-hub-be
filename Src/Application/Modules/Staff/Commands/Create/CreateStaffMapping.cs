using Application.Modules.Staff.Bases.Requests;
using Application.Modules.Staff.Entities;
using AutoMapper;

namespace Application.Modules.Staff.Commands.Create;

public class CreateStaffMapping : Profile
{
    public CreateStaffMapping()
    {
        CreateMap<CreateStaffCommand, StaffMember>().IncludeBase<StaffRequest, StaffMember>();
    }
}
