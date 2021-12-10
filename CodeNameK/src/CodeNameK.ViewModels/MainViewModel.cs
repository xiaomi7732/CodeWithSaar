using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Contracts.DataContracts;
using CodeNameK.Core.Utilities;
using CodeNameK.DataContracts;
using LiveChartsCore;
using LiveChartsCore.Drawing;
using LiveChartsCore.Kernel;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries.Segments;
using LiveChartsCore.SkiaSharpView.Painting;
using Microsoft.Extensions.Logging;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace CodeNameK.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ICategory _categoryBiz;
        private readonly IDataPoint _dataPointBiz;
        private readonly ISync _syncService;
        private readonly IChartAxisExpansion _chartAxisExpansion;
        private readonly ILogger _logger;
        private ObservableCollection<Category> _categoryCollection = new ObservableCollection<Category>();

        public MainViewModel(
            ICategory categoryBiz,
            IDataPoint dataPointBiz,
            ISync syncService,
            IChartAxisExpansion chartAxisExpansion,
            DataPointViewModel dataPointOperator,
            ErrorRevealer errorRevealer,
            ILogger<MainViewModel> logger)
                : base(errorRevealer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _categoryBiz = categoryBiz ?? throw new System.ArgumentNullException(nameof(categoryBiz));
            _dataPointBiz = dataPointBiz ?? throw new ArgumentNullException(nameof(dataPointBiz));
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _chartAxisExpansion = chartAxisExpansion ?? throw new ArgumentNullException(nameof(chartAxisExpansion));
            SelectedDataPoint = dataPointOperator ?? throw new ArgumentNullException(nameof(dataPointOperator));

            InitializeCategoryCollection();
            CategoryCollectionView = CollectionViewSource.GetDefaultView(_categoryCollection);
            CategoryCollectionView.SortDescriptions.Add(new SortDescription(nameof(Category.Id), ListSortDirection.Ascending));
            CategoryCollectionView.Filter += ApplyFilter;
            CategoryCollectionView.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(nameof(CategoryHeader));
            };
            _syncStateText = String.Empty;

            ResetZoomCommand = new RelayCommand(ResetZoom);
            SyncCommand = new RelayCommand(SyncImp);
            AddCategoryCommand = new RelayCommand(AddCategoryImp, CanAddCategoryImp);
            PickPointCommand = new RelayCommand(PickPointImp);
        }

        public ICollectionView CategoryCollectionView { get; }

        public string CategoryHeader => $"Category ({CategoryCollectionView.Cast<object>().Count()})";

        public ObservableCollection<ISeries> Series { get; } = new ObservableCollection<ISeries>();
        public ObservableCollection<ICartesianAxis> XAxes { get; } = new ObservableCollection<ICartesianAxis>();
        public ObservableCollection<ICartesianAxis> YAxes { get; } = new ObservableCollection<ICartesianAxis>();
        private IDictionary<Category, (ICartesianAxis x, ICartesianAxis y)> _axesCache = new Dictionary<Category, (ICartesianAxis, ICartesianAxis)>();

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

        private DataPoint? _hoverPoint;
        public DataPointViewModel SelectedDataPoint { get; }

        public async Task UpdateSeriesAsync()
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;
            try
            {
                List<LineSeries<DataPoint>> existingSeries = Series.OfType<LineSeries<DataPoint>>().NullAsEmpty().ToList();
                foreach (var series in existingSeries)
                {
                    series.PointHovered -= SeriesPointHover;
                }

                if (string.IsNullOrEmpty(SelectedCategory?.Id))
                {
                    // No category, no data points.
                    Series.Clear();
                    return;
                }

                List<DataPoint> dataPoints = new List<DataPoint>();
                await foreach (DataPoint dataPoint in _dataPointBiz.GetDataPoints(SelectedCategory, default).ConfigureAwait(false))
                {
                    dataPoints.Add(dataPoint);
                };
                dataPoints.Sort(DatePointComparer.DateTimeComparer);

                double maxValue = _chartAxisExpansion.ExpandUp(dataPoints.Any() ? dataPoints.Max(point => point.Value) : 0);
                double minValue = _chartAxisExpansion.ExpandDown(dataPoints.Any() ? dataPoints.Min(point => point.Value) : 0);

                await syncContext;
                Series.Add(CreateNewSeries(dataPoints));
                foreach (ISeries existItem in existingSeries)
                {
                    Series.Remove(existItem);
                }

                XAxes.Clear();
                YAxes.Clear();
                if (_axesCache.ContainsKey(SelectedCategory))
                {
                    (ICartesianAxis xAxis, ICartesianAxis yAxis) = _axesCache[SelectedCategory];
                    yAxis.MaxLimit = maxValue;
                    yAxis.MinLimit = minValue;
                    XAxes.Add(xAxis);
                    YAxes.Add(yAxis);
                }
                else
                {
                    Axis newXAxis = CreateNewXAxis();
                    XAxes.Add(newXAxis);

                    Axis newYAxis = new Axis()
                    {
                        MaxLimit = maxValue,
                        MinLimit = minValue,
                    };
                    YAxes.Add(newYAxis);
                    _axesCache.Add(SelectedCategory, (newXAxis, newYAxis));
                }
            }
            catch (Exception ex)
            {
                _errorRevealer.Reveal(ex, $"Unexpected error in {nameof(UpdateSeriesAsync)}");
            }
        }

        private LineSeries<DataPoint> CreateNewSeries(List<DataPoint> dataPoints)
        {
            if (string.IsNullOrEmpty(SelectedCategory?.Id))
            {
                throw new InvalidOperationException("Should never create a series when no selected category.");
            }

            LineSeries<DataPoint> series = new LineSeries<DataPoint>()
            {
                Name = SelectedCategory.Id,
                TooltipLabelFormatter = FormatToolTip,
                Values = dataPoints,
                LineSmoothness = 0,
                Fill = null,
                Stroke = new SolidColorPaint(SKColors.DodgerBlue, 3),
                GeometrySize = 12,
                GeometryFill = new SolidColorPaint(SKColors.AliceBlue),
                GeometryStroke = new SolidColorPaint(SKColors.SteelBlue, 4),
                Mapping = SeriesMapping,
            };
            series.PointHovered += SeriesPointHover;
            return series;
        }

        private void SeriesMapping(DataPoint dataPoint, ChartPoint chartPoint)
        {
            chartPoint.PrimaryValue = dataPoint.Value;
            chartPoint.SecondaryValue = dataPoint.WhenUTC.ToLocalTime().Ticks;
        }

        private void SeriesPointHover(TypedChartPoint<DataPoint, LineBezierVisualPoint<SkiaSharpDrawingContext, CircleGeometry, CubicBezierSegment, SKPath>, LabelGeometry, SkiaSharpDrawingContext> point)
        {
            _logger.LogDebug("Hover point: {point}", point.Model);
            if (point.Model is null)
            {
                return;
            }
            _hoverPoint = point.Model;
        }

        string FormatToolTip(TypedChartPoint<DataPoint, LineBezierVisualPoint<SkiaSharpDrawingContext, CircleGeometry, CubicBezierSegment, SKPath>, LabelGeometry, SkiaSharpDrawingContext> point)
        {
            return string.Format($"{point?.Model?.WhenUTC.ToLocalTime():g}" + Environment.NewLine + $"{point?.PrimaryValue:N2}");
        }

        private Axis CreateNewXAxis()
        {
            string CreateLabeler(double value)
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
                    MessageBox.Show($"Unexpected Error. Details: {ex}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
#endif
                    return string.Empty;
                }
            }

            return new Axis()
            {
                Labeler = CreateLabeler,
                LabelsRotation = 30,
                UnitWidth = TimeSpan.FromDays(1).Ticks,
                MinStep = TimeSpan.FromMinutes(5).Ticks,
            };
        }

        public ICommand PickPointCommand { get; }
        private void PickPointImp(object? parameter)
        {
            SelectedDataPoint.SetModel(_hoverPoint);
            SelectedDataPoint.IsCurrentDateTimeMode = false;
        }

        private double _syncProgress;

        public double SyncProgress
        {
            get { return _syncProgress; }
            set
            {
                if (_syncProgress != value)
                {
                    _syncProgress = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _syncStateText;

        public string SyncStateText
        {
            get { return _syncStateText; }
            set
            {
                if (!string.Equals(_syncStateText, value, StringComparison.Ordinal))
                {
                    _syncStateText = value;
                    RaisePropertyChanged();
                }
            }
        }

        public ICommand SyncCommand { get; }
        private async void SyncImp(object? parameters)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;

            try
            {
                OperationResult<SyncStatistic> result = await _syncService.Sync(
                    new Progress<SyncProgress>(newProgress =>
                {
                    _logger.LogInformation("New sync progress reported: [{text}]{value:p}", newProgress.DisplayText, newProgress.Value);
                    Dispatch(() =>
                    {
                        SyncStateText = newProgress.DisplayText;
                        SyncProgress = newProgress.Value;
                        return 0;
                    });
                }), default).ConfigureAwait(false);

                await syncContext;
                if (result.IsSuccess)
                {
                    SyncStateText = "Success";
                    MessageBox.Show($"{result.Entity.Uploaded} files uploaded, {result.Entity.Downloaded} files downloaded.", "Sync Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    SyncStateText = "Fail";
                    _errorRevealer.Reveal(result.Reason, "Sync error");
                }

                // Post sync
                InitializeCategoryCollection();
                await UpdateSeriesAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                await syncContext;
                _errorRevealer.Reveal(ex.Message, "Unexpected sync error");
            }
        }

        public ICommand AddCategoryCommand { get; }
        private async void AddCategoryImp(object? parameter)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;
            try
            {
                if (string.IsNullOrEmpty(CategoryText))
                {
                    MessageBox.Show("Can't add a category without a name. Please input a name.", "Failed Creating Category", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                OperationResult<Category> createCategoryResult = await _categoryBiz.AddCategoryAsync(new Category() { Id = CategoryText }, default).ConfigureAwait(false);

                await syncContext;
                if (!createCategoryResult.IsSuccess)
                {
                    MessageBox.Show($"Failed creating a category. Details: {createCategoryResult.Reason}", "Failed Creating Category", MessageBoxButton.OK, MessageBoxImage.Error);
                    CategoryText = string.Empty;
                    return;
                }

                _categoryCollection.Add(createCategoryResult!);
                MessageBox.Show($"Category {createCategoryResult.Entity?.Id} is created!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                CategoryText = string.Empty;
            }
            catch (Exception ex)
            {
                _errorRevealer.Reveal(ex.Message, "Unexpected error!");
            }
        }
        private bool CanAddCategoryImp(object? parameter) => !string.IsNullOrEmpty(CategoryText);

        public ICommand ResetZoomCommand { get; }
        private void ResetZoom(object? parameter)
        {
            foreach (Axis x in XAxes)
            {
                ResetZoom(x);
            }
        }

        private void ResetZoom(Axis axis)
        {
            axis.MinLimit = null;
            axis.MaxLimit = null;
        }

        private void InitializeCategoryCollection()
        {
            foreach (Category category in _categoryBiz.GetAllCategories().OrderBy(c => c.Id, StringComparer.InvariantCultureIgnoreCase))
            {
                if (_categoryCollection.Contains(category))
                {
                    continue;
                }
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