#nullable enable

using Android.App;
using AndroidX.Lifecycle;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.DataContracts;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeNameK.Droid.ViewModels
{
    internal class CategoryListViewModel : AndroidViewModel
    {
        private readonly App _application;
        private List<Category>? _categories;
        public CategoryListViewModel(Application application) : base(application)
        {
            if (application is not App app)
            {
                throw new InvalidCastException("Can't cast Application instance to App instance.");
            }
            _application = app;
        }

        public List<Category> Categories
        {
            get
            {
                if (_categories is null)
                {
                    LoadCategories();
                }
                return _categories!;
            }
        }

        private void LoadCategories()
        {
            ICategory categoryBiz = _application.ServiceProvider.GetRequiredService<ICategory>();
            _categories = categoryBiz.GetAllCategories().OrderBy(item => item.Id, StringComparer.OrdinalIgnoreCase).ToList();
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
        }

        public void SortCategories()
        {
            _categories?.Sort(CategoryIdOrdinalIgnoreCaseComparer.Instance);
        }
    }
}