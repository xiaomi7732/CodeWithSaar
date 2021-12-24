namespace CodeNameK.ViewModels
{
    public interface IErrorRevealerFactory
    {
        IErrorRevealer CreateInstance(string? title = null);
    }
}