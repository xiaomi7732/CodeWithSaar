using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.DAL.Interfaces;
using CodeNameK.DataContracts;

namespace CodeNameK.BIZ;

internal class BizCategory : ICategory
{
    private readonly ICategoryRepo _categoryRepo;

    public BizCategory(ICategoryRepo categoryRepo)
    {
        _categoryRepo = categoryRepo ?? throw new System.ArgumentNullException(nameof(categoryRepo));
    }

    public IEnumerable<Category> GetAllCategories() => _categoryRepo.GetAllCategories();

    public async Task<OperationResult<Category>> AddCategoryAsync(Category newCategory, bool overwrite = false, CancellationToken cancellationToken = default)
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

        // Business Logic: Unless specified, do not create a category that already exist
        if (!overwrite && GetAllCategories().Any(c => string.Equals(c.Id, newCategory.Id, StringComparison.OrdinalIgnoreCase)))
        {
            return new OperationResult<Category>()
            {
                IsSuccess = false,
                Entity = newCategory,
                Reason = $"The category of {newCategory.Id} already exists.",
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