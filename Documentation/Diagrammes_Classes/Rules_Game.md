@startuml
skinparam shadowing true
skinparam class {
    BackgroundColor #D6EAF8
    BackgroundColor<<Interface>> #D5F5E3
    BorderColor black
    ArrowColor black
    FontSize 14
    FontName "Arial"
}
skinparam namespaceSeparator none

namespace Model {

    interface IRules <<Interface>> {
        + +/+ Name : string
        + +/+ Description : string
        + +/+ NbCardsInDeck : int
        + IsValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string) : bool
        + IsGameOver(cardPassed : int, stackCounter : int) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class ClassicRules {
        + +/+ Name : string
        + +/+ Description : string
        + +/+ NbCardsInDeck : int
        + IsValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string) : bool
        + IsGameOver(cardPassed : int, stackCounter : int) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class Game {
        - -/- Players : List<Player>
        + +/+ Rules : IRules
        + +/+ PlayerCount : int
        + +/+ CardPassed : int
        + +/+ CurrentPlayer : Player
        + +/+ Deck : Deck
        + +/+ CurrentPlayerIndex : int
        + +/+ LastNumber : int?
        + +/+ <:zap:> OnGameOver : Action?
        + +/+ <:zap:> OnPlayerChanged : Action<Player>?
        + Game(players : List<Player>)
        + CheckGameOverCondition() : void
        + NotifyPlayerChanged() : void
        + NextPlayer() : void
        + Save() : void
    }

    ' Relations
    ClassicRules ..|> IRules
    Game "1" *-- "1" IRules
    Game "1" *-- "1" Deck
}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#D5F5E3>| Interface |
    |<:zap:>| Événement |
endlegend
@enduml
