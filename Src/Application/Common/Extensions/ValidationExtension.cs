using Application.Common.Interfaces.Persistence;
using Core.Bases;
using Core.Bases.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Extensions;

public static class ValidationExtension
{
	public static bool HasId<TEntity>(this IUnitOfWork unitOfWork, Guid id, Expression<Func<TEntity, bool>>? filter = null) where TEntity : BaseEntity
		=> unitOfWork.Repository<TEntity>().Any(x => x.Id == id);

	public static bool HasId<TEntity, TId>(this IUnitOfWork unitOfWork, TId id, Expression<Func<TEntity, bool>>? filter = null) where TEntity : BaseEntity<TId>
		=> unitOfWork.Repository<TEntity>().Any(x => EqualityComparer<TId>.Default.Equals(x.Id, id));

	public static bool HasIds<TEntity>(this IUnitOfWork unitOfWork, IEnumerable<Guid> ids, Expression<Func<TEntity, bool>>? filter = null) where TEntity : BaseEntity
		=> unitOfWork.Repository<TEntity>().Count(x => ids.Contains(x.Id)) == ids.Count();

	public static bool HasIds<TEntity, TId>(this IUnitOfWork unitOfWork, IEnumerable<TId> ids, Expression<Func<TEntity, bool>>? filter = null) where TEntity : BaseEntity<TId>
		=> unitOfWork.Repository<TEntity>().Count(x => ids.Contains(x.Id)) == ids.Count();

    public static bool HasCode<TEntity>(this IUnitOfWork unitOfWork, string code, Expression<Func<TEntity, bool>>? filter = null) where TEntity : BaseEntity, ICode
        => unitOfWork.Repository<TEntity>().Any(x => x.Code == code);

    public static bool HasCodes<TEntity>(this IUnitOfWork unitOfWork, IEnumerable<string> codes, Expression<Func<TEntity, bool>>? filter = null) where TEntity : BaseEntity, ICode
        => unitOfWork.Repository<TEntity>().Count(x => codes.Contains(x.Code)) == codes.Count();
}
