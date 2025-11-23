using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Modules.Categories.Commands.DeleteMany;

public class DeleteCategoriesCommand : MultipleIdentiferRequest<Guid>, IRequest<MultipleIdentiferResponse>;