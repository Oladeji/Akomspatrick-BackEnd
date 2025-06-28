namespace Person.Domain.Entities
{
    public class PersonType
    {
        public int PersonTypeId { get; set; }
        public string Description { get; set; } = string.Empty;

        // Navigation property
       // public ICollection<Person>? Persons { get; set; }
    }
}
