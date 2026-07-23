# Game Loop Service — Revision Summary

## Revised architecture

- Removed the persistent Dialogue Runner and persistent EventSystem design.
- `GameLoopService` now coordinates scene loading and loop state only.
- Every scene retains its own Dialogue Runner, Auto Start setting, Starting Node, dialogue UI, and EventSystem.
- The service waits for the active scene's runner to finish before transitioning.
- Talk routes now select morning/evening scenes; they no longer specify a Yarn node.

## Standalone scene debugging

- `GameSession` registers a runtime scene bootstrap.
- If a non-Bootstrap scene is played directly and no session exists, one debug session is created.
- The loop detects the active scene and enters standalone-scene mode.
- It does not redirect to Bootstrap.
- Directly launched Talk and Ponder scenes seed temporary topics and run their own starting nodes.
- Directly launched Decision scenes can route into an action, then stop after the action completes.
- Reset All Progression reloads the current standalone scene.
- Start New Game exits standalone mode and starts the full loop.
- Editor menu commands switch between playing the current open scene and forcing Bootstrap as the Play Mode start scene.

## Existing hub integration

- Munir Talk actions now enter through `Start_Munir`.
- `Start_Munir` uses the existing `TopicHub`.
- `TopicHub` uses `PrepareTopicHubNPC` and `TopicHubNPC`.
- Ponder actions enter through `Ponder_Start`.
- `Ponder_Start` uses the existing `Ponder_TopicHub`.
- The previous custom Talk/Ponder Yarn loops are retained only as backwards-compatible aliases.

## Topic chain

- `PrototypeAskAboutResponsibility`
  - unlocks `PrototypePonderResponsibility`
- `PrototypePonderResponsibility`
  - unlocks `PrototypeAskAboutLeadership`
  - unlocks `PrototypeMunirQuestion`
- `PrototypeAskAboutLeadership`
  - unlocks `PrototypePonderPromises`

Prototype topics are prioritized in prepared topic menus so the test chain is visible without removing existing placeholder content.

## Scene changes

- `Day_Start` starts `Loop_DayOpening`.
- `Decision_Morning` and `Decision_Evening` start `Loop_Decision`.
- Munir conversation scenes again Auto Start `Start_Munir`.
- Ponder scenes continue to Auto Start `Ponder_Start`.
- `Day_End` starts `Loop_DayEnding`.
- Ponder scenes contain no duplicate `GameSession`.

## Files modified in this revision

- `Scripts/Presentation/Runtime/GameLoopService.cs`
- `Scripts/Presentation/Runtime/GameSession.cs`
- `Scripts/Presentation/Runtime/GameLoopYarnBridge.cs`
- `Scripts/Editor/GameLoopServiceEditor.cs`
- `Scripts/Editor/HeartOfPrinceSceneBuildInstaller.cs`
- `Scripts/Domain/State/GameLoopState.cs`
- `Scripts/Application/Conversation/ConversationService.cs`
- `Scripts/Application/Ponder/PonderService.cs`
- `Yarn/YarnSpinner2/GameLoop/PrototypeGameLoop.yarn`
- `Scenes/Bootstrap/Bootstrap.unity`
- `Scenes/DayLoop/Day_Start.unity`
- `Scenes/DayLoop/Day_End.unity`
- `Scenes/Decision/Decision_Morning.unity`
- `Scenes/Decision/Decision_Evening.unity`
- `Scenes/Dialogue/Mosque/Conversation_Munir_Morning.unity`
- `Scenes/Dialogue/Mosque/Conversation_Munir_Evening.unity`
- `README_GameLoopService.md`
- `IMPLEMENTATION_SUMMARY.md`

## Static validation performed

- Verified every runtime scene name exists.
- Verified active scene Starting Nodes resolve to Yarn nodes.
- Verified all narrative scenes have Auto Start enabled.
- Verified no Dialogue Runner is marked persistent by runtime code.
- Verified only `GameSession` uses `DontDestroyOnLoad`.
- Verified all key TopicHub, TopicHubNPC, PonderHub, completion, and progression nodes exist.
- Verified all added or modified C# files have balanced braces.
- Verified Unity asset GUIDs and scene component references remain structurally valid.

Unity compilation and Play Mode execution still require the host Unity project and its installed packages.

## Chapter and act narrative definitions

- Implemented serializable `Chapter` and `Act` configuration classes.
- Added polymorphic `CompletionCondition` rules with demo implementations for completed days and all acts completed.
- Added `DemoChapterDefinition`, which defines one chapter containing one two-day act with two decisions per day.
- Updated `GameLoopService` to read scene routing, decision counts, act count, and completion behavior from the active definitions.
- Added the chapter and act start/end scenes to the full runtime flow and Build Settings installer.
- Added dedicated Yarn sequence nodes and commands for chapter start, act start, act end, and chapter end.
- Kept mutable progression in `GameLoopState`.

