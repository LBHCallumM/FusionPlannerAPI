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
using System.Net;
using Column = FusionPlannerAPI.Infrastructure.Column;
using IndexOutOfRangeException = FusionPlannerAPI.Exceptions.IndexOutOfRangeException;

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
        public async Task ListColumns_WhenCardsAreArchived_OnlyIncludesActiveCards()
        {
            // Arrange
            var board = _fixture.Build<Board>().Create();

            InMemoryDb.Instance.Boards.Add(board);
            await InMemoryDb.Instance.SaveChangesAsync();

            var cards = _fixture.Build<Card>()
                .With(x => x.IsArchived, false)
                .CreateMany(2)
                .ToList();

            cards.First().IsArchived = true;

            var column = _fixture.Build<Column>()
                .With(c => c.BoardId, board.Id)
                .Without(x => x.Board)
                .With(x => x.Cards, cards)
                .Create();

            InMemoryDb.Instance.Columns.Add(column);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _columnGateway.ListColumns(board.Id);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.First().Cards.Should().HaveCount(cards.Count() -1);
        }

        [Test]
        public async Task ListColumns_WhenCalled_ReturnsColumnsAndCards()
        {
            // Arrange
            var board = _fixture.Build<Board>().Create();

            InMemoryDb.Instance.Boards.Add(board);
            await InMemoryDb.Instance.SaveChangesAsync();

            var cards = _fixture.Build<Card>()
                .With(x => x.IsArchived, false)
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

        [Test]
        public async Task MoveCard_ThrowsException_WhenCarddIsInvalid()
        {
            // Arrange
            var cardId = 1;
            var sourceListId = 1;

            var request = new MoveCardRequestObject
            {
                CardId = cardId,
                SourceColumnId = sourceListId,
                DestinationColumnId = sourceListId,
                DestinationCardIndex = 0
            };

            // Act
            Func<Task> act = async () => await _columnGateway.MoveCard(request);

            // Assert
            await act.Should().ThrowAsync<CardNotFoundException>();
        }

        [Test]
        public async Task MoveCard_ThrowsException_WhenSourceListIdIsInvalid()
        {
            // Arrange
            var sourceListId = 1;

            var card = _fixture.Build<Card>()
                .With(x => x.ColumnId, 2)
                .Create();

            InMemoryDb.Instance.Cards.Add(card);
            await InMemoryDb.Instance.SaveChangesAsync();

            var request = new MoveCardRequestObject
            {
                CardId = card.Id,
                SourceColumnId = sourceListId,
                DestinationColumnId = sourceListId,
                DestinationCardIndex = 0
            };

            // Act
            Func<Task> act = async () => await _columnGateway.MoveCard(request);

            // Assert
            await act.Should().ThrowAsync<ColumnNotFoundException>();
        }

        [Test]
        public async Task MoveCard_ThrowsException_WhenDestinationListIdIsInvalid()
        {
            // Arrange
            var sourceList = await SetupColumn(1, 5);
            var destinationListId = 2;

            var request = new MoveCardRequestObject
            {
                CardId = sourceList.Cards[0].Id,
                SourceColumnId = sourceList.Id,
                DestinationColumnId = destinationListId,
                DestinationCardIndex = 4
            };

            // Act
            Func<Task> act = async () => await _columnGateway.MoveCard(request);

            // Assert
            await act.Should().ThrowAsync<ColumnNotFoundException>();
        }

        [Test]
        public async Task MoveCard_WhenMovingTheCardToAHigherIndexInTheSameList_UpdatesTheDisplayOrder()
        {
            // Arrange
            var sourceList = await SetupColumn(1, 5);

            var request = new MoveCardRequestObject
            {
                CardId = sourceList.Cards[0].Id,
                SourceColumnId = sourceList.Id,
                DestinationColumnId = sourceList.Id,
                DestinationCardIndex = 4
            };

            // Act
            await _columnGateway.MoveCard(request);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == sourceList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var responseIds = dbResponse.Select(x => x.Id).ToList();

            var expectedOrderIds = new List<int> { 2, 3, 4, 5, 1 };

            responseIds.Should().BeEquivalentTo(expectedOrderIds, options => options.WithStrictOrdering());
        }

        [TestCase(-1, 1)]
        [TestCase(6, 1)]
        [TestCase(-1, 2)]
        [TestCase(6, 2)]
        public async Task MoveCard_WhenMovingTheCardToInvalidIndex_ThrowsArgumentException(int destinationIndex, int destinationColumnId)
        {
            // Arrange
            var sourceList = await SetupColumn(1, 5);
            var destinationList = await SetupColumn(2, 5, 5);

            var request = new MoveCardRequestObject
            {
                CardId = sourceList.Cards[0].Id,
                SourceColumnId = sourceList.Id,
                DestinationColumnId = destinationColumnId,
                DestinationCardIndex = destinationIndex
            };

            // Act
            Func<Task> act = async () => await _columnGateway.MoveCard(request);

            // Assert
            await act.Should().ThrowAsync<IndexOutOfRangeException>();
        }

        [Test]
        public async Task MoveCard_WhenMovingTheCardToLowerIndexInTheSameList_UpdatesTheDisplayOrder()
        {
            // Arrange
            var sourceList = await SetupColumn(1,5);

            var request = new MoveCardRequestObject
            {
                CardId = sourceList.Cards[4].Id,
                SourceColumnId = sourceList.Id,
                DestinationColumnId = sourceList.Id,
                DestinationCardIndex = 0
            };

            // Act
            await _columnGateway.MoveCard(request);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == sourceList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var responseIds = dbResponse.Select(x => x.Id).ToList();

            var expectedOrderIds = new List<int> { 5, 1, 2, 3, 4 };

            responseIds.Should().BeEquivalentTo(expectedOrderIds, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task MoveCard_WhenMovingTheCardToAHigherIndexInADifferentList_UpdatesTheDisplayOrder()
        {
            // Arrange
            var sourceList = await SetupColumn(1, 5);
            var destinationList = await SetupColumn(2, 5, 5);

            var request = new MoveCardRequestObject
            {
                CardId = sourceList.Cards[0].Id,
                SourceColumnId = sourceList.Id,
                DestinationColumnId = destinationList.Id,
                DestinationCardIndex = 5
            };

            // Act
            await _columnGateway.MoveCard(request);

            // Assert
            var dbResponseSourceList = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == sourceList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var dbResponseDestinationList = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == destinationList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var responseIdsSourceList = dbResponseSourceList.Select(x => x.Id).ToList();
            var responseIdsdestinatioList = dbResponseDestinationList.Select(x => x.Id).ToList();

            var sourceListExpectedOrderIds = new List<int> { 2, 3, 4, 5 };
            var destinationListExpectedOrderIds = new List<int> { 6, 7, 8, 9, 10, 1 };

            responseIdsSourceList.Should().BeEquivalentTo(sourceListExpectedOrderIds, options => options.WithStrictOrdering());
            responseIdsdestinatioList.Should().BeEquivalentTo(destinationListExpectedOrderIds, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task MoveCard_WhenMovingTheCardToLowerIndexInADifferentList_UpdatesTheDisplayOrder()
        {
            // Arrange
            var sourceList = await SetupColumn(1, 5);
            var destinationList = await SetupColumn(2, 5, 5);

            var request = new MoveCardRequestObject
            {
                CardId = sourceList.Cards[4].Id,
                SourceColumnId = sourceList.Id,
                DestinationColumnId = destinationList.Id,
                DestinationCardIndex = 0
            };

            // Act
            await _columnGateway.MoveCard(request);

            // Assert
            var dbResponseSourceList = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == sourceList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var dbResponseDestinationList = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == destinationList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var responseIdsSourceList = dbResponseSourceList.Select(x => x.Id).ToList();
            var responseIdsdestinatioList = dbResponseDestinationList.Select(x => x.Id).ToList();

            var sourceListExpectedOrderIds = new List<int> { 1, 2, 3, 4 };
            var destinationListExpectedOrderIds = new List<int> { 5, 6, 7, 8, 9, 10 };

            responseIdsSourceList.Should().BeEquivalentTo(sourceListExpectedOrderIds, options => options.WithStrictOrdering());
            responseIdsdestinatioList.Should().BeEquivalentTo(destinationListExpectedOrderIds, options => options.WithStrictOrdering());
        }

        [Test]
        public async Task MoveCard_WhenMovingTheCardToAnEmptyList_UpdatesTheDisplayOrder()
        {
            // Arrange
            var sourceList = await SetupColumn(1, 5);
            var destinationList = await SetupColumn(2, 0);

            var request = new MoveCardRequestObject
            {
                CardId = sourceList.Cards[0].Id,
                SourceColumnId = sourceList.Id,
                DestinationColumnId = destinationList.Id,
                DestinationCardIndex = 0
            };

            // Act
            await _columnGateway.MoveCard(request);

            // Assert
            var dbResponseSourceList = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == sourceList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var dbResponseDestinationList = await InMemoryDb.Instance.Cards
                .Where(x => x.ColumnId == destinationList.Id)
                .OrderBy(x => x.DisplayOrder)
                .ToListAsync();

            var responseIdsSourceList = dbResponseSourceList.Select(x => x.Id).ToList();
            var responseIdsdestinatioList = dbResponseDestinationList.Select(x => x.Id).ToList();

            var sourceListExpectedOrderIds = new List<int> { 2, 3, 4, 5 };
            var destinationListExpectedOrderIds = new List<int> { 1 };

            responseIdsSourceList.Should().BeEquivalentTo(sourceListExpectedOrderIds, options => options.WithStrictOrdering());
            responseIdsdestinatioList.Should().BeEquivalentTo(destinationListExpectedOrderIds, options => options.WithStrictOrdering());
        }

        private async Task<Column> SetupColumn(int columnId, int total, int idOffset = 0)
        {
            var column = _fixture.Build<Column>()
                            .With(x => x.Id, columnId)
                            .With(c => c.Cards, _fixture.Build<Card>()
                            .CreateMany(total).ToList())
                            .Create();

            for (int i = 0; i < column.Cards.Count; i++)
            {
                column.Cards[i].DisplayOrder = i + 1;
                column.Cards[i].Id = i + 1 + idOffset;
            }

            InMemoryDb.Instance.Columns.Add(column);
            await InMemoryDb.Instance.SaveChangesAsync();
            return column;
        }

    }
}
    

