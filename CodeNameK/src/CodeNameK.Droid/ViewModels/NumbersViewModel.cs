#nullable enable
using Android.App;
using AndroidX.Lifecycle;
using System;
namespace CodeNameK.Droid.ViewModels
{
    internal class NumbersViewModel : AndroidViewModel
    {
        private readonly App _application;
        
        public NumbersViewModel(Application application) : base(application)
        {
            if (application is not App app)
            {
                throw new InvalidCastException("Can't cast Application instance to App instance.");
            }
            _application = app;
            CategoryName = new MutableLiveData(string.Empty);
        }

        public MutableLiveData CategoryName { get; } = new MutableLiveData();
        public MutableLiveData DataPoints { get; } = new MutableLiveData();
    }
}