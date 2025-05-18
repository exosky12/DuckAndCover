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

    class Error {
        + +/+ ErrorCode : ErrorCodes

        + Error(errorCode : ErrorCodes, message : string = "") : void
    }

    class ErrorHandler {
        + +/+ Error : Error
        + +/+ Message : string

        + ErrorHandler(error : Error)
        + HandleError() : void
        + GetErrorMessage() : string
    }

    enum ErrorCodes <<enum>> {
        InvalidChoice
        WrongPositionFormat
        PositionsMustBeIntegers
        DeckEmpty
        InvalidMove
        InvalidCard
        InvalidPosition
        InvalidPlayer
        InvalidGame
        InvalidGrid
        InvalidDeck
        InvalidRules
        InvalidBonus
        InvalidSplash
        InvalidNumber
        InvalidStack
        InvalidScore
        InvalidName
        InvalidId
        InvalidState
    }

    ' Relations
    Error --> ErrorCodes
    ErrorHandler --> Error

}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#FFA500>| Enumération |
endlegend
@enduml