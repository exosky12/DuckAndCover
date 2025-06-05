using Models.Game;
using Models.Enums;
using Models.Rules;
using Models.Exceptions;

namespace UnitTests
{
    public class BaseRulesTests
    {
        private class TestableRules : BaseRules
        {
            public override string Name => "Test Rules";
            public override string Description => "Rules for testing";
            public override int NbCardsInDeck => 24;
            public override bool IsGameOver(int cardPassed, int stackCounter, bool quit) => false;
        }

        private readonly TestableRules _rules = new TestableRules();

        [Fact]
        public void IsTheSameCard_SameNumber_ReturnsTrue()
        {
            var gameCard = new GameCard(7, 5);
            var deckCard = new DeckCard(Bonus.None, 5);

            var result = _rules.isTheSameCard(gameCard, deckCard);

            Assert.True(result);
        }

        [Fact]
        public void IsTheSameCard_DifferentNumber_ReturnsFalse()
        {
            var gameCard = new GameCard(4, 1);
            var deckCard = new DeckCard(Bonus.None, 2);

            var result = _rules.isTheSameCard(gameCard, deckCard);

            Assert.False(result);
        }

        [Fact]
        public void IsTheSameCard_WithBonus_SameNumber_ReturnsTrue()
        {
            var gameCard = new GameCard(3, 8);
            var deckCard = new DeckCard(Bonus.Again, 8);

            var result = _rules.isTheSameCard(gameCard, deckCard);

            Assert.True(result);
        }

        [Fact]
        public void ValidateCardExists_CardExists_DoesNotThrow()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var position = new Position(1, 1);

            var exception = Record.Exception(() => accessibleRules.PublicValidateCardExists(grid, position));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateCardExists_CardDoesNotExist_ThrowsException()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var position = new Position(10, 10);

            var ex = Assert.Throws<ErrorException>(() => accessibleRules.PublicValidateCardExists(grid, position));
            Assert.Equal(ErrorCodes.CardNotFound, ex.ErrorCode);
        }

        private class AccessibleTestRules : TestableRules
        {
            public void PublicValidateCardExists(Grid grid, Position position) => 
                ValidateCardExists(grid, position);
            
            public void PublicValidateCardNumber(Grid grid, Position position, DeckCard deckCard) => 
                ValidateCardNumber(grid, position, deckCard);
            
            public void PublicValidateCoverMove(Position currentPos, Position newPos, Grid grid) => 
                ValidateCoverMove(currentPos, newPos, grid);
            
            public void PublicValidateDuckMove(Position currentPos, Position newPos, Grid grid) => 
                ValidateDuckMove(currentPos, newPos, grid);
        }

        [Fact]
        public void ValidateCardNumber_CorrectNumber_DoesNotThrow()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var card = grid.GetCard(new Position(1, 1))!;
            var deckCard = new DeckCard(Bonus.None, card.Number);

            var exception = Record.Exception(() => 
                accessibleRules.PublicValidateCardNumber(grid, card.Position, deckCard));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateCardNumber_IncorrectNumber_ThrowsException()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var card = grid.GetCard(new Position(1, 1))!;
            var deckCard = new DeckCard(Bonus.None, card.Number + 1);

