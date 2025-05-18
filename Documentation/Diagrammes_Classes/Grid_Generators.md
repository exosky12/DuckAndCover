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

    interface IGenerator<T> <<Interface>> {
        + Generate() : List<T>
    }

    class Grid {
        + +/+ GameCardsGrid : List<GameCard>
        + Grid()
        + GetBounds(positions : List<Position>) : (int minX, int maxX, int minY, int maxY)
        + GetCard(p : Position) : GameCard?
        + SetCard(p : Position, newCard : GameCard) : void
        + IsInGrid(p : Position) : bool
        + IsAdjacentToCard(p : Position) : (bool isAdjacent, GameCard? adjacentCard)
        + RemoveCard(p : Position) : void
    }

    class DeckGenerator {
        + +/+ Deck : List<DeckCard>
        + +/+ AllPossibleCards : List<DeckCard>
        + DeckGenerator()
        + Generate() : List<DeckCard>
        - InitializeDeck() : List<DeckCard> {static}
        - GetSecureRandomIndex(max : int) : int {static}
    }

    class GridGenerator {
        + +/+ Grid : List<GameCard>
        + +/+ AllPositions : List<Position>
        + +/+ NbCards : int = 12
        + GridGenerator()
        + Generate() : List<GameCard>
        - InitializePositions() : List<Position> {static}
        - GenerateAllCards() : List<GameCard> {static}
    }

    class Deck {
        + +/+ Cards : List<DeckCard>
        + Deck()
    }

    ' Relations
    Grid "1" *-- "many" GameCard
    DeckGenerator "1" *-- "many" DeckCard
    DeckGenerator ..|> IGenerator
    GridGenerator ..|> IGenerator
    Deck "1" *-- "many" DeckCard
    GridGenerator "1" *-- "many" GameCard
}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#D5F5E3>| Interface |
endlegend
@enduml
