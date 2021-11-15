using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataAccess;
using CodeNameK.DataContracts;

namespace CodeNameK.Biz;

internal class BizCategory : ICategory
{
    private readonly ICategoryRepo _categoryRepo;

    public BizCategory(ICategoryRepo categoryRepo)
    {
        _categoryRepo = categoryRepo ?? throw new System.ArgumentNullException(nameof(categoryRepo));
    }

    public IEnumerable<Category> GetAllCategories() => _categoryRepo.GetAllCategories();

    public async Task<OperationResult<Category>> AddCategoryAsync(Category newCategory, CancellationToken cancellationToken)
    {
        // Business Logic: Category name can't be null;
        if (string.IsNullOrEmpty(newCategory.Id))
        {
            return new OperationResult<Category>()
            {
                IsSuccess = false,
                Entity = newCategory,
                Reason = "Category name is required.",
            };
        }

        // Business Logic: Category name can't be longer than 100 characters
        if (newCategory.Id.Length > 100)
        {
            return new OperationResult<Category>()
            {
                IsSuccess = false,
                Entity = newCategory,
                Reason = "Category name can't be long than 100 characters.",
            };
        }

        await _categoryRepo.AddCategoryAsync(newCategory, cancellationToken).ConfigureAwait(false);
        return new OperationResult<Category>()
        {
            IsSuccess = true,
            Entity = newCategory,
        };
    }
}