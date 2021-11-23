using HirdetoRendszer.Bll.Pagination;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace HirdetoRendszer.Bll.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TSource> Where<TSource>(this IQueryable<TSource> source, bool condition, Expression<Func<TSource, bool>> conditionTruePredicate, Expression<Func<TSource, bool>> conditionFalsePredicate = null)
        {
            if (condition)
            {
                return source.Where(conditionTruePredicate);
            }
            else if (conditionFalsePredicate != null)
            {
                return source.Where(conditionFalsePredicate);
            }
            else
            {
                return source;
            }
        }

        public static async Task<PageResponse<TSource>> ToPagedListAsync<TSource>(
            this IQueryable<TSource> source, PageRequest pageRequest, CancellationToken cancellationToken = default)
        {
            var totalCount = await source.CountAsync(cancellationToken);

            return new PageResponse<TSource>(
                await source.Skip((pageRequest.OldalSzam - 1) * pageRequest.OldalMeret)
                            .Take(pageRequest.OldalMeret)
                            .ToListAsync(cancellationToken), pageRequest.OldalSzam, totalCount);
        }
    }
}
