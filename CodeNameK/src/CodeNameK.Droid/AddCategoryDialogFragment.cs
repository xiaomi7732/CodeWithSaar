#nullable enable

using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;
using System;

namespace CodeNameK.Droid
{
    internal class AddCategoryDialogFragment : DialogFragment
    {
        private TextView? _newCategoryNameTextView;
        private IAddCategoryDialogEventListener? _listener;
        // Making sure ctor is there.
        public AddCategoryDialogFragment()
        {
        }

        public override void OnAttach(Context context)
        {
            base.OnAttach(context);

            try
            {
                _listener = (IAddCategoryDialogEventListener)context;
            }
            catch (InvalidCastException)
            {
                throw new InvalidCastException(Activity.ToString() + "  must implement " + nameof(IAddCategoryDialogEventListener));
            }
        }

        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            LayoutInflater inflator = Activity.LayoutInflater;

            View dialogView = inflator.Inflate(Resource.Layout.dialog_content_add_category, null) ?? throw new InvalidCastException("Failed inflate adding category dialog");
            _newCategoryNameTextView = dialogView.FindViewById<TextView>(Resource.Id.new_category_name);
            builder.SetView(dialogView);
            builder.SetTitle(Resource.String.category_list_add_category_title);
            builder.SetPositiveButton(Resource.String.ok, (sender, e) =>
            {
                _listener?.OnOKClicked(_newCategoryNameTextView?.Text);
            });
            builder.SetNegativeButton(Resource.String.cancel, (sender, e) =>
            {
                _listener?.OnCancelClicked(_newCategoryNameTextView?.Text);
            });

            return builder.Create();
        }
    }
}