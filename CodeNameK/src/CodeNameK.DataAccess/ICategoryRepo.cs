using System.Collections.Generic;
using System.Threading.Tasks;
using CodeNameK.DataContracts;

namespace CodeNameK.DataAccess
{
    public interface ICategoryRepo
    {
        /// <summary>
        /// Adds a category. Return the id for the category on success.
        /// </summary>
        Task<string> AddCategoryAsync(Category category);

        /// <summary>
        /// Get a list of categories
        /// </summary>
        /// <returns></returns>
        IAsyncEnumerable<Category> GetAllCategories();
    }
}