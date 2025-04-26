using System.Data;
using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Domain.Events;

namespace CleanArchitecture.Application.TodoItems.Commands.DeleteTodoItem;

public record DeleteTodoItemCommand(int Id) : IRequest;

public class DeleteTodoItemCommandHandler : IRequestHandler<DeleteTodoItemCommand>
{
    private readonly IApplicationDbContext _context;
    private readonly IDapperService _dapperService;

    public DeleteTodoItemCommandHandler(IApplicationDbContext context, IDapperService dapperService)
    {
        _context = context;
        _dapperService = dapperService;
    }

    public async Task Handle(DeleteTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.TodoItems
            .FindAsync(new object[] { request.Id }, cancellationToken);

        Guard.Against.NotFound(request.Id, entity);

        //_context.TodoItems.Remove(entity);
        await _dapperService.ExecuteAsync("DELETE FROM TodoItems WHERE Id = @Id", new { Id = request.Id }, CommandType.Text);

        entity.AddDomainEvent(new TodoItemDeletedEvent(entity));

        //await _context.SaveChangesAsync(cancellationToken);
    }
}
