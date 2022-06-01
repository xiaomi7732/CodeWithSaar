#nullable enable

using Android.App;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.DataContracts;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeNameK.Droid.ViewModels
{
    internal class CategoryListViewModel : AndroidViewModelBase
    {
        public CategoryListViewModel(Application application) : base(application)
        {
        }

        public ObservableCollection<Category> Categories { get; } = new ObservableCollection<Category>();


        public void LoadCategories()
        {
            ICategory categoryBiz = GetRequiredService<ICategory>();
            Categories.Clear();

            foreach (Category item in categoryBiz.GetAllCategories().OrderBy(item => item.Id, StringComparer.OrdinalIgnoreCase))
            {
                Categories.Add(item);
            }
#if DEBUG
            if (Categories.Count == 0)
            {
                for (int i = 0; i < 100; i++)
                {
                    Categories.Add(new Category()
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
                int index = FindIndexToInsert(newInstance);

                if (index == -1)
                {
                    Categories.Add(newInstance);
                }
                else
                {
                    Categories.Insert(index, newInstance);
                }
            }
            return operationResult;
        }

        private int FindIndexToInsert(Category category)
        {
            Category item = Categories.FirstOrDefault(item => string.Compare(item.Id, category.Id, StringComparison.OrdinalIgnoreCase) > 0);
            return item is null ? -1 : Categories.IndexOf(item);
        }
    }
}