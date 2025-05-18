```plantuml
@startuml
' Style général
skinparam packageStyle rectangle
skinparam defaultTextAlignment center
skinparam shadowing true

' Couleurs personnalisées pour chaque package
package "DuckAndCover" #white {
    package "Model" #application {
        class Card #D6EAF8
        class Game #D6EAF8
        class Player #D6EAF8
        class Deck #D6EAF8
        class Position #D6EAF8
        class ClassicRules #D6EAF8
        class DeckGenerator #D6EAF8
        class GameCard #D6EAF8
        class Grid #D6EAF8
        class GridGenerator #D6EAF8
        interface IRules <<interface>> #D5F5E3
        interface IGenerator <<interface>> #D5F5E3 {}
        class DeckCard #D6EAF8
        enum Bonus <<enum>> #FAD7A0
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
|<#application>| Model |
|<#lavender>| ConsoleApp |
|<#mintcream>| UnitTests |
endlegend

@enduml
