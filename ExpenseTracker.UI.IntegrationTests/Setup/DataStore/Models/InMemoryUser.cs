namespace ExpenseTracker.IntegrationTests.Setup.DataStore.Models
{
    public class InMemoryUser
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;

        public InMemoryUser() { }

        public InMemoryUser(
                int id,
                Guid userId,
                string email,
                string firstName,
                string lastName,
                string fullName,
                string password
            )
        {
            Id = id;
            UserId = userId;
            Email = email;
            FirstName = firstName;
            LastName = lastName;
            FullName = fullName;
            Password = password;
        }
    }
}
