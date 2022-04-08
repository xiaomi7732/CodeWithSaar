using Android.Content;
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using System;

namespace CodeNameK.Droid
{
    public interface IAddCategoryDialogListener
    {
        void OnOKClicked();
    }

    internal class AddCategoryDialogFragment : DialogFragment
    {
        IAddCategoryDialogListener _listener;
        public AddCategoryDialogFragment()
        {
            // Making sure a default ctor exists.
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetMessage("Hello message");
            builder.SetTitle("Dialog in Fragment");
            builder.SetPositiveButton(Resource.String.ok, (sender, e) => {
                _listener?.OnOKClicked();
            });

            return builder.Create();
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);
            if (context is IAddCategoryDialogListener listener)
            {
                _listener = listener;
            }
        }

        public override void OnDetach()
        {
            _listener = null;
            base.OnDetach();
        }
    }
}