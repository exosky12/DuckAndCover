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

    interface IDataPersistence <<interface>> {
        + LoadData() : (List<Player>, List<Game>)
        + SaveData(players : List<Player>, games : List<Game>) : void
    }

    class Stub {
        + LoadData() : (List<Player>, List<Game>)
        + SaveData(players : List<Player>, games : List<Game>) : void
    }

    ' Relations
    Stub ..|> IDataPersistence

}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#D5F5E3>| Interface |
endlegend
@enduml