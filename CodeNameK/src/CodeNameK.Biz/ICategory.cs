using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.Biz;

public interface ICategory
{
    IEnumerable<Category> AllCategories { get; }

    Task<OperationResult<Category>> AddCategoryAsync(Category newCategory, CancellationToken cancellationToken);
}