            var ex = Assert.Throws<ErrorException>(() => 
                accessibleRules.PublicValidateCardNumber(grid, card.Position, deckCard));
            Assert.Equal(ErrorCodes.CardNumberNotEqualToDeckCardNumber, ex.ErrorCode);
        }

        [Fact]
        public void ValidateCoverMove_ValidMove_DoesNotThrow()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var currentPos = new Position(1, 1);
            var targetPos = new Position(1, 2);

            var exception = Record.Exception(() => 
                accessibleRules.PublicValidateCoverMove(currentPos, targetPos, grid));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateCoverMove_TargetEmpty_ThrowsException()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var currentPos = new Position(1, 1);
            var targetPos = new Position(5, 5);

            var ex = Assert.Throws<ErrorException>(() => 
                accessibleRules.PublicValidateCoverMove(currentPos, targetPos, grid));
            Assert.Equal(ErrorCodes.CardNotFound, ex.ErrorCode);
        }

        [Fact]
        public void ValidateCoverMove_NotAdjacent_ThrowsException()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var currentPos = new Position(1, 1);
            var targetPos = new Position(3, 3);

            var ex = Assert.Throws<ErrorException>(() => 
                accessibleRules.PublicValidateCoverMove(currentPos, targetPos, grid));
            Assert.Equal(ErrorCodes.CardsAreNotAdjacent, ex.ErrorCode);
        }

        [Fact]
        public void ValidateDuckMove_ValidMove_DoesNotThrow()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var currentPos = new Position(1, 1);
            var targetPos = new Position(1, 5);

            var exception = Record.Exception(() => 
                accessibleRules.PublicValidateDuckMove(currentPos, targetPos, grid));
            Assert.Null(exception);
        }

        [Fact]
        public void ValidateDuckMove_TargetOccupied_ThrowsException()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var currentPos = new Position(1, 1);
            var targetPos = new Position(1, 2);

            var ex = Assert.Throws<ErrorException>(() => 
                accessibleRules.PublicValidateDuckMove(currentPos, targetPos, grid));
            Assert.Equal(ErrorCodes.CardAlreadyExists, ex.ErrorCode);
        }

        [Fact]
        public void ValidateDuckMove_NotAdjacent_ThrowsException()
        {
            var accessibleRules = new AccessibleTestRules();
            var grid = new Grid();
            var currentPos = new Position(1, 1);
            var targetPos = new Position(10, 10);

            var ex = Assert.Throws<ErrorException>(() => 
                accessibleRules.PublicValidateDuckMove(currentPos, targetPos, grid));
            Assert.Equal(ErrorCodes.AdjacentCardNotFound, ex.ErrorCode);
        }

        [Fact]
        public void TryValidMove_InvalidFunctionName_ThrowsException()
        {
            var grid = new Grid();
            var card = grid.GetCard(new Position(1, 1))!;
            var deckCard = new DeckCard(Bonus.None, card.Number);

            var ex = Assert.Throws<ErrorException>(() => 
                _rules.TryValidMove(card.Position, new Position(1, 2), grid, "invalid", deckCard));
            Assert.Equal(ErrorCodes.InvalidFunctionName, ex.ErrorCode);
        }

        [Fact]
        public void TryValidMove_CardNotFound_ThrowsException()
        {
            var grid = new Grid();
            var emptyPosition = new Position(10, 10);
            var deckCard = new DeckCard(Bonus.None, 5);

            var ex = Assert.Throws<ErrorException>(() => 
                _rules.TryValidMove(emptyPosition, new Position(1, 2), grid, "cover", deckCard));
            Assert.Equal(ErrorCodes.CardNotFound, ex.ErrorCode);
        }

        [Fact]
        public void TryValidMove_CardNumberMismatch_ThrowsException()
        {
            var grid = new Grid();
            var card = grid.GetCard(new Position(1, 1))!;
            var wrongDeckCard = new DeckCard(Bonus.None, card.Number + 1);

            var ex = Assert.Throws<ErrorException>(() => 
                _rules.TryValidMove(card.Position, new Position(1, 2), grid, "cover", wrongDeckCard));
            Assert.Equal(ErrorCodes.CardNumberNotEqualToDeckCardNumber, ex.ErrorCode);
        }

        [Fact]
        public void TryValidMove_ValidCover_DoesNotThrow()
        {
            var grid = new Grid();
            var card = grid.GetCard(new Position(1, 1))!;
            var deckCard = new DeckCard(Bonus.None, card.Number);

            var exception = Record.Exception(() => 
                _rules.TryValidMove(card.Position, new Position(1, 2), grid, "cover", deckCard));
            Assert.Null(exception);
        }

        [Fact]
        public void TryValidMove_ValidDuck_DoesNotThrow()
        {
            var grid = new Grid();
            var card = grid.GetCard(new Position(1, 1))!;
            var deckCard = new DeckCard(Bonus.None, card.Number);

            var exception = Record.Exception(() => 
                _rules.TryValidMove(card.Position, new Position(1, 5), grid, "duck", deckCard));
            Assert.Null(exception);
        }
    }
} 