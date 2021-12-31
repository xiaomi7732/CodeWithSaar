using CodeNameK.BIZ;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.Contracts.CustomOptions;
using CodeNameK.Contracts.DataContracts;
using CodeNameK.Core.CustomExceptions;
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
using Microsoft.Extensions.Options;
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
        private readonly IDateRangeService _dateRangeService;
        private readonly IChartAxisExpansion _chartAxisExpansion;
        private readonly InternetAvailability _internetAvailability;
        private readonly ILogger _logger;
        private bool _initialSyncRequested;
        private ObservableCollection<Category> _categoryCollection = new ObservableCollection<Category>();

        public MainViewModel(
            ICategory categoryBiz,
            IDataPoint dataPointBiz,
            ISync syncService,
            IDateRangeService dateRangeService,
            IChartAxisExpansion chartAxisExpansion,
            DataPointViewModel dataPointOperator,
            InternetAvailability internetAvailability,
            IErrorRevealerFactory errorRevealer,
            BackgroundSyncProgress<UpSyncBackgroundService> upSyncProgress,
            BackgroundSyncProgress<DownSyncBackgroundService> downSyncProgress,
            IOptions<LocalStoreOptions> localStoreOptions,
            ILogger<MainViewModel> logger)
                : base(errorRevealer)
        {
            if (localStoreOptions?.Value is null)
            {
                throw new ArgumentNullException(nameof(localStoreOptions));
            }

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _categoryBiz = categoryBiz ?? throw new System.ArgumentNullException(nameof(categoryBiz));
            _dataPointBiz = dataPointBiz ?? throw new ArgumentNullException(nameof(dataPointBiz));
            _syncService = syncService ?? throw new ArgumentNullException(nameof(syncService));
            _syncService.SignInStatusChanged += OnSignInStatusChanged;
            _dateRangeService = dateRangeService ?? throw new ArgumentNullException(nameof(dateRangeService));
            _chartAxisExpansion = chartAxisExpansion ?? throw new ArgumentNullException(nameof(chartAxisExpansion));
            _internetAvailability = internetAvailability ?? throw new ArgumentNullException(nameof(internetAvailability));
            SelectedDataPoint = dataPointOperator ?? throw new ArgumentNullException(nameof(dataPointOperator));
            InitializeCategoryCollection();
            CategoryCollectionView = CollectionViewSource.GetDefaultView(_categoryCollection);
            CategoryCollectionView.SortDescriptions.Add(new SortDescription(nameof(Category.Id), ListSortDirection.Ascending));
            CategoryCollectionView.Filter += ApplyFilter;
            CategoryCollectionView.CollectionChanged += (sender, e) =>
            {
                RaisePropertyChanged(nameof(CategoryHeader));
            };
            _syncStateText = "No sync.";
            _upSyncStateText = string.Empty;

            _dataFolderPath = localStoreOptions.Value.DataStorePath.Replace('/', '\\');

            ResetZoomCommand = new RelayCommand(ResetZoom);
            SyncCommand = new AsyncRelayCommand(SyncImpAsync, canExecute: null, exceptionCallback: OnSyncImpException);
            AddCategoryCommand = new AsyncRelayCommand(AddCategoryImpAsync, CanAddCategoryImp, exceptionCallback: _errorRevealerFactory.CreateInstance($"Unhandled Exception Invoking {nameof(AddCategoryCommand)}").Reveal);
            PickPointCommand = new RelayCommand(PickPointImp);
            TodayOnlyCommand = new AsyncRelayCommand(TodayOnlyImpAsync, canExecute: null, exceptionCallback: _errorRevealerFactory.CreateInstance($"Unhandled exception invoking {TodayOnlyCommand}").Reveal);
            ExitCommand = new RelayCommand(ExitImp);
            CancelSignInCommand = new RelayCommand(CancelSignIn);

            SelectedDateRangeOption = DateRangeOptions.First();
            _selectedDateRangeOption = SelectedDateRangeOption;

            UpSyncQueueLength = _syncService.UpSyncQueueLength;
            upSyncProgress.ProgressChanged += BackgroundSyncProgress_ProgressChanged;
            RequestInitialSync().FireWithExceptionHandler(OnSyncImpException);

            DownSyncQueueLength = _syncService.DownSyncQueueLength;
            downSyncProgress.ProgressChanged += DownSyncProgress_ProgressChanged;

            _signInStatus = string.Empty;
        }

        public ICollectionView CategoryCollectionView { get; }

        public List<DateRangeItemViewModel> DateRangeOptions { get; } = new List<DateRangeItemViewModel>()
        {
            new DateRangeItemViewModel() { DisplayName = "All", Value = int.MinValue },
            new DateRangeItemViewModel() { DisplayName = "Today", Value = 1 },
            new DateRangeItemViewModel() { DisplayName = "Last 3 days", Value = 3 },
            new DateRangeItemViewModel() { DisplayName = "Last 7 days", Value = 7 },
            new DateRangeItemViewModel() { DisplayName = "This week", Value = 8 },
            new DateRangeItemViewModel() { DisplayName = "This month", Value = 30 },
        };

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
                    _ = UpdateSeriesAsync(default);
                    if (value != null)
                    {
                        _syncService.EnqueueDownSyncAsync(value, default)
                            .AsTask()
                            .FireWithExceptionHandler(_errorRevealerFactory.CreateInstance("Down sync error").Reveal);
                    }
                }
            }
        }

        private DateRangeItemViewModel _selectedDateRangeOption;
        public DateRangeItemViewModel SelectedDateRangeOption
        {
            get { return _selectedDateRangeOption; }
            set
            {
                if (_selectedDateRangeOption != value)
                {
                    _selectedDateRangeOption = value;
                    ApplyNewDateRangeAsync().FireWithExceptionHandler(_errorRevealerFactory.CreateInstance($"Unexpected error applying new date range").Reveal);
                    RaisePropertyChanged();
                }
            }
        }

        private DateTime _startDateRange = default;
        /// <summary>
        /// Gets or sets the start date range time in local time.
        /// </summary>
        /// <value></value>
        public DateTime StartDateRange
        {
            get { return _startDateRange; }
            set
            {
                if (_startDateRange != value)
                {
                    _startDateRange = value;
                    RaisePropertyChanged();
                }

            }
        }

        private DateTime _endDateRange = default;
        /// <summary>
        /// Gets or sets the end date range time in form of local time.
        /// </summary>
        /// <value></value>
        public DateTime EndDateRange
        {
            get { return _endDateRange; }
            set { _endDateRange = value; }
        }

        private string _dataFolderPath;

        public string DataFolderPath
        {
            get { return _dataFolderPath; }
            set
            {
                if (!string.Equals(value, _dataFolderPath))
                {
                    _dataFolderPath = value;
                    RaisePropertyChanged();
                }
            }
        }

        private DataPoint? _hoverPoint;
        public DataPointViewModel SelectedDataPoint { get; }

        public async Task UpdateSeriesAsync(CancellationToken cancellationToken)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;
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
            await foreach (DataPoint dataPoint in _dataPointBiz.GetDataPoints(
                SelectedCategory,
                StartDateRange == default ? null : StartDateRange.ToUniversalTime(),
                EndDateRange == default ? null : EndDateRange.ToUniversalTime(),
                cancellationToken).ConfigureAwait(false))
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

        private string FormatToolTip(TypedChartPoint<DataPoint, LineBezierVisualPoint<SkiaSharpDrawingContext, CircleGeometry, CubicBezierSegment, SKPath>, LabelGeometry, SkiaSharpDrawingContext> point)
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

        private string _upSyncStateText;

        public string UpSyncStateText
        {
            get { return _upSyncStateText; }
            set
            {
                if (!string.Equals(_upSyncStateText, value, StringComparison.Ordinal))
                {
                    _upSyncStateText = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _upSyncQueueLength;
        public int UpSyncQueueLength
        {
            get { return _upSyncQueueLength; }
            set
            {
                if (_upSyncQueueLength != value)
                {
                    _upSyncQueueLength = value;
                    RaisePropertyChanged();
                }
            }
        }

        private int _downSyncQueueLength;
        public int DownSyncQueueLength
        {
            get { return _downSyncQueueLength; }
            set
            {
                if (_downSyncQueueLength != value)
                {
                    _downSyncQueueLength = value;
                    RaisePropertyChanged();
                    UpdateSeriesAsync(default).ConfigureAwait(false);
                }
            }
        }

        private string _signInStatus;
        public string SignInStatus
        {
            get { return _signInStatus; }
            set
            {
                if (_signInStatus != value)
                {
                    _signInStatus = value;
                    RaisePropertyChanged();
                }
            }
        }


        public ICommand CancelSignInCommand { get; }
        private void CancelSignIn(object? parameter)
        {
            _syncService.CancelSignIn();
        }

        public ICommand SyncCommand { get; }
        private async Task SyncImpAsync(object? parameters)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;

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
                _errorRevealerFactory.CreateInstance(string.Empty).Reveal(result.Reason, "Sync error");
            }

            // Post sync
            InitializeCategoryCollection();
            await UpdateSeriesAsync(default).ConfigureAwait(false);
        }
        private void OnSyncImpException(Exception ex)
        {
            switch (ex)
            {
                case SigninTimeoutException:
                    MessageBoxResult choice = MessageBox.Show("User sign in timeout. This is a fatal error. " + Environment.NewLine +
                        $"Details: {ex.Message}" + Environment.NewLine +
                        Environment.NewLine +
                        "It is highly recommended to restart the application. Do you want to exit the current session?", "Fatal error", MessageBoxButton.YesNo, MessageBoxImage.Stop, MessageBoxResult.Yes);

                    if (choice == MessageBoxResult.Yes)
                    {
                        Application.Current.MainWindow.Close();
                    }
                    break;
                default:
                    _errorRevealerFactory.CreateInstance(string.Empty).Reveal(ex, $"Error invoking {nameof(SyncCommand)}");
                    break;
            }
        }

        public ICommand AddCategoryCommand { get; }
        private async Task AddCategoryImpAsync(object? parameter)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;
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
        private bool CanAddCategoryImp(object? parameter) => !string.IsNullOrEmpty(CategoryText);

        public ICommand TodayOnlyCommand { get; }
        private async Task TodayOnlyImpAsync(object? parameter)
        {
            StartDateRange = DateTime.Today;
            EndDateRange = DateTime.Today.AddDays(1);
            await UpdateSeriesAsync(default).ConfigureAwait(false);
        }

        public ICommand ResetZoomCommand { get; }
        private void ResetZoom(object? parameter)
        {
            foreach (Axis x in XAxes)
            {
                ResetZoom(x);
            }
        }

        public ICommand ExitCommand { get; }
        private void ExitImp(object? parameter)
        {
            MessageBoxResult userChoice = MessageBox.Show("Do you want to quit the application?", "Closing", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (userChoice == MessageBoxResult.No)
            {
                return;
            }
            Application.Current.MainWindow.Close();
        }

        private async Task RequestInitialSync()
        {
            if (_initialSyncRequested)
            {
                return;
            }

            _initialSyncRequested = true;
            SynchronizationContext uiThread = Dispatch<SynchronizationContext>(() =>
            {
                SynchronizationContext syncContext = SynchronizationContext.Current!;
                return syncContext;
            });

            if (await _internetAvailability.IsInternetAvailableAsync())
            {
                await uiThread;
                MessageBoxResult syncChoice = MessageBox.Show("You have internet access. Do you want to start a data synchronization immediately?", "Sync", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes);
                if (syncChoice == MessageBoxResult.No)
                {
                    return;
                }

                await SyncImpAsync(null).ConfigureAwait(false);
            }
            else
            {
                await uiThread;
                MessageBox.Show("There is no internet connection, skip initial syncing.", "No internet", MessageBoxButton.OK, MessageBoxImage.Information);
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

        private async Task ApplyNewDateRangeAsync(CancellationToken cancellationToken = default)
        {
            // Show all
            DateTime startAt = default;
            DateTime endAt = default;

            switch (SelectedDateRangeOption.Value)
            {
                case int.MinValue:
                    break;
                case 1:
                case 3:
                case 7:
                    (startAt, endAt) = _dateRangeService.GetLastNDays((uint)SelectedDateRangeOption.Value);
                    break;
                case 8:
                    (startAt, endAt) = _dateRangeService.GetThisWeek();
                    break;
                case 30:
                    (startAt, endAt) = _dateRangeService.GetThisMonth();
                    break;
                default:
                    _errorRevealerFactory.CreateInstance(string.Empty).Reveal("Unrecognized date time range. Will use the default range.", "Error");
                    SelectedDateRangeOption = DateRangeOptions.First();
                    return;
            }

            StartDateRange = startAt;
            EndDateRange = endAt;

            await UpdateSeriesAsync(default).ConfigureAwait(false);
        }

        private void BackgroundSyncProgress_ProgressChanged(object? sender, (string, int) args)
        {
            (string text, int queueLength) = args;
            Dispatch(() =>
            {
                UpSyncStateText = text;
                UpSyncQueueLength = queueLength;
                return 0;
            });
        }

        private void DownSyncProgress_ProgressChanged(object? sender, (string, int) args)
        {
            (string text, int queueLength) = args;
            Dispatch(() =>
            {
                UpSyncStateText = text;
                DownSyncQueueLength = queueLength;
                return 0;
            });
        }

        private void OnSignInStatusChanged(object? sender, OneDriveCredentialStatus newStatus)
        {
            // Get Sign in display text
            string newStateText = "Unknown";
            switch (newStatus)
            {
                case OneDriveCredentialStatus.Initial:
                    newStateText = "No sign in yet";
                    break;
                case OneDriveCredentialStatus.SigningIn:
                    newStateText = "Waiting signing in response ...";
                    break;
                case OneDriveCredentialStatus.SignedIn:
                    newStateText = "Signed in.";
                    break;
                case OneDriveCredentialStatus.Failed:
                    newStateText = "Sign in failed.";
                    break;
                case OneDriveCredentialStatus.Expired:
                    newStateText = "Sign in expired.";
                    break;
                default:
                    newStateText = $"Unrecognized sign in status: {newStatus}";
                    break;
            }

            // Update UI.
            Dispatch(() =>
            {
                SignInStatus = newStateText;
                return 0;
            });
        }
    }
}