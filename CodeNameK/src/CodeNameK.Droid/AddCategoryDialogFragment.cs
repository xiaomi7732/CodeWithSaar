
using Android.OS;
using AndroidX.AppCompat.App;
using AndroidX.Fragment.App;

namespace CodeNameK.Droid
{
    internal class AddCategoryDialogFragment : DialogFragment
    {
        public override Android.App.Dialog OnCreateDialog(Bundle savedInstanceState)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(Activity);
            builder.SetMessage("Hello message");
            builder.SetTitle(Resource.String.category_list_add_category_title);
            builder.SetPositiveButton(Resource.String.ok, (sender, e) => { 
            
            });

            return builder.Create();
        }
    }
}