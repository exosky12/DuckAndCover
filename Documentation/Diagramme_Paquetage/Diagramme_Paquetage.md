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

    package "DataPersistence" #FFE5B4 {
        class JsonPersistency #D6EAF8
        class FakePersistency #D6EAF8
        class DataToPersist #D6EAF8
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
|<#FFE5B4>| DataPersistence |
|<#lavender>| ConsoleApp |
|<#mintcream>| UnitTests |
endlegend
@enduml

# Explication du Diagramme de Paquetage

Ce diagramme représente l'architecture du projet DuckAndCover. L'organisation est structurée en plusieurs packages principaux, chacun ayant un rôle spécifique dans l'application.

## Structure Principale

### 1. Package Models
Le package Models est le cœur de l'application et contient plusieurs sous-packages :

- **Game** : Contient les classes fondamentales du jeu comme `Card`, `Game`, `Player`, `Deck`, `Position`, `GameCard`, `Grid`, et `DeckCard`. Ces classes représentent les éléments de base du jeu.

- **Rules** : Contient la classe `ClassicRules` qui implémente les règles du jeu.

- **Generators** : Contient les classes `DeckGenerator` et `GridGenerator` responsables de la génération des éléments du jeu.

- **Interfaces** : Définit les contrats principaux de l'application avec les interfaces :
  - `IRules` : Pour les règles du jeu
  - `IGameCardGenerator` : Pour la génération des cartes de jeu
  - `IDeckCardGenerator` : Pour la génération des cartes du deck
  - `IDataPersistence` : Pour la persistance des données

- **Enums** : Contient les énumérations `Bonus` et `ErrorCodes` utilisées dans le jeu.

- **Exceptions** : Gère les erreurs avec les classes `Error` et `ErrorHandler`.

- **Events** : Contient toutes les classes d'événements pour gérer les interactions du jeu, comme les changements de joueur, la fin de partie, et les choix des joueurs.

### 2. Package DataPersistence
Ce package gère la persistance des données du jeu avec :
- `JsonPersistency` : Pour sauvegarder les données au format JSON
- `FakePersistency` : Pour simuler la persistance des données
- `DataToPersist` : Structure de données pour les informations à sauvegarder

### 3. Package ConsoleApp
Contient les classes d'interface utilisateur en mode console :
- `Program` : Point d'entrée de l'application
- `Utils` : Utilitaires pour l'interface console

### 4. Package UnitTests
Contient tous les tests unitaires de l'application, testant les différentes composantes du jeu comme les joueurs, les positions, les règles, la grille, etc.

## Code Couleur
- **Application** (bleu clair) : Packages principaux de l'application
- **Pêche** (#FFE5B4) : Package de persistance des données
- **Lavender** : Interface console
- **Mintcream** : Tests unitaires

Cette architecture suit les principes de séparation des responsabilité permettant une 
maintenance et une évolution faciles du code.