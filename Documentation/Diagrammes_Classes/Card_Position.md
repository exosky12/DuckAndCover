@startuml
skinparam shadowing true
skinparam class {
    BackgroundColor #D6EAF8
    BackgroundColor<<Enum>> #FAD7A0
    BorderColor black
    ArrowColor black
    FontSize 14
    FontName "Arial"
}
skinparam namespaceSeparator none

namespace Model {

    enum Bonus <<Enum>> {
        None
        Max
        Again
    }

    abstract class Card {
        + +/+ Number : int
        + Card(number : int)
    }

    class GameCard {
        + +/+ Position : Position
        + +/+ Splash : int
        + GameCard(splash : int, number : int)
    }

    class DeckCard {
        + -/- Bonus : Bonus
        + DeckCard(number : int)
        + DeckCard(bonus : Bonus, number : int)
    }

    class Position {
        + -/- Row : int
        + -/- Column : int
        + Position(row : int, column : int)
        + Equals(other : Position?) : bool
        + Equals(obj : object?) : bool {override}
        + GetHashCode() : int {override}
    }

    ' Relations
    GameCard --|> Card
    DeckCard --|> Card
    DeckCard --> Bonus
    GameCard --> Position
}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#FAD7A0>| Enum |
endlegend
@enduml
