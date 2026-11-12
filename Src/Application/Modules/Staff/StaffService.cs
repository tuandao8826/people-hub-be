using Application.Common.Extensions;
using Application.Common.Interfaces.DependencyInjection;
using Application.Common.Interfaces.Persistence;
using Application.Common.Interfaces.Persistence.Repositories;
using Application.Common.Responses;
using Application.Modules.Staff.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application.Modules.Staff;

public interface IStaffService : IScopedService
{
    Task<PaginationResponse<StaffResponse>> ListAsync(PaginationParams pagination, CancellationToken cancellationToken);
    Task<StaffResponse?> GetAsync(Guid id, CancellationToken cancellationToken);
    Task<StaffResponse?> SaveAsync(Guid? id, StaffRequest request, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
}

public class DuplicateStaffCodeException : Exception;

public class StaffService(IUnitOfWork unitOfWork, IMapper mapper) : IStaffService
{
    private readonly IRepository<StaffMember> staff = unitOfWork.Repository<StaffMember>();

    public Task<PaginationResponse<StaffResponse>> ListAsync(PaginationParams pagination, CancellationToken cancellationToken)
        => staff.Find().AsNoTracking().OrderBy(member => member.Code)
            .ProjectTo<StaffResponse>(mapper.ConfigurationProvider)
            .ToPageListAsync(pagination, cancellationToken);

    public Task<StaffResponse?> GetAsync(Guid id, CancellationToken cancellationToken)
        => staff.Find(member => member.Id == id).AsNoTracking()
            .ProjectTo<StaffResponse>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync(cancellationToken);

    public async Task<StaffResponse?> SaveAsync(Guid? id, StaffRequest request, CancellationToken cancellationToken)
    {
        StaffMember? member = id.HasValue
            ? await staff.GetByIdAsync(id.Value, cancellationToken)
            : new StaffMember();

        if (member is null)
        {
            return null;
        }

        member.Code = request.Code.Trim();
        member.Name = request.Name.Trim();
        member.Email = NormalizeOptional(request.Email);
        member.Phone = NormalizeOptional(request.Phone);
        member.Department = NormalizeOptional(request.Department);
        member.JobTitle = NormalizeOptional(request.JobTitle);
        member.Status = request.Status;
        member.JoinedOn = request.JoinedOn?.ToUniversalTime();

        try
        {
            if (id.HasValue)
            {
                await staff.UpdateAsync(member, cancellationToken);
            }
            else
            {
                await staff.AddAsync(member, cancellationToken);
            }
        }
        catch (DbUpdateConcurrencyException) when (id.HasValue)
        {
            return null;
        }
        catch (DbUpdateException exception) when (exception.InnerException is PostgresException
            { SqlState: PostgresErrorCodes.UniqueViolation, ConstraintName: StaffMemberConfiguration.CodeIndex })
        {
            throw new DuplicateStaffCodeException();
        }

        return mapper.Map<StaffResponse>(member);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        => await staff.Find(member => member.Id == id).ExecuteDeleteAsync(cancellationToken) > 0;

    private static string? NormalizeOptional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
