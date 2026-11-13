using Application.Modules.Staff.Entities;
using AutoMapper;

namespace Application.Modules.Staff.Bases.Requests;

public class StaffRequestMapping : Profile
{
    public StaffRequestMapping()
    {
        CreateMap<StaffRequest, StaffMember>()
            .ForMember(member => member.Id, options => options.Ignore())
            .ForMember(member => member.CreatedAt, options => options.Ignore())
            .ForMember(member => member.Code, options => options.MapFrom(request => request.Code.Trim()))
            .ForMember(member => member.Name, options => options.MapFrom(request => request.Name.Trim()))
            .ForMember(member => member.Email, options => options.MapFrom(request => NormalizeOptional(request.Email)))
            .ForMember(member => member.Phone, options => options.MapFrom(request => NormalizeOptional(request.Phone)))
            .ForMember(member => member.Department, options => options.MapFrom(request => NormalizeOptional(request.Department)))
            .ForMember(member => member.JobTitle, options => options.MapFrom(request => NormalizeOptional(request.JobTitle)))
            .ForMember(member => member.JoinedOn, options => options.MapFrom(request => NormalizeJoinedOn(request.JoinedOn)));
    }

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static DateTimeOffset? NormalizeJoinedOn(DateTimeOffset? value)
        => value?.ToUniversalTime();
}
