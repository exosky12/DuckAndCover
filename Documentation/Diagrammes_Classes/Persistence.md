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

    interface "IDataPersistence" as IDataPersistence <<interface>> {
        + LoadData() : (ObservableCollection<Player>, ObservableCollection<Game>)
        + SaveData(players : ObservableCollection<Player>, games : ObservableCollection<Game>)
    }

    class FakePersistency {
        - _players : ObservableCollection<Player>
        - _games : ObservableCollection<Game>
        + +/% Players : ReadOnlyObservableCollection<Player>
        + +/% Games : ReadOnlyObservableCollection<Game>
        + FakePersistency()
        + LoadData() : (ObservableCollection<Player>, ObservableCollection<Game>)
        + SaveData(players : ObservableCollection<Player>, games : ObservableCollection<Game>)
        + LoadPlayers()
        + LoadGames()
        - GeneratePlayers() : ObservableCollection<Player>
        - GenerateGames() : List<Game>
        - CreateJordyGrid() : Grid
        - CreateJordy1Grid() : Grid
        - CreateJules1Grid() : Grid
    }

    class JsonPersistency {
        + +/+ FileName : string
        + +/+ FilePath : string
        + JsonPersistency()
        + LoadData() : (ObservableCollection<Player>, ObservableCollection<Game>)
        + SaveData(players : ObservableCollection<Player>, games : ObservableCollection<Game>)
    }

    class DataToPersist {
        + +/+ Players : ObservableCollection<Player>?
        + +/+ Games : ObservableCollection<Game>?
    }

    class Player {
        + +/% Name : string
        + +/+ HasSkipped : bool
        + +/+ HasPlayed : bool
        + +/+ Scores : List<int>
        + +/+ TotalScore : int
        + +/+ StackCounter : int
        + +/+ Grid : Grid
        + Player(name : string)
        + Player(name : string, stack : int, scores : List<int>, skipped : bool, played : bool, grid : Grid)
        + HasCardWithNumber(number : int) : bool
    }

    class Game {
        + +/+ Id : string
        - _allPlayers : ObservableCollection<Player>
        + +/+ AllPlayers : ObservableCollection<Player>
        + +/+ Players : List<Player>
        - _games : ObservableCollection<Game>
        + +/+ Games : ObservableCollection<Game>
        + +/+ Rules : IRules
        + +/+ CardsSkipped : int
        + +/+ CurrentPlayer : Player
        - _currentPlayerIndex : int
        + +/+ Deck : Deck
        + +/+ Quit : bool
        + +/+ IsFinished : bool
        + +/% LastGameFinishStatus : bool
        + +/% CurrentDeckCard : DeckCard
        + +/+ LastNumber : int?

        + <:zap:>PlayerChanged : EventHandler<PlayerChangedEventArgs>
        + <:zap:>GameIsOver : EventHandler<GameIsOverEventArgs>
        + <:zap:>ErrorOccurred : EventHandler<ErrorOccurredEventArgs>
        + <:zap:>PlayerChooseCoin : EventHandler<PlayerChooseCoinEventArgs>
        + <:zap:>PlayerChooseDuck : EventHandler<PlayerChooseDuckEventArgs>
        + <:zap:>PlayerChooseShowPlayersGrid : EventHandler<PlayerChooseShowPlayersGridEventArgs>
        + <:zap:>PlayerChooseQuit : EventHandler<PlayerChooseQuitEventArgs>
        + <:zap:>PlayerChooseCover : EventHandler<PlayerChooseCoverEventArgs>
        + <:zap:>PlayerChooseShowScores : EventHandler<PlayerChooseShowScoresEventArgs>
        + <:zap:>DisplayMenuNeeded : EventHandler<DisplayMenuNeededEventArgs>
        + <:zap:>PropertyChanged : PropertyChangedEventHandler

        + Game(rules : IRules)
        + InitializeGame(id : string, players : List<Player>, deck : Deck, currentDeckCard : DeckCard, currentPlayerIndex : int, cardsSkipped : int, isFinished : bool, lastNumber : int?)
        + NextPlayer()
        + GameLoop()
        + HandlePlayerChoice(player : Player, choice : string)
        + HandlePlayerChooseCover(player : Player, cardToMovePosition : Position, cardToCoverPosition : Position)
        + TriggerGameOver()
        + HandlePlayerChooseDuck(player : Player, cardToMovePosition : Position, duckPosition : Position)
        + CheckGameOverCondition() : bool
        + DoCover(player : Player, cardToMovePosition : Position, cardToCoverPosition : Position)
        + DoDuck(player : Player, cardToMovePosition : Position, duckPosition : Position)
        + DoCoin(player : Player)
        + NextDeckCard() : DeckCard
        + AllPlayersPlayed() : bool
        + CheckAllPlayersSkipped()
        + SavePlayers()
        + SaveGame()
    }

    ' Relations
    FakePersistency ..|> IDataPersistence
    JsonPersistency ..|> IDataPersistence

    FakePersistency ..> Player : génère
    FakePersistency ..> Game : génère

    JsonPersistency ..> DataToPersist : utilise
    DataToPersist *-- Player : contient
    DataToPersist *-- Game : contient

    JsonPersistency ..> Player : désérialise
    JsonPersistency ..> Game : désérialise

    Game *-- Player : contient
}

