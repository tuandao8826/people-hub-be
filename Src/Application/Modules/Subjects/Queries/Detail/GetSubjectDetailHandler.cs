using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Bases.Responses;
using Application.Modules.Subjects.Entites;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net;

namespace Application.Modules.Subjects.Queries.Detail;

public class GetSubjectDetailHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<GetSubjectDetailQuery, SubjectDefaultResponse>
{
    public async Task<SubjectDefaultResponse> Handle(GetSubjectDetailQuery request, CancellationToken cancellationToken)
    {
        return await unitOfWork.Repository<Subject>()
            .Find(x => x.Id == request.SubjectId)
            .AsNoTracking()
            .ProjectTo<SubjectDefaultResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException(Message<Subject>.NotFound(), HttpStatusCode.NotFound);
    }
}
