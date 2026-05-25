# TestArmyClushGame

## О проекте

`TestArmyClushGame` — это мобильный игровой прототип в жанре автобатлер в стиле VooDoo, реализованный на Unity. Цель проекта — демонстрация гибкой, модульной архитектуры с чётким разделением слоёв. 

## Ключевые особенности

- Сцена главного меню с выбором силы армии и кнопкой "Играть"
- Асинхронная загрузка сцен с затемнением и индикатором прогресса
- DI через `VContainer` и явное разделение на глобальные и сценовые зависимости
- Событийная шина `EventBus` для общения между менеджерами и UI
- Генерация бойцов и расстановка боевых формаций
- Боевая логика с обновлением юнитов, столкновениями через `SpatialGrid` и системой смерти юнитов

## Архитектура

Проект разделён на следующие слои:

- `Infrastructure` — базовые сервисы, переходы сцен, аудио и экономика
- `DI` — конфигурация зависимостей для каждой сцены
- `MainMenu` — логика главного меню, UI-презентеры и вьюхи
- `Gameplay` — основная игровая сцена, менеджеры боя и управление состоянием
- `GameEvents` — события игры

### Сценарии запуска

- `EntryPointScene` — стартовая сцена проекта, в которой инициализируется `AppEntryPoint`. С этой сцены должна запускаться игра.
- `MainMenuScene` — сцена главного меню с выбором желаемой мощности армий. Армия захватчиков примерно соответствует желаемой мощности, армия защитников +/- 20% от желаемой. Рандомно меняется на игровом экране.
- `GameplayScene` — игровая сцена

### Основной поток

```mermaid
flowchart TD
    AppEntryPoint -->|Play music + load menu|PerfomanceOverlay|SceneTransitionManager
    SceneTransitionManager --> MainMenuScene
    MainMenuScene --> MainMenuLifetimeScope
    MainMenuLifetimeScope --> MainMenuBootstrap
    MainMenuBootstrap --> PlayButtonPresenter
    PlayButtonPresenter --> PlayButtonView
    PlayButtonView -->|click| EventBus
    EventBus --> MainMenuManager
    MainMenuManager --> SceneTransitionManager
    SceneTransitionManager --> GameplayScene
    GameplayScene --> GameplayLifetimeScope
    GameplayLifetimeScope --> GameplayBootstrap
    GameplayBootstrap --> GameplayManager
    GameplayBootstrap --> BattleManager
```

## Основные модули

### `Assets/_Project/_Scripts/_Runtime/Infrastructure`

- `AppEntryPoint.cs` — старт приложения, воспроизведение фоновой музыки, загрузка меню
- `SceneTransitionManager.cs` — асинхронная загрузка сцен с прогрессом
- `EventBus` — простая потокобезопасная шина событий
- `Audio` — управление музыкой и звуковыми эффектами
- `Economy` — экономическая подсистема

### `Assets/_Project/_Scripts/_Runtime/DI`

- `RootLifetimeScope.cs` — глобальные синглтоны и сервисы приложения
- `MainMenuLifetimeScope.cs` — регистрация компонентов главного меню
- `GameplayLifetimeScope.cs` — регистрация игровых сервисов, UI, спавнеров и менеджеров

### `Assets/_Project/_Scripts/_Runtime/MainMenu`

- `MainMenuBootstrap.cs` — сборка и инициализация UI-презентеров
- `MainMenuManager.cs` — обработка перехода в игровой режим
- `Views/` — визуальные компоненты меню
- `Presenters/` — привязка логики к UI

### `Assets/_Project/_Scripts/_Runtime/Gameplay`

- `GameplayBootstrap.cs` — точка входа для сцены Gameplay
- `GameplayManager.cs` — управление состоянием игры и цикл обновления
- `Battle/BattleManager.cs` — организация боя, формирование армий и запуск столкновений
- `Units/` — системы юнитов, фабрика, пул объектов, логика искусственного интеллекта
- `UI/` — игровые UI-вьюхи и презентеры

## Структура папок проекта

- `Assets/_Project/_Scripts/_Runtime/DI`
- `Assets/_Project/_Scripts/_Runtime/Infrastructure`
- `Assets/_Project/_Scripts/_Runtime/MainMenu`
- `Assets/_Project/_Scripts/_Runtime/Gameplay`
- `Assets/_Project/_Scripts/_Runtime/GameEvents`
- `Assets/_Project/_Scripts/Editor`

## Важные классы и службы

- `EventBus` — центральный посредник событий между сценами и сервисами
- `IStartable` Bootstrap-классы — инициализируют сцену и связывают UI
- `SpatialGrid` — ускоряет поиск соседних юнитов на поле боя
- `ArmySpawner` — создаёт экземпляры юнитов и раскрашивает их
- `FormationBuilder` — формирует расположение войск по заданной мощности
- `CoinObjectPool` и `UnitObjectPool` — пуллы для повторного использования объектов

## Зависимости

Проект использует следующие пакеты:

- `com.unity.render-pipelines.universal`
- `com.unity.ugui`
- `jp.hadashikick.vcontainer` — DI-контейнер VContainer
- `com.cysharp.unitask` — UniTask для асинхронной логики
- `DG.Tweening` — DOTween для анимации UI

## Как открыть и запустить

1. Откройте `TestArmyClushGame.slnx` или директорию проекта в Unity Editor.
2. Убедитесь, что пакетный кэш загрузил внешние зависимости.
3. Откройте сцену `EntryPointScene` и запустите игру.
4. На главном меню нажмите `Play` для перехода в `GameplayScene`.
