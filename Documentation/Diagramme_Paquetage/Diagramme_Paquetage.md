```plantuml
@startuml
' Style général
skinparam packageStyle rectangle
skinparam defaultTextAlignment center
skinparam shadowing true

' Couleurs personnalisées pour chaque package
package "DuckAndCover" #white {
    package "Models" #application {
        package "Game" #application {
            class Card #D6EAF8
            class Game #D6EAF8
            class Player #D6EAF8
            class Deck #D6EAF8
            class Position #D6EAF8
            class GameCard #D6EAF8
            class Grid #D6EAF8
            class DeckCard #D6EAF8
        }

        package "Rules" #application {
            class ClassicRules #D6EAF8
        }

        package "Generators" #application {
            class DeckGenerator #D6EAF8
            class GridGenerator #D6EAF8
        }

        package "Interfaces" #application {
            interface "IRules" as IRules <<interface>> #D5F5E3
            interface "IGameCardGenerator" as IGameCardGenerator <<interface>> #D5F5E3
            interface "IDeckCardGenerator" as IDeckCardGenerator <<interface>> #D5F5E3
            interface "IDataPersistence" as IDataPersistence <<interface>> #D5F5E3
        }

        package "Enums" #application {
            enum Bonus <<enum>> #FAD7A0
            enum ErrorCodes <<enum>> #FAD7A0
        }

        package "Exceptions" #application {
            class Error #D6EAF8
            class ErrorHandler #D6EAF8
        }

        package "Events" #application {
            class PlayerChangedEventArgs #D6EAF8
            class GameIsOverEventArgs #D6EAF8
            class ErrorOccurredEventArgs #D6EAF8
            class PlayerChooseCoinEventArgs #D6EAF8
            class PlayerChooseDuckEventArgs #D6EAF8
            class PlayerChooseShowPlayersGridEventArgs #D6EAF8
            class PlayerChooseQuitEventArgs #D6EAF8
            class PlayerChooseCoverEventArgs #D6EAF8
            class PlayerChooseShowScoresEventArgs #D6EAF8
            class DisplayMenuNeededEventArgs #D6EAF8
        }
    }

    package "ConsoleApp" #lavender {
        class Program #D6EAF8
        class Utils #D6EAF8
    }

    package "UnitTests" #mintcream {
        class PlayerTests #D6EAF8
        class PositionTests #D6EAF8
        class ClassicRulesTests #D6EAF8
        class GridTest #D6EAF8
        class DeckCardTests #D6EAF8
        class GameTests #D6EAF8
        class GameCardTests #D6EAF8
        class DeckGeneratorTest #D6EAF8
    }
}

legend right
|= Package |= Couleur |
|<#application>| Models |
|<#lavender>| ConsoleApp |
|<#mintcream>| UnitTests |
endlegend
@enduml
