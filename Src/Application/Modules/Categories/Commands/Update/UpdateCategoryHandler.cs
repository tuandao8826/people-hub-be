using Application.Common.Definitions.Messages;
using Application.Common.Exceptions;
using Application.Common.Interfaces.Persistence;
using Application.Modules.Categories.Bases.Responses;
using Application.Modules.Categories.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Categories.Commands.Update;

public class UpdateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<UpdateCategoryCommand, CategoryDefaultResponse>
{
    public async Task<CategoryDefaultResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category category = await unitOfWork.Repository<Category>().GetByIdAsync(request.CategoryId, cancellationToken)
            ?? throw new BadRequestException(Message<Category>.NotFound());

        if (await unitOfWork.Repository<Category>().AnyAsync(x => x.Id != category.Id && x.Name.ToLower().Equals(request.Data.Name.ToLower()), cancellationToken))
        {
            throw new BadRequestException(Message<Category>.AlreadyExists(x => x.Name));
        }

        mapper.Map(request.Data, category);
        
        await unitOfWork.Repository<Category>().UpdateAsync(category, cancellationToken);
        
        return await unitOfWork.Repository<Category>()
            .Find(x => x.Id == category.Id)
            .ProjectTo<CategoryDefaultResponse>(mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}
