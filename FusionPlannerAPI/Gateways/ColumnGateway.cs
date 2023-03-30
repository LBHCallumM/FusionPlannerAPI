using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Factories;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using FusionPlannerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.Linq;

namespace FusionPlannerAPI.Gateways
{
    public class ColumnGateway : IColumnGateway
    {
        private readonly PlannerDbContext _dbContext;

        public ColumnGateway(PlannerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ColumnResponse> GetById(int columnId)
        {
            var column = await _dbContext.Columns.FindAsync(columnId);
            if (column == null) return null;

            return column.ToResponse();
        }

        public async Task<IEnumerable<ColumnResponse>> ListColumns(int boardId)
        {
            var board = await _dbContext.Boards.FindAsync(boardId);
            if (board == null) throw new BoardNotFoundException(boardId);

            var columns = await _dbContext.Columns
                .Where(x => x.BoardId == boardId)
                .Include(x => x.Cards)
                .ToListAsync();

            return columns.ToResponse();
        }

        public async Task<int> CreateColumn(CreateColumnRequest request)
        {
            var board = await _dbContext.Boards.FindAsync(request.BoardId);
            if (board == null) throw new BoardNotFoundException(request.BoardId);

            var column = request.ToDatabase();

            _dbContext.Columns.Add(column);
            await _dbContext.SaveChangesAsync();

            return column.Id;
        }

        public async Task EditColumn(int columnId, EditColumnRequest request)
        {
            var column = await _dbContext.Columns.FindAsync(columnId);
            if (column == null) throw new ColumnNotFoundException(columnId);

            column.Name = request.Name;

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteColumn(int columnId)
        {
            var column = await _dbContext.Columns.FindAsync(columnId);
            if (column == null) throw new ColumnNotFoundException(columnId);

            // ToDo - Check for cards

            _dbContext.Columns.Remove(column);
            await _dbContext.SaveChangesAsync();
        }
    }
}
