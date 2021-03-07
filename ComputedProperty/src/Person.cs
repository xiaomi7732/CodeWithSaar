namespace ComputedProperty
{
    public class Person
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // Computed Property: Full name always consistent with First / Last name
        public string FullName => $"{FirstName} {LastName}";
    }
}
