using System;

namespace CodeNameK.ViewModels
{
    public interface IErrorRevealer
    {
        void Reveal(Exception ex);
        void Reveal(Exception ex, string title);
        void Reveal(string message, string title);
    }
}