using Application.Common.Definitions.Messages;
using Application.Common.Extensions;
using Application.Common.Interfaces.Persistence;
using Core.Bases;
using FluentValidation;
using System.Linq.Expressions;

namespace Application.Common.Requests;

public class MultipleIdentiferRequest<TId>
{
	public IEnumerable<TId>? Ids { get; set; }
}

public class MultipleIdentiferValidator<TEntity, TId> : AbstractValidator<MultipleIdentiferRequest<TId>> where TEntity : BaseEntity<TId>
{
    public MultipleIdentiferValidator(IUnitOfWork unitOfWork, Expression<Func<TEntity, bool>>? filter = null)
    {
        RuleFor(x => x.Ids)
            .NotEmpty()
            .WithMessage(Message<TEntity>.Required(x => x.Id))
            .NotDuplicate()
            .WithMessage(Message<TEntity>.Duplicate(x => x.Id))
            .Must(ids => unitOfWork.HasIds<TEntity, TId>(ids!, filter))
            .WithMessage(Message<TEntity>.NotFound(x => x.Id));
	}
}
