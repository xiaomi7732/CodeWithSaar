using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;

namespace CodeNameK.Droid
{
    internal class AddCategoryDialogFragment : DialogFragment
    {
        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Context);
            builder.SetMessage("Hello message");
            builder.SetTitle("Dialog in Fragment");
            builder.SetPositiveButton(Resource.String.ok, (sender, e) => { });

            return builder.Create();
        }
    }
}