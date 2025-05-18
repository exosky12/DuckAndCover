```plantuml
@startuml
skinparam shadowing true
skinparam class {
    BackgroundColor #D6EAF8
    BackgroundColor<<Interface>> #D5F5E3
    BackgroundColor<<enum>> #FFA500
    BackgroundColor<<event>> #FFD700
    BorderColor black
    ArrowColor black
    FontSize 14
    FontName "Arial"
}
skinparam namespaceSeparator none

namespace Models {

    class Game {
        + +/+ Players : List<Player>
        + +/+ Rules : IRules
        + +/+ CardsSkipped : int
        + +/+ CurrentPlayer : Player
        + +/+ Deck : Deck
        + +/+ Quit : bool
        + +/+ IsFinished : bool
        + +/+ CurrentDeckCard : DeckCard
        + +/+ LastNumber : int?
        + +/+ Id : string
        - _id : string
        - _currentPlayerIndex : int

        + <:zap>PlayerChanged : EventHandler<PlayerChangedEventArgs>
        + <:zap>GameIsOver : EventHandler<GameIsOverEventArgs>
        + <:zap>ErrorOccurred : EventHandler<ErrorOccurredEventArgs>
        + <:zap>PlayerChooseCoin : EventHandler<PlayerChooseCoinEventArgs>
        + <:zap>PlayerChooseDuck : EventHandler<PlayerChooseDuckEventArgs>
        + <:zap>PlayerChooseShowPlayersGrid : EventHandler<PlayerChooseShowPlayersGridEventArgs>
        + <:zap>PlayerChooseQuit : EventHandler<PlayerChooseQuitEventArgs>
        + <:zap>PlayerChooseCover : EventHandler<PlayerChooseCoverEventArgs>
        + <:zap>PlayerChooseShowScores : EventHandler<PlayerChooseShowScoresEventArgs>
        + <:zap>DisplayMenuNeeded : EventHandler<DisplayMenuNeededEventArgs>

        + Game(players : List<Player>)
        + Game(id : string, players : List<Player>, currentPlayerIndex : int, cardsSkipped : int, isFinished : bool)
        + NextPlayer() : void
        + GameLoop() : void
        + HandlePlayerChoice(player : Player, choice : string) : void
        + HandlePlayerChooseCover(player : Player, cardToMovePosition : Position, cardToCoverPosition : Position) : void
        + TriggerGameOver() : void
        + HandlePlayerChooseDuck(player : Player, cardToMovePosition : Position, duckPosition : Position) : void
        + CheckGameOverCondition() : bool
        + DoCover(player : Player, cardToMovePosition : Position, cardToCoverPosition : Position) : void
        + DoDuck(player : Player, cardToMovePosition : Position, duckPosition : Position) : void
        + DoCoin(player : Player) : void
        + NextDeckCard() : DeckCard
        + Save() : void
        + AllPlayersPlayed() : bool
        + CheckAllPlayersSkipped() : void
    }

    class Player {
        + +/% Name : string
        + +/+ HasSkipped : bool
        + +/+ HasPlayed : bool
        + +/+ Scores : List<int>
        + +/+ TotalScore : int
        + +/+ StackCounter : int
        + +/+ Grid : Grid
        - _id : Guid

        + Player(name : string)
        + Player(name : string, stack : int, scores : List<int>, skipped : bool, played : bool, grid : Grid)
        + HasCardWithNumber(number : int) : bool
    }

    class ClassicRules {
        + +/+ Name : string
        + +/+ Description : string
        + +/+ NbCardsInDeck : int
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard) : void
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    interface IRules <<interface>> {
        + Name : string
        + Description : string
        + NbCardsInDeck : int
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard) : void
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    ' Relations principales
    Game *-- Player
    Game --> ClassicRules
    ClassicRules ..|> IRules

}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
endlegend
@enduml