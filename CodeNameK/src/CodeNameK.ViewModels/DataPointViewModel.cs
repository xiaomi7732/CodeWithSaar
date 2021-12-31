using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CodeNameK.BIZ.Interfaces;
using CodeNameK.Contracts;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;

namespace CodeNameK.ViewModels
{
    public class DataPointViewModel : ViewModelBase
    {
        private readonly IDataPoint _dataPointBiz;
        private readonly ILogger _logger;
        private Guid _id;

        /// <summary>
        /// View Model for a data point.
        /// </summary>
        public DataPointViewModel(
            IDataPoint dataPointBiz,
            IErrorRevealerFactory errorRevealer,
            ILogger<DataPointViewModel> logger)
            : base(errorRevealer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataPointBiz = dataPointBiz ?? throw new ArgumentNullException(nameof(dataPointBiz));
            LocalDate = DateTime.Today;

            AddPointCommand = new AsyncRelayCommand(AddPointAsync, exceptionCallback: _errorRevealerFactory.CreateInstance($"Unexpected error invoking {nameof(AddPointCommand)}").Reveal);
            DeletePointCommand = new AsyncRelayCommand(DeletePointAsync, CanDelete, exceptionCallback: _errorRevealerFactory.CreateInstance($"Unexpected error invoking {nameof(AsyncRelayCommand)}").Reveal);
        }

        /// <summary>
        /// Sets the values of the view model by a model
        /// </summary>
        /// <param name="newModel"></param>
        public void SetModel(DataPoint? newModel)
        {
            if (newModel is null)
            {
                IsCurrentDateTimeMode = true;
            }

            newModel ??= CreateDefaultModel();
            _id = newModel.Id;

            DateTime utcDateTime = newModel.WhenUTC;
            DateTime localDateTime = utcDateTime.ToLocalTime();

            LocalDate = localDateTime.Date;
            TimeSpan = localDateTime - LocalDate;
            Value = newModel.Value;
        }

        private DateTime _localDate;
        /// <summary>
        /// Gets or sets the local date - only the date potion;
        /// </summary>
        public DateTime LocalDate
        {
            get
            {
                return _localDate;
            }
            set
            {
                if (_localDate != value)
                {
                    _localDate = value;
                    RaisePropertyChanged();
                }
            }
        }

        private TimeSpan _timeSpan;
        /// <summary>
        /// Gets or sets the local time offset to the <see cref="LocalDate"/>.
        /// </summary>
        public TimeSpan TimeSpan
        {
            get
            {
                return _timeSpan;
            }
            set
            {
                if (_timeSpan != value)
                {
                    _timeSpan = value;
                    RaisePropertyChanged();
                }
            }
        }

        private double _value;
        /// <summary>
        /// Gets or sets the value for the data point
        /// </summary>
        public double Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (_value != value)
                {
                    _value = value;
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isCurrentDateTimeMode = true;
        /// <summary>
        /// Gets or sets the value to determine which value to use for creating a datapoint.
        /// </summary>
        public bool IsCurrentDateTimeMode
        {
            get { return _isCurrentDateTimeMode; }
            set
            {
                if (_isCurrentDateTimeMode != value)
                {
                    _isCurrentDateTimeMode = value;
                    RaisePropertyChanged();
                }
            }
        }

        /// <summary>
        /// Add a data point command.
        /// </summary>
        public ICommand AddPointCommand { get; }
        private async Task AddPointAsync(object? parameter)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;

            if (!(parameter is MainViewModel mainViewModel))
            {
                return;
            }

            Category? targetCategory = mainViewModel.SelectedCategory;
            if (targetCategory == null)
            {
                throw new InvalidOperationException("No selected category!");
            }
            DataPoint newItem = BuildDataPoint(targetCategory);

            _logger.LogInformation("Adding a data point into category: {category}", newItem.Category?.Id);
            OperationResult<DataPoint> addResult = await _dataPointBiz.AddAsync(newItem, default).ConfigureAwait(false);

            await syncContext;

            if (addResult.IsSuccess)
            {
                await mainViewModel.UpdateSeriesAsync(default).ConfigureAwait(false);
            }
            else
            {
                _errorRevealerFactory.CreateInstance(string.Empty).Reveal(addResult.Reason, "Failed adding a data point");
            }
        }

        /// <summary>
        /// Delete a datapoint command
        /// </summary>
        public ICommand DeletePointCommand { get; }
        private async Task DeletePointAsync(object? parameter)
        {
            SynchronizationContext syncContext = SynchronizationContext.Current!;
            if (!(parameter is MainViewModel mainViewModel))
            {
                _logger.LogWarning("Main view model is not specified.");
                return;
            }

            _logger.LogInformation("Deleting a data point by id {id}.", _id);
            MessageBoxResult userChoice = MessageBox.Show($"Do you want to delete the data point: {Value} at {BuildWhenUTC().ToLocalTime():f}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (userChoice == MessageBoxResult.No)
            {
                return;
            }

            OperationResult<bool> result = await _dataPointBiz.DeleteAsync(BuildDataPoint(mainViewModel.SelectedCategory!), default).ConfigureAwait(false);

            await syncContext;
            if (result.IsSuccess)
            {
                await mainViewModel.UpdateSeriesAsync(default).ConfigureAwait(false);
            }
            else
            {
                _errorRevealerFactory.CreateInstance(string.Empty).Reveal(result.Reason, "Delete data point failed.");
            }
        }
        private bool CanDelete(object? parameter)
        {
            if (parameter is MainViewModel mainViewModel)
            {
                return mainViewModel.SelectedDataPoint is not null && _id != Guid.Empty;
            }
            return false;
        }

        /// <summary>
        /// Creates a default datapoint model with current date time.
        /// </summary>
        private DataPoint CreateDefaultModel()
        {
            return new DataPoint()
            {
                WhenUTC = DateTime.UtcNow,
                Value = 0,
            };
        }

        /// <summary>
        /// Builds a data point by current view model
        /// </summary>
        private DataPoint BuildDataPoint(Category category)
        {
            if (category is null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            return new DataPoint()
            {
                Id = _id,
                Category = category,
                WhenUTC = IsCurrentDateTimeMode ? DateTime.UtcNow : BuildWhenUTC(),
                Value = this.Value,
            };
        }

        /// <summary>
        /// Builds Date Time in UTC by LocalDate + TimeSpan
        /// </summary>
        private DateTime BuildWhenUTC()
        {
            return LocalDate.Add(TimeSpan).ToUniversalTime();
        }
    }
}