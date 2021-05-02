namespace JWT.Example.WithSQLDB
{
    public abstract class UserDBBase
    {
        protected UserDBContext UserDBContext { get; }

        public UserDBBase(UserDBContext userDBContext)
        {
            UserDBContext = userDBContext ?? throw new System.ArgumentNullException(nameof(userDBContext));
        }
    }
}