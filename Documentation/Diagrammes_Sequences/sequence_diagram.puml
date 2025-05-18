@startuml
title Duck and Cover - Diagramme de Séquence

actor Player
participant "Game" as Game
participant "Player" as PlayerClass
participant "Grid" as Grid
participant "Rules" as Rules
participant "Deck" as Deck

== Initialisation ==
Player -> Game: new Game(players)
activate Game
Game -> Deck: new Deck()
activate Deck
Deck --> Game: deck
deactivate Deck
Game -> Rules: new ClassicRules()
activate Rules
Rules --> Game: rules
deactivate Rules
Game -> PlayerClass: new Player(name)
activate PlayerClass
PlayerClass -> Grid: new Grid()
activate Grid
Grid --> PlayerClass: grid
deactivate Grid
PlayerClass --> Game: player
deactivate PlayerClass
deactivate Game

== Tour de jeu ==
Player -> Game: DoCover(player, cardToMovePos, cardToCoverPos)
activate Game
Game -> Rules: TryValidMove()
activate Rules
Rules --> Game: validation
deactivate Rules
Game -> Grid: GetCard(cardToMovePos)
activate Grid
Grid --> Game: cardToMove
Game -> Grid: GetCard(cardToCoverPos)
Grid --> Game: cardToCover
Game -> Grid: RemoveCard(cardToCover)
Game -> Grid: UpdateCardPosition(cardToMove)
Game -> PlayerClass: UpdateStackCounter()
activate PlayerClass
PlayerClass --> Game: updated
deactivate PlayerClass
Game -> Game: NextPlayer()
Game --> Player: result
deactivate Game
deactivate Grid

== Fin de partie ==
Player -> Game: Save()
activate Game
Game -> PlayerClass: CalculateScore()
activate PlayerClass
PlayerClass --> Game: score
deactivate PlayerClass
Game -> Game: CheckGameOverCondition()
Game --> Player: gameOver
deactivate Game

@enduml 