using Application.Common.Interfaces.Persistence;
using Application.Modules.Categories.Bases.Responses;
using Application.Modules.Categories.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Modules.Categories.Commands.Create;

public class CreateCategoryHandler(IUnitOfWork unitOfWork, IMapper mapper) : IRequestHandler<CreateCategoryCommand, CategoryDefaultResponse>
{
    public async Task<CategoryDefaultResponse> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        Category category = mapper.Map<Category>(request);

        string? parentPath = null;
        if (category.ParentId.HasValue)
        {
            Category parentCategory = (await unitOfWork.Repository<Category>().GetByIdAsync(category.ParentId, cancellationToken))!;
            parentPath = parentCategory.HierarchyPath;
        }

        await unitOfWork.Repository<Category>().AddAsync(category.SetHierarchy(parentPath), cancellationToken);
        
        return await unitOfWork.Repository<Category>()
            .Find(x => x.Id == category.Id)
            .ProjectTo<CategoryDefaultResponse>(mapper.ConfigurationProvider)
            .FirstAsync(cancellationToken);
    }
}
