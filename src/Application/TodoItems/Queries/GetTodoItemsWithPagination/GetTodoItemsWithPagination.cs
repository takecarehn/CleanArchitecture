using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Application.Common.Models;
using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsWithPagination;

public record GetTodoItemsWithPaginationQuery : IRequest<PaginatedList<TodoItemBriefDto>>
{
    public int ListId { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public class GetTodoItemsWithPaginationQueryHandler : IRequestHandler<GetTodoItemsWithPaginationQuery, PaginatedList<TodoItemBriefDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _cache;

    public GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context, IMapper mapper, IMemoryCache cache)
    {
        _cache = cache;
        _context = context;
        _mapper = mapper;
    }

    public async Task<PaginatedList<TodoItemBriefDto>> Handle(GetTodoItemsWithPaginationQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "TodoItemPagedList";

        // Kiểm tra dữ liệu trong cache
        if (!_cache.TryGetValue(cacheKey, out PaginatedList<TodoItemBriefDto>? todoItemPagedList))
        {
            // Nếu không có trong cache, lấy dữ liệu từ nguồn gốc
            todoItemPagedList = await _context.TodoItems
                                              .Where(x => x.ListId == request.ListId)
                                              .OrderBy(x => x.Title)
                                              .ProjectTo<TodoItemBriefDto>(_mapper.ConfigurationProvider)
                                              .PaginatedListAsync(request.PageNumber, request.PageSize, cancellationToken);

            // Lưu dữ liệu vào cache
            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            };
            _cache.Set(cacheKey, todoItemPagedList, cacheEntryOptions);
        }

        return todoItemPagedList ?? new PaginatedList<TodoItemBriefDto>(Array.Empty<TodoItemBriefDto>(), 0, request.PageNumber, request.PageSize);
    }
}
