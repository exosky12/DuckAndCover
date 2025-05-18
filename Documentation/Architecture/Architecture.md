# 🦆 Architecture du Projet **DuckAndCover**

Ce document décrit l'architecture de **DuckAndCover**, en listant les outils de développement, l'intégration continue, la qualité de code, ainsi que l'organisation du projet.

---

## Outils & Infrastructure

### Développement & Collaboration

- **Git** : Outil de versionning. Suivi des modifications, gestion des branches, pull requests, etc.
- **NuGet** : Gestionnaire de packages .NET pour l'installation et la mise à jour des dépendances.


### Modélisation & Documentation

- **PlantUML** : Création de diagrammes UML (classes, séquences, composants, etc.) pour illustrer l'architecture.
- **Doxygen** : Génération automatique de documentation à partir des commentaires dans le code C#.


### Intégration Continue & Qualité

- **Drone CI** : Pipeline CI pour l'exécution automatique des builds, tests unitaires, et vérifications de code à chaque push.
- **SonarQube** : Analyse continue de la qualité du code (couverture des tests, duplications, bugs, code smells, etc.).


### Stack Technique

- **.NET 9** : Framework principal utilisé pour la logique métier et la console.
- **.NET MAUI** & **XAML** : Utilisés pour le développement de l'interface graphique, en complément de la version console.

---

## Structure du projet

### `Models`

Le dossier Models contient toute la logique métier du jeu et est organisé de la manière suivante :

#### Organisation des dossiers

- **Game/** : Contient les classes principales du jeu :
  - `Game.cs` : Classe principale gérant l'état du jeu
  - `Player.cs` : Gestion des joueurs
  - `Grid.cs` : Représentation de la grille de jeu
  - `Position.cs` : Gestion des positions sur la grille
  - `Card.cs`, `DeckCard.cs`, `GameCard.cs` : Gestion des cartes et du deck
  - `Deck.cs` : Gestion du deck de cartes

- **Events/** : Implémente le système d'événements pour la communication entre composants
- **Exceptions/** : Contient les classes d'exceptions personnalisées et le système de gestion d'erreurs
- **Interfaces/** : Définit les contrats utilisés dans le projet
- **Enums/** : Regroupe tous les enums utilisées dans le jeu
- **Rules/** : Contient les règles du jeu :
  - `ClassicRules.cs` : Implémentation des règles classiques du jeu
- **Generators/** : Implémente les générateurs pour la création de contenu du jeu

#### Architecture et patrons 

- Utilisation du patron Standard des Événements pour la communication entre composants
- Système de gestion d'erreurs personnalisé avec :
  - Une classe d'erreur personnalisée
  - Un gestionnaire d'erreurs dédié
  - Un enum représentant tous les types d'erreurs possibles
- Interfaces bien définies pour permettre l'extension et la modification du comportement
- Séparation claire des responsabilités entre les différents composants (séparation code métier et affichage)

### `DataPersistence`

- Gère la persistance des données du jeu.
- Implémente la sauvegarde et le chargement des parties.

### `ConsoleApp`

- Application **console** servant à tester la logique sans UI graphique.
- Utilisée pour valider rapidement les fonctionnalités du modèle.

### `UnitTests`

- Contient les tests unitaires gérés par **xUnit**.
- Garantit le bon fonctionnement du code via une suite de tests exécutée automatiquement par **Drone CI** à chaque push sur la branch `master`.

### `DuckAndCover`

- Projet principal contenant l'application MAUI.
- Gère l'interface utilisateur et l'interaction avec le modèle.

---

## Patrons de Conception

Le projet utilise le patron Standard des Événements pour assurer une architecture robuste et maintenable :

### Patron Standard des Événements

- Utilisé pour la communication entre les composants du jeu
- Permet un découplage efficace entre le modèle et la vue
- Implémenté via le système d'événements standard de C#
- Facilite la communication asynchrone entre les différentes parties du jeu
- Permet une meilleure testabilité des composants

### Gestion des Erreurs Personnalisée

- Implémentation d'un système de gestion d'erreurs robuste avec :
  - Une classe d'erreur personnalisée
  - Un gestionnaire d'erreurs dédié
  - Un enum représentant tous les types d'erreurs possibles de l'application
- Permet une gestion centralisée et cohérente des erreurs
- Facilite le débogage et la maintenance

Ces patrons de conception contribuent à la maintenabilité et à l'évolutivité du code, tout en facilitant les tests unitaires et l'intégration de nouvelles fonctionnalités.
