# Narrative and Schedule Structure

`Chapter` and `Act` remain ScriptableObject definitions. Runtime progression remains in `GameState`.

An `Act` now references:

- start and end scenes;
- one reusable day-start scene;
- one reusable decision scene;
- one day-end scene;
- a `DayRules` asset;
- a completion condition.

Decision scenes are no longer stored in an array by decision index. Time is represented by `WorldClockState`, and activity scene variants are resolved from the current clock.

The demo contains one chapter and one act. The act completes after two days. Each day starts at 08:00 and permits two activities.
