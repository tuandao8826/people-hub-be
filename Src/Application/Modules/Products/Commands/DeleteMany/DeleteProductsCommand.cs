using Application.Common.Requests;
using Application.Common.Responses;
using MediatR;

namespace Application.Modules.Products.Commands.DeleteMany;

public class DeleteProductsCommand : MultipleIdentiferRequest<Guid>, IRequest<MultipleIdentiferResponse>;
