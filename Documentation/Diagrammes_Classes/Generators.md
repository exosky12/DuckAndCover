```plantuml
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

namespace Models {

    interface IGameCardGenerator <<interface>> {
        + Generate() : List<GameCard>
    }

    interface IDeckCardGenerator <<interface>> {
        + Generate() : List<DeckCard>
    }

    class GridGenerator {
        + +/+ Grid : List<GameCard>
        + +/+ AllPositions : List<Position>
        + +/+ NbCards : int

        + GridGenerator()
        + Generate() : List<GameCard>
        - InitializePositions() : List<Position>
        - GenerateAllCards() : List<GameCard>
    }

    class DeckGenerator {
        + +/+ Cards : List<DeckCard>
        + +/+ NbCards : int

        + DeckGenerator()
        + Generate() : List<DeckCard>
        - GenerateAllCards() : List<DeckCard>
    }

    ' Relations
    GridGenerator ..|> IGameCardGenerator
    DeckGenerator ..|> IDeckCardGenerator

}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#D5F5E3>| Interface |
endlegend
@enduml
