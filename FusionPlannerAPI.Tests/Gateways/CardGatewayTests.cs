using AutoFixture;
using FluentAssertions;
using FusionPlannerAPI.Boundary.Request;
using FusionPlannerAPI.Boundary.Response;
using FusionPlannerAPI.Exceptions;
using FusionPlannerAPI.Gateways;
using FusionPlannerAPI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace FusionPlannerAPI.Tests.Gateways
{
    [TestFixture]
    public class CardGatewayTests
    {
        private CardGateway _cardGateway;
        private readonly Fixture _fixture = new Fixture();

        public CardGatewayTests()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _fixture.Customizations.Add(new IgnoreVirtualMembersSpecimenBuilder());
        }

        [SetUp]
        public void SetUp()
        {
            _cardGateway = new CardGateway(InMemoryDb.Instance);
        }

        [TearDown]
        public void TearDown()
        {
            InMemoryDb.Teardown();
        }

        [Test]
        public async Task CreateCard_WhenColumnNotFound_ThrowsColumnNotFoundException()
        {
            // Arrange
            var request = _fixture.Create<CreateCardRequest>();

            // Act
            Func<Task<int>> act = async () => await _cardGateway.CreateCard(request, 1);

            // Assert
            await act.Should().ThrowAsync<ColumnNotFoundException>();
        }

        [Test]
        public async Task CreateCard_WhenUserNotFound_ThrowsUserNotFoundException()
        {
            // Arrange
            var column = _fixture.Build<Column>() .Create();

            InMemoryDb.Instance.Columns.Add(column);
            await InMemoryDb.Instance.SaveChangesAsync();

            var request = _fixture.Build<CreateCardRequest>()
                .With(c => c.ColumnId, column.Id)
                .Create();

            // Act
            Func<Task<int>> act = async () => await _cardGateway.CreateCard(request, -1);

            // Assert
            await act.Should().ThrowAsync<UserNotFoundException>();
        }

        [Test]
        public async Task CreateCard_WhenCalled_CreatesCardAndReturnsId()
        {
            // Arrange
            var column = _fixture.Build<Column>().Create();
            var user = _fixture.Build<User>().Create();

            InMemoryDb.Instance.Columns.Add(column);
            InMemoryDb.Instance.Users.Add(user);

            await InMemoryDb.Instance.SaveChangesAsync();

            var request = _fixture.Build<CreateCardRequest>()
                .With(c => c.ColumnId, column.Id)
                .Create();

            // Act
            var result = await _cardGateway.CreateCard(request, user.Id);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Cards.FindAsync(result);
            dbResponse.Should().NotBeNull();
            dbResponse.ColumnId.Should().Be(request.ColumnId);
            dbResponse.CreatedById.Should().Be(user.Id);
            dbResponse.Name.Should().Be(request.Name);
        }

        [Test]
        public async Task DeleteCard_WhenNotFound_ThrowsCardNotFoundException()
        {
            // Arrange
            int nonExistingId = _fixture.Create<int>();

            // Act
            Func<Task> act = async () => await _cardGateway.DeleteCard(nonExistingId);

            // Assert
            await act.Should().ThrowAsync<CardNotFoundException>();
        }

        [Test]
        public async Task DeleteCard_WhenCalled_RemovesCardFromDatabase()
        {
            // Arrange
            var card = _fixture.Build<Card>().Create();

            await InMemoryDb.Instance.Cards.AddAsync(card);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            await _cardGateway.DeleteCard(card.Id);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Cards.FindAsync(card.Id);
            dbResponse.Should().BeNull();
        }

        [Test]
        public async Task EditCard_WhenNotFound_ThrowsCardNotFoundException()
        {
            // Arrange
            int nonExistingId = _fixture.Create<int>();
            var request = _fixture.Create<EditCardRequest>();

            // Act
            Func <Task> act = async () => await _cardGateway.EditCard(nonExistingId, request);

            // Assert
            await act.Should().ThrowAsync<CardNotFoundException>();
        }

        [Test]
        public async Task EditCard_WhenCalled_SavesChangesToDatabase()
        {
            // Arrange
            var card = _fixture.Build<Card>().Create();

            await InMemoryDb.Instance.Cards.AddAsync(card);
            await InMemoryDb.Instance.SaveChangesAsync();

            var request = _fixture.Create<EditCardRequest>();

            // Act
            await _cardGateway.EditCard(card.Id, request);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Cards.FindAsync(card.Id);
            dbResponse.Name.Should().Be(request.Name);
            dbResponse.Description.Should().Be(request.Description);

            dbResponse.LastEditedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        }

        [Test]
        public async Task GetById_WhenNotFound_ReturnsNull()
        {
            // Arrange
            int nonExistingId = _fixture.Create<int>();

            // Act
            var result = await _cardGateway.GetById(nonExistingId);

            // Assert
            result.Should().BeNull();
        }

        [Test]
        public async Task GetById_WhenFound_ReturnsCard()
        {
            // Arrange
            var card = _fixture.Build<Card>().Create();

            await InMemoryDb.Instance.Cards.AddAsync(card);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            var result = await _cardGateway.GetById(card.Id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(card.Id);
            result.Name.Should().Be(card.Name);
            result.Description.Should().Be(card.Description);
        }

        [Test]
        public async Task ArchiveCard_WhenTrue_SetsCardToArchived()
        {
            // Arrange
            var card = _fixture.Build<Card>()
                .With(x => x.IsArchived, false)
                .Create();

            await InMemoryDb.Instance.Cards.AddAsync(card);
            await InMemoryDb.Instance.SaveChangesAsync();

            // Act
            await _cardGateway.ArchiveCard(card.Id);

            // Assert
            var dbResponse = await InMemoryDb.Instance.Cards.FindAsync(card.Id);

            dbResponse.Should().NotBeNull();
            dbResponse.IsArchived.Should().BeTrue();
        }
    }
}
