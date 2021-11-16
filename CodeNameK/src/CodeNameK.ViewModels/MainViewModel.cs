using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CodeNameK.Biz;
using CodeNameK.DataContracts;

namespace CodeNameK.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICategory _categoryBiz;
        private ObservableCollection<Category> _categoryCollection = new ObservableCollection<Category>();

        public MainViewModel(ICategory categoryBiz)
        {
            _categoryBiz = categoryBiz ?? throw new System.ArgumentNullException(nameof(categoryBiz));
            InitializeCategoryCollection();
            CategoryCollectionView = CollectionViewSource.GetDefaultView(_categoryCollection);
            CategoryCollectionView.SortDescriptions.Add(new SortDescription(nameof(Category.Id), ListSortDirection.Ascending));
            CategoryCollectionView.Filter += ApplyFilter;
            CategoryCollectionView.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(nameof(CategoryCount));
            };
        }

        public ICollectionView CategoryCollectionView { get; }

        public int CategoryCount
        {
            get
            {
                return CategoryCollectionView.Cast<object>().Count();
            }
        }

        private string? _categoryText;
        public string? CategoryText
        {
            get { return _categoryText; }
            set
            {
                if (!string.Equals(_categoryText, value, StringComparison.InvariantCulture))
                {
                    _categoryText = value;
                    CategoryCollectionView.Refresh();
                    RaisePropertyChanged();
                }
            }
        }

        private ICommand? _addCategoryCommand;
        public ICommand AddCategoryCommand
        {
            get
            {
                if (_addCategoryCommand is null)
                {
                    _addCategoryCommand = new RelayCommand(p =>
                    {
                        if (string.IsNullOrEmpty(CategoryText))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Can't add a category without a name. Please input a name.", "Failed Creating Category", MessageBoxButton.OK, MessageBoxImage.Error);
                            });
                            return;
                        }

                        Task.Run(async () =>
                        {
                            OperationResult<Category> createCategoryResult = await _categoryBiz.AddCategoryAsync(new Category() { Id = CategoryText }, default);
                            if (!createCategoryResult.IsSuccess)
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show($"Failed creating a category. Details: {createCategoryResult.Reason}", "Failed Creating Category", MessageBoxButton.OK, MessageBoxImage.Error);
                                    CategoryText = string.Empty;
                                });
                                return;
                            }

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _categoryCollection.Add(createCategoryResult!);
                                MessageBox.Show($"Category {createCategoryResult.Entity?.Id} is created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                                CategoryText = string.Empty;
                            });
                        });
                    }, p => !string.IsNullOrEmpty(CategoryText));
                }
                return _addCategoryCommand;
            }
        }

        private void InitializeCategoryCollection()
        {
            foreach (Category category in _categoryBiz.GetAllCategories().OrderBy(c => c.Id, StringComparer.InvariantCultureIgnoreCase))
            {
                _categoryCollection.Add(category);
            }
        }

        private bool ApplyFilter(object id)
        {
            if (string.IsNullOrEmpty(CategoryText))
            {
                return true;
            }

            if (id is Category categoryObj && !string.IsNullOrEmpty(categoryObj.Id))
            {
                return categoryObj.Id.Contains(CategoryText, StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}