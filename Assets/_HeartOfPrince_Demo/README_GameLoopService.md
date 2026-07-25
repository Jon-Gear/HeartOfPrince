# Heart of Prince — Scalable Activity and Day Loop

## Import and run

1. Import `_HeartOfPrince_Demo` into the Unity project's `Assets` folder.
2. Allow Unity and Yarn Spinner to recompile.
3. Add the supplied scenes to Build Settings, or run the included editor build installer.
4. Open `Scenes/Bootstrap/Bootstrap.unity`.
5. Enter Play Mode.

The supplied archive is an Assets subfolder rather than a complete Unity project. Final compilation and Play Mode validation therefore occur in the host project with its installed Yarn Spinner, Cinemachine, and other package dependencies.

## Runtime flow

```text
Start day
  -> set explicit clock time
  -> query activity option providers
  -> show the reusable decision scene
  -> choose a typed ActivityRequest<TInput>
  -> validate day, activity, character, and scene rules
  -> create serializable ActivityRunState
  -> load the isolated activity scene
  -> complete the activity
  -> apply results and advance time
  -> return to the decision scene or end the day
```

The demo permits two activities per day. Talk and Ponder each take six hours. Prince wakes at 08:00, so the first selection uses a morning activity scene and the second uses an evening activity scene.

## Configuration assets

The default runtime configuration is loaded from:

- `Resources/HeartOfPrince/GameConfiguration.asset`
- `Resources/HeartOfPrince/DemoActivityCatalog.asset`
- `Resources/HeartOfPrince/Time/DemoDayRules.asset`
- `Resources/HeartOfPrince/Activities/Talk.asset`
- `Resources/HeartOfPrince/Activities/Ponder.asset`
- `Resources/HeartOfPrince/Characters/Munir.asset`
- `Resources/HeartOfPrince/ActivityModules/TalkActivityModule.asset`
- `Resources/HeartOfPrince/ActivityModules/NoInputActivityModule.asset`

`GameConfiguration` points to the bootstrap scene, starting chapter, and activity catalog. The catalog contains day rules, activity definitions, and character definitions.

## Typed activity inputs

Every activity request carries its own strongly typed payload:

```csharp
var input = new TalkActivityInput(characterId);

ActivityOption option =
    GameSession.Instance.ActivityQuery.FindOption(
        "talk",
        input);

GameLoopService.Instance.RequestActivity(option);
```

You can also construct a request directly when you already have the definition:

```csharp
GameLoopService.Instance.RequestActivity(
    talkActivity,
    new TalkActivityInput(characterId));
```

The generic request type is:

```csharp
ActivityRequest<TInput>
    where TInput : class, IActivityInput
```

Activities without parameters use `NoActivityInput.Instance`. A future activity can define a dedicated input class containing any fields it needs.

## Request and run-state boundary

```text
ActivityRequest<TInput>
  -> availability and handler validation
  -> ActivityRunState
  -> isolated Unity scene
  -> ActivityResult
  -> clock/history/day progression
```

Request input describes what the caller wants. Run data is the finalized, serializable snapshot that the activity scene reads. Talk uses `TalkActivityRunData`, which stores the resolved character ID.

## Runtime modules

Each `ActivityDefinition` contains a stable `runtimeModuleId`. At startup, `ActivityModuleRegistry` loads module assets from `Resources/HeartOfPrince/ActivityModules` and wires every configured activity into `ActivityService` and `ActivityQueryService`.

The central `GameSession` and `GameLoopService` contain no Talk or Ponder registration branches.

The supplied modules are:

- `TalkActivityRuntimeModule`, which owns `TalkActivityInput`, the Talk handler, and one option per configured character.
- `NoInputActivityRuntimeModule`, which can serve Ponder and any future activity that needs no input payload.

## Adding a new activity with custom input

1. Create an `IActivityInput` implementation.
2. Create an optional `IActivityRunData` implementation for serializable resolved state.
3. Create an `ActivityHandler<TInput>`.
4. Create an `IActivityOptionProvider`.
5. Create an `ActivityRuntimeModule` subclass that registers the handler/provider and can reconstruct a request for standalone scene debugging.
6. Create one module asset in `Resources/HeartOfPrince/ActivityModules`.
7. Set the activity asset's `runtimeModuleId` to that module asset's ID.
8. Add the activity definition to the catalog and author its isolated scenes.

No central scheduler or session switch statement needs to change.

## Decision UI

`DecisionScenePresenter` queries `ActivityQueryService` and renders the current options. It has no Talk, Ponder, or character-specific branches. The current implementation uses a minimal IMGUI panel so the architecture remains prefab-independent; it can be replaced by UI Toolkit or uGUI without changing the application layer.

`Loop_Decision` provides narrative text only. It no longer owns a hard-coded Yarn choice list.

## Scene variants and availability

`ActivityDefinition` resolves scenes from authored variants using:

- target ID, when applicable;
- earliest minute;
- latest minute.

Availability can be extended with reusable `AvailabilityRule` assets. Supplied rules include time windows, once-per-day restrictions, and required story flags. Character definitions can also have separate talk-availability rules.

Standalone activity scenes derive their debug start time and target from the authored scene variant. Scene names are no longer parsed for “morning,” “evening,” or character names.

## Persistent state

`GameState` owns:

- `WorldClockState`;
- `DayActivityState`;
- activity history;
- the active `ActivityRunState`;
- story flags;
- existing topic and relationship state;
- narrative loop state.

The loop no longer stores a Talk/Ponder enum, a current Talk character field, morning/evening booleans, decision-index time assumptions, or activity-specific scene routes.

## Yarn commands

- `<<CompleteActivity>>`
- `<<CompleteDayOpening>>`
- `<<CompleteDay>>`
- `<<CompleteChapterStart>>`
- `<<CompleteActStart>>`
- `<<CompleteAct>>`
- `<<CompleteChapter>>`

A generic command remains available for authored direct requests:

```text
<<StartActivity "activity-id" "selection-key">>
```

Normal decision choices use `DecisionScenePresenter` and typed runtime requests.

## Standalone scene debugging

Playing a configured narrative or activity scene directly creates a temporary `GameSession` and loads the default Resources configuration. Activity scenes reconstruct typed requests through their runtime modules, and decision scenes generate their options dynamically.
