# Connections — Unity

[![Unity](https://img.shields.io/badge/Unity-6.3%20LTS-blue.svg?logo=unity)](https://unity.com/)
[![C#](https://img.shields.io/badge/C%23-9.0-green.svg?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![DOTween](https://img.shields.io/badge/DOTween-v1.2%2B-orange.svg)](http://dotween.demigiant.com/)

A recreation of the **NYT Connections puzzle game** built in Unity (C#) using **DOTween** for UI animations. Designed with a focus on event-driven architecture, clean code decoupling, and reactive game-feel.


## 🧩 Architecture

### `GameManager`
**Core Game Logic**

- Manages game state, win/loss conditions, and mistake limits.
- Handles guess validation, including exact matches, "One Away" hints, and duplicate guesses.
- Exposes C# events to keep the UI reactive and decoupled from the core logic.

### `BoardController`
**UI & Presentation**

- Responds to `GameManager` events and manages UI button states.
- Coordinates multi-stage tile and category animations.
- Handles FLIP-style anchor tweens for smooth grid reflows when categories are solved.

### `Tile` & `TileAnimator`
**Tile Behaviour & Animation**

- `Tile` manages individual tile state and display data.
- `TileAnimator` handles reusable DOTween animation sequences for tile interactions.

### `SolvedCategoryGridParent` & `SolvedCategoryRow`
**Solved Category UI**

- Creates and manages solved category rows.
- Handles category color-coding and scaling feedback animations.

### `PuzzleSO` & `AllPuzzlesSO`
**Puzzle Data**

- Uses ScriptableObjects for data-driven puzzle creation.
- Keeps puzzle content separate from gameplay logic, making new puzzles easy to create and manage.


## 🎓 What I Learned

* **State-Synchronized Animations:** Deferring state transitions until async DOTween sequences complete keeps visual feedback and game logic perfectly in sync.
* **FLIP-Style Grid Reflows:** Rebuilding Unity UI layouts dynamically while capturing pre/post `anchoredPosition` values enables smooth layout transition animations.
* **Controlled Board Shuffling:** Implementing index-filtered reordering allows active tiles to shuffle without disturbing static, solved category rows.
* **Event-Driven Decoupling:** Managing game flow through standard C# events maintains a clean separation between pure game logic and presentation.
