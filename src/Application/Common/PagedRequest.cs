namespace Application.Common;

public sealed record PagedRequest(int Page = 1, int PageSize = 10)
{
    public int Skip => (Page - 1) * PageSize;
}
