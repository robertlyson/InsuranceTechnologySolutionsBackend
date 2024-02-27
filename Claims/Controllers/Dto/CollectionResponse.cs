namespace Claims.Controllers.Dto;

public record CollectionResponse<T>(IEnumerable<T> Items);