namespace CodeNameK.Droid
{
    /// <summary>
    /// Listener for adding category dialog buttons clicks.
    /// </summary>
    internal interface IAddCategoryDialogEventListener
    {
        void OnOKClicked(string category);
        void OnCancelClicked(string category);
    }
}