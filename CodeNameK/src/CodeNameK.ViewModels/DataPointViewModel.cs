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
        private readonly ILogger _logger;
        private DataPoint _model;

        public DataPointViewModel(
            IDataPoint dataPointBiz,
            ILogger<DataPointViewModel> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _dataPointBiz = dataPointBiz ?? throw new ArgumentNullException(nameof(dataPointBiz));
            _model = new DataPoint();
            AddPointCommand = new RelayCommand(AddPoint);
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

        public ICommand AddPointCommand { get; }
        private void AddPoint(object? parameter)
        {
            if (parameter is MainViewModel mainViewModel)
            {
                _logger.LogInformation("Adding a data point into category: {category}", mainViewModel.SelectedCategory?.Id);
                _ = Task.Run(async () =>
                {
                    try
                    {
                        OperationResult<DataPoint> addResult = await _dataPointBiz.AddAsync(_model, default).ConfigureAwait(false);
                        if (addResult.IsSuccess)
                        {
                            await mainViewModel.UpdateSeriesAsync().ConfigureAwait(false);
                        }
                        else
                        {
                            Dispatch<int>(() =>
                            {
                                MessageBox.Show(addResult.Reason, "Failed adding a data point", MessageBoxButton.OK, MessageBoxImage.Error);
                                return int.MinValue;
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Dispatch<int>(() =>
                        {
                            MessageBox.Show(ex.Message, "Unexpected exception.", MessageBoxButton.OK, MessageBoxImage.Stop);
                            return int.MinValue;
                        });
                    }
                });
            }
        }
    }
}