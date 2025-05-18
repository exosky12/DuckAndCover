```plantuml
@startuml
skinparam shadowing true
skinparam class {
    BackgroundColor #D6EAF8
    BorderColor black
    ArrowColor black
    FontSize 14
    FontName "Arial"
}
skinparam namespaceSeparator none

namespace Models {

    class PlayerChangedEventArgs {
        + +/+ Player : Player
        + +/+ CurrentDeckCard : DeckCard
    }

    class GameIsOverEventArgs {
        + +/+ IsOver : bool
    }

    class ErrorOccurredEventArgs {
        + +/+ Error : Error
    }

    class PlayerChooseCoinEventArgs {
        + +/+ Player : Player
    }

    class PlayerChooseDuckEventArgs {
        + +/+ Player : Player
    }

    class PlayerChooseShowPlayersGridEventArgs {
        + +/+ Players : List<Player>
    }

    class PlayerChooseQuitEventArgs {
        + +/+ Player : Player
        + +/+ Game : Game
    }

    class PlayerChooseCoverEventArgs {
        + +/+ Player : Player
    }

    class PlayerChooseShowScoresEventArgs {
        + +/+ Players : List<Player>
    }

    class DisplayMenuNeededEventArgs {
        + +/+ Player : Player
        + +/+ CurrentDeckCard : DeckCard
    }

}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
endlegend
@enduml