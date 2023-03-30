using AutoFixture;
using FluentAssertions;
using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Factories;
using FusionPlannerAPI.Gateways;
using FusionPlannerAPI.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Transactions;
using Xunit;

namespace FusionPlannerAPI.Tests.Gateways
{
    [TestFixture]
    public class BoardGatewayTests
    {
        private BoardGateway _boardGateway;
        private readonly Fixture _fixture = new Fixture();

        public BoardGatewayTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        }

        [SetUp]
        public void SetUp()
        {
            _boardGateway = new BoardGateway(InMemoryDb.Instance);
        }

        [TearDown]
        public void TearDown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CreateBoard_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<CreateBoardRequest>();
            var userId = 1;

            // Act
            Func<Task> act = async () => await _boardGateway.CreateBoard(request, userId);

            // Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Test]
        public async Task CreateBoard_WhenCalled_CreatesBoardAndId()
        {
            // Arrange
            var request = _fixture.Create<CreateBoardRequest>();
            var userId = 1;

            InMemoryDb.Instance.Users.Add(new User { Id = userId });
            await InMemoryDb.Instance.SaveChangesAsync();

            // Set up mock object to return a Board entity
            // Act
            var result = await _boardGateway.CreateBoard(request, userId);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Boards.FindAsync(result);
            dbResponse.Should().NotBeNull();
            dbResponse.Should().BeEquivalentTo(request.ToDatabase(userId).ToResponse(),
                options => options.Excluding(x => x.Id));
        }

        [Test]
        public async Task EditBoard_WhenBoardNotFound_ThrowsBoardNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<EditBoardRequest>();
            var boardId = 1;

            // Act
            Func<Task> act = async () => await _boardGateway.EditBoard(boardId, request);

            // Assert
            await act.Should().ThrowAsync<BoardNotFoundException>();
        }

        [Test]
        public async Task EditBoard_WhenCalled_UpdatesBoard()
        {
            // Arrange
            var board = new Board { Name = "Old name", Description = "Old description" };
            await InMemoryDb.Instance.Boards.AddAsync(board);
            await InMemoryDb.Instance.SaveChangesAsync();

            var request = _fixture.Create<EditBoardRequest>();

            // Act
            await _boardGateway.EditBoard(board.Id, request);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Boards.FindAsync(board.Id);
            dbResponse.Name.Should().Be(request.Name);
            dbResponse.Description.Should().Be(request.Description);
        }

        [Test]
        public async Task DeleteBoard_WhenBoardNotFound_ThrowsBoardNotFoundException()
        {
            // Arrange
            var boardId = 1;

            // Act
            Func<Task> act = async () => await _boardGateway.DeleteBoard(boardId);

            // Assert
            await act.Should().ThrowAsync<BoardNotFoundException>();
        }

        [Test]
        public async Task DeleteBoard_WhenCalled_DeletesBoard()
        {
            // Arrange
            var board = _fixture.Create<Board>();
            await InMemoryDb.Instance.Boards.AddAsync(board);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            await _boardGateway.DeleteBoard(board.Id);

            // Assert
            var deletedBoard = await InMemoryDb.Instance.Boards.FindAsync(board.Id);
            deletedBoard.Should().BeNull();
        }

        [Test]
        public async Task ListBoards_WhenCalled_ReturnsBoards()
        {
            // Arrange
            var boards = _fixture.Build<Board>().CreateMany(2);

            InMemoryDb.Instance.Boards.AddRange(boards);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _boardGateway.ListBoards();

            // Assert
            result.Should().HaveSameCount(boards);
            result.Should().BeEquivalentTo(boards.ToResponse(), options => options.Excluding(b => b.Id));
        }

        [Test]
        public async Task GetById_IfBoardNotFound_ReturnsNull()
        {
            // Arrange
            var boardId = 1;

            // Act
            var result = await _boardGateway.GetById(boardId);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetById_WhenBoardFound_ReturnsBoard()
        {
            // Arrange
            var board = _fixture.Create<Board>();

            await InMemoryDb.Instance.Boards.AddAsync(board);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _boardGateway.GetById(board.Id);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(board.Name);
            result.Description.Should().Be(board.Description);
        }

    }
}
