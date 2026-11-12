using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Modules.Subjects.Commands.Delete;

public class DeleteSubjectCommand : MultipleIdentiferRequest<Guid>, IRequest<MultipleIdentiferResponse>;