namespace Person.Api.Models
{
    public class Person
    {
        public int PersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Age { get; set; }
        public int PersonTypeId { get; set; }
        public PersonType? PersonType { get; set; }
    }
}
