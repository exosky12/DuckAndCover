```plantuml 
@startuml
skinparam shadowing true
skinparam class {
    BackgroundColor #D6EAF8
    BackgroundColor<<Interface>> #D5F5E3
    BackgroundColor<<enum>> #FFA500
    BackgroundColor<<event>> #FFD700
    BorderColor black
    ArrowColor black
    FontSize 14
    FontName "Arial"
}

namespace Models {

    interface "IRules" as IRules <<interface>> {
        + Name : string
        + Description : string
        + NbCardsInDeck : int
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class Game {
        + Rules : IRules
        + Game(rules : IRules)
        + HandlePlayerChoice(player : Player, choice : string)
        + HandlePlayerChooseCover(player : Player, cardToMovePosition : Position, cardToCoverPosition : Position)
        + HandlePlayerChooseDuck(player : Player, cardToMovePosition : Position, duckPosition : Position)
        + CheckGameOverCondition() : bool
    }

    class ClassicRules {
        + ClassicRules()
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class BlitzRules {
        + BlitzRules()
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    class InsaneRules {
        + InsaneRules()
        + TryValidMove(position : Position, newPosition : Position, grid : Grid, funcName : string, currentDeckCard : DeckCard)
        + IsGameOver(cardPassed : int, stackCounter : int, quit : bool) : bool
        + isTheSameCard(currentCard : GameCard, currentDeckCard : DeckCard) : bool
    }

    ' Relations
    ClassicRules ..|> IRules
    BlitzRules ..|> IRules
    InsaneRules ..|> IRules

    Game o-- IRules : utilise
}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#D5F5E3>| Interface |
endlegend
@enduml 

# Explication du Diagramme de Classes des Règles

Ce diagramme représente l'architecture des règles du jeu DuckAndCover, mettant en évidence les différentes implémentations de règles et leur utilisation par la classe Game.

## Structure Principale

### 1. Interface IRules
L'interface `IRules` définit le contrat que doivent respecter toutes les implémentations de règles :
- Propriétés fondamentales :
  - `Name` : Nom du mode de jeu
  - `Description` : Description des règles spécifiques
  - `NbCardsInDeck` : Nombre de cartes dans le deck
- Méthodes principales :
  - `TryValidMove` : Vérifie la validité d'un mouvement
  - `IsGameOver` : Détermine si la partie est terminée
  - `isTheSameCard` : Compare deux cartes

### 2. Classe Game
La classe `Game` est le seul consommateur des règles :
- Contient une référence à `IRules`
- Utilise les règles pour :
  - Gérer les choix des joueurs
  - Vérifier la validité des mouvements
  - Déterminer la fin de partie

### 3. Implémentations des Règles

#### ClassicRules
Implémentation des règles classiques du jeu :
- Règles standard pour une partie normale
- Conditions de fin de partie classiques

#### BlitzRules
Mode de jeu rapide avec des règles adaptées :
- Règles simplifiées pour des parties plus courtes
- Vérifications de mouvements accélérées
- Conditions de fin de partie plus rapides

#### InsaneRules
Mode de jeu avec des règles plus complexes :
- Règles qui rendent la partie plus longue et extravagante
- Conditions de fin de partie plus longues

## Relations et Interactions

1. **Relations d'Implémentation**
   - Les trois classes de règles implémentent `IRules`
   - Chaque implémentation fournit sa propre logique de jeu

2. **Relation d'Utilisation**
   - La classe `Game` utilise `IRules` pour gérer la logique du jeu
   - Cette relation est représentée par une composition (o--)

## Points Forts de l'Architecture

1. **Séparation des Responsabilités**
   - La logique des règles est complètement séparée de la logique du jeu
   - Chaque implémentation de règles est indépendante

2. **Extensibilité**
   - Interface commune permettant d'ajouter facilement de nouveaux modes de jeu
   - Structure modulaire facilitant l'ajout de nouvelles règles

3. **Maintenabilité**
   - Code organisé et structuré
   - Facilité de modification des règles existantes

Cette architecture des règles permet une grande flexibilité dans la gestion des différents modes de jeu tout en maintenant une structure claire et maintenable, avec une séparation nette entre la logique du jeu et les règles