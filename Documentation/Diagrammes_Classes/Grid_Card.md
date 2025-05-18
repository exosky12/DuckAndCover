```plantuml
@startuml
skinparam shadowing true
skinparam class {
    BackgroundColor #D6EAF8
    BackgroundColor<<enum>> #FFA500
    BorderColor black
    ArrowColor black
    FontSize 14
    FontName "Arial"
}
skinparam namespaceSeparator none

namespace Models {

    class Grid {
        + +/+ GameCardsGrid : List<GameCard>

        + Grid()
        + GetCard(p : Position) : GameCard?
        + {static} GetBounds(positions : List<Position>) : (int minX, int maxX, int minY, int maxY)
        + SetCard(p : Position, newCard : GameCard) : void
        + IsInGrid(p : Position) : bool
        + IsAdjacentToCard(p : Position) : (bool, GameCard?)
        + AreAdjacentCards(p1 : Position, p2 : Position) : bool
        + RemoveCard(p : Position) : void
    }

    class Position {
        + +/ Row : int
        + +/ Column : int

        + Position(row : int, column : int)
        + Equals(other : Position?) : bool
        + Equals(obj : object?) : bool
        + GetHashCode() : int
    }

    abstract class Card {
        + +/+ Number : int

        + {abstract} Card(number : int)
    }

    class GameCard {
        + +/+ Position : Position
        + +/ Splash : int

        + GameCard(splash : int, number : int)
    }

    class DeckCard {
        + +/ Bonus : Bonus

        + DeckCard(number : int)
        + DeckCard(bonus : Bonus, number : int)
    }

    class Deck {
        + +/+ Cards : List<DeckCard>

        + Deck()
    }

    enum Bonus <<enum>> {
        None
        Max
        Again
    }

    ' Relations
    Grid *-- GameCard
    Grid o-- Position
    Card <|-- GameCard
    Card <|-- DeckCard
    DeckCard --> Bonus
    GameCard *-- Position
    Deck *-- DeckCard

}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#FFA500>| Enumération |
endlegend
@enduml