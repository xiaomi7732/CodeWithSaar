using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DAL.Interfaces
{
    public interface ICategoryRepo
    {
        /// <summary>
        /// Adds a category. Return the id for the category on success.
        /// </summary>
        Task<string> AddCategoryAsync(Category category, CancellationToken cancellationToken);

        /// <summary>
        /// Get a list of categories
        /// </summary>
        IEnumerable<Category> GetAllCategories();
    }
}