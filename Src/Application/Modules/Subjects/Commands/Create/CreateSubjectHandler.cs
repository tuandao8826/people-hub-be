using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Entites;
using AutoMapper;
using MediatR;

namespace Application.Modules.Subjects.Commands.Create;

public class CreateSubjectHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateSubjectCommand, Guid>
{
    public async Task<Guid> Handle(CreateSubjectCommand request, CancellationToken cancellationToken)
    {
        // 1. Map từ Command sang Entity Subject
        var subject = mapper.Map<Subject>(request);

        // 2. Thêm vào Repository thông qua UnitOfWork
        await unitOfWork.Repository<Subject>().AddAsync(subject, cancellationToken);

        // 3. Trả về ID của môn học vừa được tạo thành công
        return subject.Id;
    }
}