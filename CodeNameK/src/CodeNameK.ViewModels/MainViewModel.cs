using System;
using System.Collections.ObjectModel;
using System.Linq;
using CodeNameK.Biz;
using CodeNameK.DataContracts;

namespace CodeNameK.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICategory _categoryBiz;

        public MainViewModel(ICategory categoryBiz)
        {
            _categoryBiz = categoryBiz ?? throw new System.ArgumentNullException(nameof(categoryBiz));
            InitializeCategoryCollection();
        }

        public ObservableCollection<Category> CategoryCollection { get; } = new ObservableCollection<Category>();

        private void InitializeCategoryCollection()
        {
            foreach(Category category in _categoryBiz.GetAllCategories().OrderBy(c => c.Id, StringComparer.InvariantCultureIgnoreCase))
            {
                CategoryCollection.Add(category);
            }
        }
    }
}