using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Subjects.Entites;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Subjects.Commands.Update;

public class UpdateSubjectHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateSubjectCommand, Guid>
{
    public async Task<Guid> Handle(UpdateSubjectCommand request, CancellationToken cancellationToken)
    {
        // 1. Tìm môn học theo ID, nếu không tồn tại thì báo lỗi (tùy thuộc vào class BadRequestException hoặc NotFoundException trong project của bạn)
        var subject = await unitOfWork.Repository<Subject>()
            .Find(x => x.Id == request.SubjectId)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new BadRequestException($"Không tìm thấy môn học với ID: {request.SubjectId}");

        // 2. Kiểm tra xem SubjectCode mới có bị trùng với môn học khác hay không (nếu có thay đổi SubjectCode)
        if (subject.SubjectCode != request.Data.SubjectCode)
        {
            var codeExists = await unitOfWork.Repository<Subject>()
                .AnyAsync(x => x.SubjectCode == request.Data.SubjectCode && x.Id != request.SubjectId, cancellationToken);

            if (codeExists)
            {
                throw new BadRequestException($"Mã môn học '{request.Data.SubjectCode}' đã tồn tại trong hệ thống.");
            }
        }

        // 3. Map dữ liệu mới vào entity hiện tại
        mapper.Map(request.Data, subject);

        // 4. Cập nhật vào Database qua UnitOfWork
        await unitOfWork.Repository<Subject>().UpdateAsync(subject, cancellationToken);

        return request.SubjectId;
    }
}