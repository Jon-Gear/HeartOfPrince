# Narrative Chapter and Act Structure

The demo now uses code-authored `Chapter` and `Act` definitions.

## Files

- `Scripts/Domain/Chapter/Chapter.cs`
- `Scripts/Domain/Chapter/Act.cs`
- `Scripts/Domain/Chapter/CompletionCondition.cs`
- `Scripts/Domain/Chapter/DemoChapterDefinition.cs`

`Chapter` and `Act` are serializable reference types. They contain configuration only: identity, scene names, decisions per day, and completion conditions. Runtime values such as the current act, day, and decision index remain in `GameLoopState`.

## Demo definition

`DemoChapterDefinition.Create()` constructs:

- Chapter 1
  - Start scene: `Chapter_1_Start`
  - End scene: `Chapter_1_End`
  - One act
- Act 1
  - Start scene: `Act_1_Start`
  - End scene: `Act_1_End`
  - Day start: `Day_Start`
  - Day end: `Day_End`
  - Decision scenes: `Decision_Morning`, `Decision_Evening`
  - Two decisions per day
  - Completion: two completed days

The chapter uses `AllActsCompletedCondition`.

## Extending completion rules

Create another subclass of `CompletionCondition` and implement `IsMet`. The supplied `NarrativeProgress` includes the complete `GameState`, completed days in the current act, completed acts in the chapter, and total act count. This allows future conditions based on relationships, discussed topics, flags, or combinations of rules.

## Runtime flow

`GameLoopService` reads all structural values from the active definitions. It no longer owns separate `daysPerAct`, `actsInDemo`, or `decisionsPerDay` configuration fields.
