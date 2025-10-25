namespace ExpenseTracker.API.Services
{
    public interface IUserContextService
    {
        public int Id { get; }
        public Guid UserId { get; }
        public string Email { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string FullName { get; }
    }
}
