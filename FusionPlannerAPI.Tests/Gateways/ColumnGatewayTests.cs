using AutoFixture;
using FluentAssertions;
using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Gateways;
using FusionPlannerAPI.Gateways.Interfaces;
using FusionPlannerAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Column = FusionPlannerAPI.Infrastructure.Column;

namespace FusionPlannerAPI.Tests.Gateways
{
    [TestFixture]
    public class ColumnGatewayTests
    {
        private ColumnGateway _columnGateway;
        private readonly Fixture _fixture = new Fixture();

        [SetUp]
        public void Setup()
        {
            _columnGateway = new ColumnGateway(InMemoryDb.Instance);
        }

        public ColumnGatewayTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));

            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

            _fixture.Customizations.Add(new IgnoreVirtualMembersSpecimenBuilder());

        }

        [TearDown]
        public void TearDown()
        {
            InMemoryDb.Teardown();
        }


        [Test]
        public async Task GetById_WhenNotFound_ReturnsNull()
        {
            // Arrange
            var id = _fixture.Create<int>();

            // Act
            var result = await _columnGateway.GetById(id);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetById_WhenFound_ReturnsColumn()
        {
            // Arrange
            var column = _fixture.Build<Column>().Create();

            await InMemoryDb.Instance.Columns.AddAsync(column);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _columnGateway.GetById(column.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(column.Id);
            result.Name.Should().Be(column.Name);
        }

        [Test]
        public async Task ListColumns_WhenBoardNotFound_ThrowsBoardNotFoundException()
        {
            // Arrange
            int nonExistingBoardId = _fixture.Create<int>();

            // Act
            Func<Task<IEnumerable<ColumnResponse>>> act = async () => await _columnGateway.ListColumns(nonExistingBoardId);

            // Assert
            await act.Should().ThrowAsync<BoardNotFoundException>();
        }

        [Test]
        public async Task ListColumns_WhenCalled_ReturnsColumnsAndCards()
        {
            // Arrange
            var board = _fixture.Build<Board>().Create();

            InMemoryDb.Instance.Boards.Add(board);
            await InMemoryDb.Instance.SaveChangesAsync();

            var cards = _fixture.Build<Card>()
                .CreateMany(2)
                .ToList();

            var column = _fixture.Build<Column>()
                .With(c => c.BoardId, board.Id)
                .Without(x => x.Board)
                //.Without(x => x.Cards)
                .With(x => x.Cards, cards)
                .Create();

            InMemoryDb.Instance.Columns.Add(column);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _columnGateway.ListColumns(board.Id);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().ContainSingle(c => c.Id == column.Id);
            result.First().Cards.Should().NotBeNullOrEmpty();
            result.First().Cards.Should().HaveCount(cards.Count());
        }

        [Test]
        public async Task CreateColumn_WhenBoardNotFound_ThrowsBoardNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<CreateColumnRequest>();

            // Act
            Func<Task<int>> act = async () => await _columnGateway.CreateColumn(request);

            // Assert
            await act.Should().ThrowAsync<BoardNotFoundException>();
        }

        [Test]
        public async Task CreateColumn_WhenCalled_CreatesColumnAndReturnsId()
        {
            // Arrange
            var board = _fixture.Build<Board>().Create();

            InMemoryDb.Instance.Boards.Add(board);
            await InMemoryDb.Instance.SaveChangesAsync();

            var request = _fixture.Build<CreateColumnRequest>()
                .With(r => r.BoardId, board.Id)
                .Create();

            // Act
            int result = await _columnGateway.CreateColumn(request);

            // Assert
            result.Should().BeGreaterThan(0);

            var dbResponse = await InMemoryDb.Instance.Columns.FindAsync(result);
            dbResponse.Should().NotBeNull();
            dbResponse.BoardId.Should().Be(request.BoardId);
            dbResponse.Name.Should().Be(request.Name);
        }

        [Test]
        public async Task EditColumn_WhenColumnNotFound_ThrowsColumnNotFoundException()
        {
            // Arrange
            int nonExistingId = _fixture.Create<int>();
            var request = _fixture.Create<EditColumnRequest>();

            // Act
            Func<Task> act = async () => await _columnGateway.EditColumn(nonExistingId, request);

            // Assert
            await act.Should().ThrowAsync<ColumnNotFoundException>();
        }

        [Test]
        public async Task EditColumn_WhenCalled_SavesChangesToDatabase()
        {
            // Arrange
            var column = _fixture.Build<Column>().Create();

            await InMemoryDb.Instance.Columns.AddAsync(column);
            await InMemoryDb.Instance.SaveChangesAsync();

            var request = _fixture.Create<EditColumnRequest>();

            // Act
            await _columnGateway.EditColumn(column.Id, request);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Columns.FindAsync(column.Id);
            dbResponse.Name.Should().Be(request.Name);
        }

        [Test]
        public async Task DeleteColumn_WhenColumnNotFound_ThrowsColumnNotFoundException()
        {
            // Arrange
            int nonExistingId = _fixture.Create<int>();

            // Act
            Func<Task> act = async () => await _columnGateway.DeleteColumn(nonExistingId);

            // Assert
            await act.Should().ThrowAsync<ColumnNotFoundException>();
        }

        [Test]
        public async Task DeleteColumn_WhenCalled_DeletesColumnFromDatabase()
        {
            // Arrange
            var column = _fixture.Build<Column>().Create();

            await InMemoryDb.Instance.Columns.AddAsync(column);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            await _columnGateway.DeleteColumn(column.Id);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Columns.FindAsync(column.Id);
            dbResponse.Should().BeNull();
        }
    }
}
    

