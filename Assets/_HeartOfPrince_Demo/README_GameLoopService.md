# Heart of Prince — Game Loop Service Prototype

## Run the prototype

1. Import the `_HeartOfPrince_Demo` folder into the Unity project's `Assets` folder.
2. Allow Unity and Yarn Spinner to recompile the scripts and Yarn project.
3. Use **Heart of Prince > Open Starting Scene**, or open `Scenes/Bootstrap/Bootstrap.unity`.
4. Enter Play Mode.

`HeartOfPrinceSceneBuildInstaller` automatically adds the demo scenes to Build Settings. The same operation can be run manually from **Heart of Prince > Rebuild Demo Scene List**.

The opening dialogue starts automatically. Continue through the Yarn lines, select **Talk to Munir** or **Ponder**, and complete two decisions per day. The default prototype contains two acts with two days in each act.

## Architecture

`GameSession` remains the single persistent composition root. It owns the persistent `GameState`, conversation service, ponder service, exploration service, and `GameLoopService`.

`GameLoopService` is an explicit state machine. It owns:

- Current act, day, and decision index.
- The configurable decisions-per-day value.
- Action-running, day-ending, and completion flags.
- Scene transitions and the next Yarn node.
- Day and act progression.
- Demo completion.
- A single persistent Yarn `DialogueRunner` and EventSystem.

Scenes do not choose the next scene. Yarn commands report the player's action choice or sequence completion to the service, and the service validates the transition.

Talk destinations are data-driven through the `talkRoutes` list on `GameLoopService`. The included route maps Munir to `Conversation_Munir_Evening` and `Loop_Talk_Munir`; another character can be added by supplying another route and Yarn node without changing the day/decision state machine.

The existing `Conversation_Munir_Evening` dialogue system is reused as a persistent narrative host. The service preserves its Yarn runner and UI across scene changes. Talk loads the existing Munir conversation scene. Ponder loads the existing morning/evening Ponder scene. Day opening, decision, and day-ending scenes use their existing scene files.

## Default loop

```text
Bootstrap
  -> day opening Yarn node
  -> decision Yarn menu
      -> Talk scene and Yarn node
      -> or Ponder scene and Yarn node
  -> next decision
  -> end-of-day Yarn node
  -> next day or act transition
  -> demo ending
```

Default configuration on the Bootstrap `GameLoopService` component:

- Decisions per day: 2
- Days per act: 2
- Acts in demo: 2

These values can be changed in the Bootstrap scene Inspector.

## Topic progression

The prototype adds a deterministic topic chain:

```text
Talk: ask Munir about responsibility
  -> unlock PrototypePonderResponsibility
Ponder: reflect on responsibility versus control
  -> unlock PrototypeAskAboutLeadership
  -> unlock a later question from Munir
Talk again
  -> discuss the deeper topic
```

Available and discussed Talk topics are retained in `CharacterTopicState`. Available and discussed Ponder topics are retained in `PonderTopicState`. Munir's relationship trust is retained in `CharacterRelationshipState`.

The original random topic services remain available. Selecting a topic through those services now marks it discussed instead of simply deleting all history.

## Yarn commands and functions

Loop commands:

- `<<loop_choose_talk "Munir">>`
- `<<loop_choose_action "Talk">>` (defaults to Munir)
- `<<loop_choose_action "Ponder">>`
- `<<loop_action_complete>>`
- `<<loop_sequence_complete>>`
- `<<loop_new_game>>`

Loop functions:

- `loop_current_act()`
- `loop_current_day()`
- `loop_decision_number()`
- `loop_decisions_per_day()`
- `loop_is_complete()`

Topic and relationship commands:

- `<<UnlockPonderTopic "NodeName">>`
- `<<MarkPonderTopicDiscussed "NodeName">>`
- `<<UnlockConversationTopic "Munir" "PlayerToCharacter" "NodeName">>`
- `<<MarkConversationTopicDiscussed "Munir" "PlayerToCharacter" "NodeName">>`
- `<<ChangeRelationship "Munir" 1>>`

Topic and relationship functions:

- `HasPonderTopic("NodeName")`
- `HasDiscussedPonderTopic("NodeName")`
- `HasConversationTopic("Munir", "PlayerToCharacter", "NodeName")`
- `HasDiscussedConversationTopic("Munir", "PlayerToCharacter", "NodeName")`
- `RelationshipTrust("Munir")`

Compatibility commands were also added for the existing `AddPlayerToCharacterTopic`, `AddCharacterToPlayerTopic`, `RemovePlayerToCharacterTopic`, and `RemoveCharacterToPlayerTopic` Yarn calls.

## Debugging

During Play Mode, select the persistent `GameSession` object and inspect its `GameLoopService` component. The custom Inspector shows the current phase, act, day, decision index, and status flags.

Debug buttons provide:

- Start New Game
- Reset All Progression
- Skip To Next Day

The service also logs all important phase changes, scene loads, action resolutions, topic unlocks, and relationship changes with `[GameLoop]`, `[Topics]`, or `[Relationship]` prefixes.

The existing **Heart of Prince > Game Session Runtime Monitor** remains available for inspecting live topic state.

## Notes

The demo uses placeholder narrative text and the existing Yarn UI. It deliberately centralizes flow control and does not require character models, animation, or finished environments.

The supplied archive is an Assets subfolder rather than a complete Unity project. Unity itself is not included in the archive, so the final import/compile and Play Mode pass must be performed in the host Unity project with its existing Yarn Spinner and Cinemachine packages.
