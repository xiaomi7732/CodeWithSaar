using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using CodeNameK.Biz;
using CodeNameK.DataContracts;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.Measure;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace CodeNameK.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICategory _categoryBiz;
        private readonly IDataPoint _dataPointBiz;
        private ObservableCollection<Category> _categoryCollection = new ObservableCollection<Category>();

        public MainViewModel(ICategory categoryBiz, IDataPoint dataPointBiz)
        {
            _categoryBiz = categoryBiz ?? throw new System.ArgumentNullException(nameof(categoryBiz));
            _dataPointBiz = dataPointBiz ?? throw new ArgumentNullException(nameof(dataPointBiz));
            InitializeCategoryCollection();
            CategoryCollectionView = CollectionViewSource.GetDefaultView(_categoryCollection);
            CategoryCollectionView.SortDescriptions.Add(new SortDescription(nameof(Category.Id), ListSortDirection.Ascending));
            CategoryCollectionView.Filter += ApplyFilter;
            CategoryCollectionView.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(nameof(CategoryHeader));
            };
        }

        public ICollectionView CategoryCollectionView { get; }

        public string CategoryHeader => $"Category ({CategoryCollectionView.Cast<object>().Count()})";

        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
        public ObservableCollection<ICartesianAxis> XAxes { get; } = new ObservableCollection<ICartesianAxis>();
        public ObservableCollection<ICartesianAxis> YAxes { get; } = new ObservableCollection<ICartesianAxis>();


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

        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get { return _selectedCategory; }
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    RaisePropertyChanged();
                    _ = UpdateSeriesAsync();
                }
            }
        }

        // This is a workaround since after Visible.Collapsed is set, the chart won't show.
        private double chartWidth = 0;
        public double ChartWidth
        {
            get { return chartWidth; }
            set
            {
                if (chartWidth != value)
                {
                    chartWidth = value;
                    RaisePropertyChanged();
                }
            }
        }


        private async Task UpdateSeriesAsync()
        {
            _ = Application.Current.Dispatcher.Invoke(() =>
            {
                XAxes.Clear();
                YAxes.Clear();
                Series.Clear();

                ChartWidth = 0;
                return 0;
            });

            if (string.IsNullOrEmpty(SelectedCategory?.Id))
            {
                return;
            }

            List<DateTimePoint> dataPoints = new List<DateTimePoint>();
            await foreach (DataPoint dataPoint in _dataPointBiz.GetDataPoints(SelectedCategory, default).ConfigureAwait(false))
            {
                dataPoints.Add(dataPoint.ToDateTimePoint());
            };
            dataPoints.Sort(DateTimePointComparers.DateTimeComparer);

            Application.Current.Dispatcher.Invoke(() =>
            {
                ChartWidth = double.NaN;
                Series.Add(
                    new LineSeries<DateTimePoint>()
                    {
                        Name = SelectedCategory.Id,
                        Values = dataPoints,
                        LineSmoothness = 0,
                        Fill = null,
                        Stroke = new SolidColorPaint(SKColors.DodgerBlue, 3),
                        GeometrySize = 12,
                        GeometryFill = new SolidColorPaint(SKColors.AliceBlue),
                        GeometryStroke = new SolidColorPaint(SKColors.SteelBlue, 4),
                    });

                XAxes.Add(
                    new Axis()
                    {
                        Labeler = value =>
                        {
                            try
                            {
                                if (value < DateTime.MinValue.Ticks || value > DateTime.MaxValue.Ticks)
                                {
                                    return string.Empty;
                                }
                                return new DateTime((long)value).ToString("MM/dd HH:mm");
                            }
                            catch (Exception ex)
                            {
#if DEBUG
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    MessageBox.Show($"Unexpected Error. Details: {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                                });
#endif
                                return string.Empty;
                            }
                        },
                        LabelsRotation = 30,
                        UnitWidth = TimeSpan.FromDays(1).Ticks,
                        MinStep = TimeSpan.FromMinutes(5).Ticks,
                    }
                );
            });
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
                            _ = Application.Current.Dispatcher.Invoke(() =>
                            {
                                MessageBox.Show("Can't add a category without a name. Please input a name.", "Failed Creating Category", MessageBoxButton.OK, MessageBoxImage.Error);
                                return 0;
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