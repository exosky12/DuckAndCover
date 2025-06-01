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

namespace Models {

    interface "IRules" as IRules <<interface>> {
        + Name : string
        + Description : string
        + NbCardsInDeck : int
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class Game {
        + Rules : IRules
        + Game(rules : IRules)
        + HandlePlayerChoice(player : Player, choice : string)
        + HandlePlayerChooseCover(player : Player, cardToMovePosition : Position, cardToCoverPosition : Position)
        + HandlePlayerChooseDuck(player : Player, cardToMovePosition : Position, duckPosition : Position)
        + CheckGameOverCondition() : bool
    }

    class ClassicRules {
        + ClassicRules()
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class BlitzRules {
        + BlitzRules()
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class InsaneRules {
        + InsaneRules()
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    ' Relations
    ClassicRules ..|> IRules
    BlitzRules ..|> IRules
    InsaneRules ..|> IRules

    Game o-- IRules : utilise
}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#D5F5E3>| Interface |
endlegend
@enduml 