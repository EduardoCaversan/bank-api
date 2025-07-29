namespace BankApp.Domain.DTOs;

public class PaginatedResponse<T>
{
    public IEnumerable<T> Data { get; set; } = [];
    public PaginationMetadata Pagination { get; set; } = new();
}

public class PaginationMetadata
{
    public int ItemsPerPage { get; set; }
    public int CurrentPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }
}