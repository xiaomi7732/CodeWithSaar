using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CodeNameK.Biz;
using CodeNameK.DataContracts;
using Microsoft.Extensions.Logging;

namespace CodeNameK.ViewModels
{
    public class DataPointViewModel : ViewModelBase
    {
        private readonly IDataPoint _dataPointBiz;
        private readonly ErrorRevealer _errorRevealer;
        private readonly ILogger _logger;
        private DataPoint _model;

        public DataPointViewModel(
            IDataPoint dataPointBiz,
            ErrorRevealer errorRevealer,
            ILogger<DataPointViewModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataPointBiz = dataPointBiz ?? throw new ArgumentNullException(nameof(dataPointBiz));
            _errorRevealer = errorRevealer ?? throw new ArgumentNullException(nameof(errorRevealer));
            _model = new DataPoint();
            AddPointCommand = new RelayCommand(AddPoint);
            DeletePointCommand = new RelayCommand(DeletePoint);
        }

        public void SetModel(DataPoint newModel)
        {
            if (_model != newModel)
            {
                _model = newModel;
            }
            RaisePropertyChanged(nameof(WhenLocal));
            RaisePropertyChanged(nameof(TimeSpan));
            RaisePropertyChanged(nameof(Value));
        }

        public DateTime WhenLocal
        {
            get
            {
                return _model.WhenUTC.ToLocalTime();
            }
            set
            {
                DateTime utc = value.Add(TimeSpan).ToUniversalTime();
                if (_model.WhenUTC != utc)
                {
                    _model = _model with { WhenUTC = utc };
                    RaisePropertyChanged();
                }
            }
        }

        public TimeSpan TimeSpan
        {
            get
            {
                DateTime localDateTime = _model.WhenUTC.ToLocalTime();
                return localDateTime - localDateTime.Date;
            }
            set
            {
                if (TimeSpan != value)
                {
                    TimeSpan newTimeSpan = value;
                    _model = _model with
                    {
                        WhenUTC = WhenLocal.Add(value).ToUniversalTime(),
                    };
                }
            }
        }

        public double Value
        {
            get
            {
                return _model.Value;
            }
            set
            {
                if (_model.Value != value)
                {
                    _model = _model with { Value = value };
                    RaisePropertyChanged();
                }
            }
        }

        private bool _isCurrentDateTimeMode = true;
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

        public ICommand AddPointCommand { get; }
        private void AddPoint(object? parameter)
        {
            if (!(parameter is MainViewModel mainViewModel))
            {
                return;
            }

            Category? targetCategory = mainViewModel.SelectedCategory;
            DateTime whenUTC = IsCurrentDateTimeMode? DateTime.UtcNow : _model.WhenUTC;
            DataPoint newItem = _model with {
                Category = targetCategory,
                WhenUTC = whenUTC,
            };

            _logger.LogInformation("Adding a data point into category: {category}", newItem.Category?.Id);
            _ = Task.Run(async () =>
            {
                try
                {
                    OperationResult<DataPoint> addResult = await _dataPointBiz.AddAsync(newItem, default).ConfigureAwait(false);
                    if (addResult.IsSuccess)
                    {
                        await mainViewModel.UpdateSeriesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        _errorRevealer.Reveal(addResult.Reason, "Failed adding a data point");
                    }
                }
                catch (Exception ex)
                {
                    _errorRevealer.Reveal(ex.Message, "Unexpected exception.");
                }
            });
        }

        public ICommand DeletePointCommand { get; }
        private void DeletePoint(object? parameter)
        {
            if (!(parameter is MainViewModel mainViewModel))
            {
                _logger.LogWarning("Main view model is not specified.");
                return;
            }

            _logger.LogInformation("Deleting a data point by id {id}.", _model.Id);

            MessageBoxResult userChoice = MessageBox.Show($"Do you want to delete the data point: {_model.Value} at {_model.WhenUTC.ToLocalTime():f}?", "Delete", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (userChoice == MessageBoxResult.No)
            {
                return;
            }

            Task.Run(async () =>
            {
                try
                {
                    OperationResult<bool> result = await _dataPointBiz.DeleteAsync(_model, default).ConfigureAwait(false);
                    if (result.IsSuccess)
                    {
                        await mainViewModel.UpdateSeriesAsync().ConfigureAwait(false);
                    }
                    else
                    {
                        _errorRevealer.Reveal(result.Reason, "Delete data point failed.");
                    }
                }
                catch (Exception ex)
                {
                    _errorRevealer.Reveal(ex.Message, "Unexpected error");
                }
            });
        }
    }
}