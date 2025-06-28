using Person.Domain.ValueObjects;

namespace Person.Domain.Entities
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
