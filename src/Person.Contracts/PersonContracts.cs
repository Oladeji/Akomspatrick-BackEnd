namespace Person.Contracts;

// Request DTOs
public record CreatePersonRequest(
    string Name,
    int Age,
    int PersonTypeId
);
public record UpdatePersonRequest(
    string Name,
    int Age,
    int PersonTypeId
);

// Response DTOs
public record PersonResponse(
    int PersonId,
    string Name,
    int Age,
    int PersonTypeId,
    PersonTypeResponse? PersonType
);

public record PersonTypeResponse(
    int PersonTypeId,
    string TypeName
);