legend right
    |= Type |= Couleur |
    |<#D6EAF8>| Classe |
    |<#D5F5E3>| Interface |
endlegend
@enduml
```

# Explication du Diagramme de Classes de Persistance

Ce diagramme représente l'architecture de la persistance des données dans l'application DuckAndCover. Il met en évidence les différentes classes et interfaces impliquées dans la sauvegarde et le chargement des données du jeu.

## Structure Principale

### 1. Interface IDataPersistence
L'interface `IDataPersistence` définit le contrat que doivent respecter toutes les implémentations de persistance :
- `LoadData()` : Méthode pour charger les données (joueurs et parties)
- `SaveData()` : Méthode pour sauvegarder les données

### 2. Implémentations de la Persistance

#### JsonPersistency
Cette classe gère la persistance des données au format JSON :
- Utilise `DataToPersist` comme structure intermédiaire
- Gère la sérialisation/désérialisation des objets `Player` et `Game`
- Stocke les données dans un fichier JSON

#### FakePersistency
Cette classe simule la persistance des données pour les tests et le développement :
- Génère des données fictives pour les joueurs et les parties
- Implémente des méthodes spécifiques pour la génération de données de test
- Permet de tester l'application sans dépendre d'un système de stockage réel

### 3. Structure de Données

#### DataToPersist
Classe conteneur qui structure les données à persister :
- Contient des collections de `Player` et `Game`
- Sert d'intermédiaire pour la sérialisation JSON

### 4. Classes Métier

#### Player
Représente un joueur avec ses attributs :
- Informations personnelles (nom)
- État du jeu (scores, grille, etc.)
- Méthodes de gestion du jeu

#### Game
Représente une partie de jeu avec :
- Gestion des joueurs
- État de la partie
- Événements du jeu
- Méthodes de gestion de la partie

## Relations et Interactions

1. **Relations d'Implémentation**
   - `FakePersistency` et `JsonPersistency` implémentent `IDataPersistence`
   - Chaque implémentation fournit sa propre logique de persistance

2. **Relations de Composition**
   - `DataToPersist` contient des collections de `Player` et `Game`
   - `Game` contient une collection de `Player`

3. **Relations de Dépendance**
   - `JsonPersistency` utilise `DataToPersist` pour la sérialisation
   - `FakePersistency` génère des instances de `Player` et `Game`

## Points Forts de l'Architecture

1. **Flexibilité**
   - Interface commune permettant différentes implémentations
   - Facilité d'ajout de nouvelles méthodes de persistance

2. **Séparation des Responsabilités**
   - Claires séparations entre la persistance et la logique métier
   - Structure modulaire facilitant la maintenance

3. **Testabilité**
   - `FakePersistency` permet des tests sans dépendance externe
   - Architecture facilitant les tests unitaires
