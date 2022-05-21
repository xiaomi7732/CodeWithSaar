#nullable enable

using Android.App;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.DataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.Droid.ViewModels
{
    internal class CategoryListViewModel : AndroidViewModelBase
    {
        private readonly List<Category> _categories;
        public CategoryListViewModel(Application application) : base(application)
        {
            _categories = new List<Category>();
        }

        public List<Category> Categories
        {
            get
            {
                return _categories;
            }
        }


        public void LoadCategories()
        {
            ICategory categoryBiz = GetRequiredService<ICategory>();
            _categories.Clear();
            _categories.AddRange(categoryBiz.GetAllCategories().OrderBy(item => item.Id, StringComparer.OrdinalIgnoreCase));
#if DEBUG
            if (_categories.Count == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    _categories.Add(new Category()
                    {
                        Id = "Test Category " + (i + 1),
                    });
                }
            }
#endif
            RaisePropertyChanged(nameof(Categories));
        }

        public async Task<OperationResult<Category>> AddCategoryAsync(string newCategoryName, CancellationToken cancellationToken)
        {
            Category newInstance = new Category() { Id = newCategoryName };
            ICategory categoryBiz = GetRequiredService<ICategory>();
            OperationResult<Category> operationResult = await categoryBiz.AddCategoryAsync(newInstance, overwrite: false, cancellationToken);
            if (operationResult.IsSuccess)
            {
                _categories.Add(newInstance);
                _categories.Sort(CategoryIdOrdinalIgnoreCaseComparer.Instance);
                RaisePropertyChanged(nameof(Categories));
            }
            return operationResult;
        }
    }
}