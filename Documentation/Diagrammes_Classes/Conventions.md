@startuml
skinparam shadowing true
skinparam class {
    BackgroundColor #ffffff
    BorderColor black
    FontSize 14
    FontName "Arial"
}

class "Convention de nommage des propriétés" as Convention

note right of Convention
  == Conventions de nommage des propriétés ==
  `+/+` : Getter public, Setter public  
  `+/-` : Getter public, Setter privé  
  `+/ #` : Getter public, Setter init  
  `-/-` : Getter privé, Setter privé  

  `+` devant le type : propriété ou méthode publique  
  `-` devant le type : propriété ou méthode privée  

  `<:zap:>` devant : évènement  
  Les méthodes statiques sont _soulignées_
end note
@enduml
