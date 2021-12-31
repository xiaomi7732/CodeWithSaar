using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.Contracts;
using CodeNameK.DataContracts;

namespace CodeNameK.BIZ.Interfaces;

public interface ICategory
{
    IEnumerable<Category> GetAllCategories();

    Task<OperationResult<Category>> AddCategoryAsync(Category newCategory, bool overwrite = false, CancellationToken cancellationToken = default);
}