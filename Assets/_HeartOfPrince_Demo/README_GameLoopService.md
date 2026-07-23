# Heart of Prince — Game Loop Service Prototype

## Running the complete prototype

1. Import the `_HeartOfPrince_Demo` folder into the Unity project's `Assets` folder.
2. Allow Unity and Yarn Spinner to recompile the scripts and Yarn project.
3. Open `Scenes/Bootstrap/Bootstrap.unity`.
4. Enter Play Mode.

`HeartOfPrinceSceneBuildInstaller` adds all demo scenes to Build Settings. You can also run **Heart of Prince > Rebuild Demo Scene List** manually.

The default prototype contains two acts, two days per act, and two decisions per day. These values are configurable on the `GameLoopService` component in the Bootstrap scene.

## Scene-local Dialogue Runners

Dialogue Runners are deliberately **not persistent**.

Each narrative scene owns its Dialogue Runner, dialogue UI, EventSystem, Yarn Project reference, Auto Start setting, and Starting Node. The runner is destroyed when its scene unloads.

Configured scene entry nodes:

| Scene | Starting Node |
|---|---|
| `Day_Start` | `Loop_DayOpening` |
| `Decision_Morning` | `Loop_Decision` |
| `Decision_Evening` | `Loop_Decision` |
| `Conversation_Munir_Morning` | `Start_Munir` |
| `Conversation_Munir_Evening` | `Start_Munir` |
| `Ponder_Morning` | `Ponder_Start` |
| `Ponder_Evening` | `Ponder_Start` |
| `Day_End` | `Loop_DayEnding` |

`GameLoopService` sets the loop phase before loading a scene and waits for that scene's runner to report completion. It does not call `StartDialogue` and does not preserve or replace a scene's runner.

## Existing Talk and Ponder hubs

Talk uses the existing flow:

```text
Start_Munir
  -> TopicHub
  -> PrepareTopicHubNPC / TopicHubNPC
  -> End
```

Ponder uses the existing flow:

```text
Ponder_Start
  -> Ponder_TopicHub
  -> Ponder_End
```

The loop service only loads the correct scene and records when the complete action ends.

## Topic progression demonstration

Prototype topics are presented before placeholder topics so the chain is easy to test, while the existing placeholder topics remain available.

```text
Talk: PrototypeAskAboutResponsibility
  -> unlocks PrototypePonderResponsibility

Ponder: PrototypePonderResponsibility
  -> unlocks PrototypeAskAboutLeadership
  -> unlocks PrototypeMunirQuestion

Talk again:
  -> PrototypeAskAboutLeadership appears in TopicHub
  -> PrototypeMunirQuestion is available through TopicHubNPC
```

Selecting a Talk or Ponder topic moves it from available topics to discussed-topic history. The history, available topics, and Munir relationship state live in the persistent `GameSession`.

## Playing an individual scene directly

Open any demo scene other than Bootstrap and enter Play Mode.

If the Unity project has a global Play Mode Start Scene configured, select **Heart of Prince > Debug > Play Current Open Scene** first. **Play Full Game From Bootstrap** restores the explicit Bootstrap launch option.

A single `GameSession [Direct Scene Debug]` object is created automatically only when no scene or persistent session already exists. The loop enters standalone-scene mode instead of starting a new game or redirecting to Bootstrap.

Standalone behavior:

- Day opening and day ending scenes play their own starting node and remain loaded afterward.
- Conversation scenes start `Start_Munir` and use `TopicHub`/`TopicHubNPC`.
- Ponder scenes start `Ponder_Start` and use `Ponder_TopicHub`.
- Decision scenes show the normal decision menu and may load the selected Talk or Ponder scene.
- Completing a standalone action stops progression in that action scene instead of returning to Bootstrap.
- **Start New Game** exits standalone mode and begins the complete game loop.
- **Reset All Progression** resets and reloads the currently tested standalone scene.

Standalone mode seeds temporary Munir and Ponder topics so the hubs can be exercised without first playing Bootstrap.

## Persistent architecture

`GameSession` is the only persistent runtime composition root. It owns:

- `GameState`
- `ConversationService`
- `PonderService`
- `ExplorationService`
- `GameLoopService`

Only state and services persist. Scene presentation objects do not.

`GameLoopService` owns:

- Current act, day, and decision index
- Configurable decisions per day
- Current loop phase
- Action-running, day-ending, and completion flags
- Scene selection and scene transitions
- Day and act progression
- Talk routes for future characters
- Full-game versus standalone-scene launch mode

## Default full-game flow

```text
Bootstrap
  -> Day_Start / Loop_DayOpening
  -> Decision scene / Loop_Decision
      -> Munir scene / Start_Munir / TopicHub
      -> or Ponder scene / Ponder_Start / Ponder_TopicHub
  -> next Decision scene
  -> Day_End / Loop_DayEnding
  -> next day or act
  -> Day_End / Loop_DayEnding in ending phase
  -> Completed
```

## Yarn completion commands

- `<<loop_choose_talk "Munir">>`
- `<<loop_choose_action "Ponder">>`
- `<<loop_action_complete>>`
- `<<loop_sequence_complete>>`
- `<<loop_new_game>>`

`End_Munir.yarn` calls `loop_action_complete` after `EndConversation`.

`Ponder_End.yarn` calls `loop_action_complete` after `EndPonder`.

Day opening and ending nodes call `loop_sequence_complete`.

## Topic and relationship commands

- `<<UnlockPonderTopic "NodeName">>`
- `<<UnlockConversationTopic "Munir" "PlayerToCharacter" "NodeName">>`
- `<<UnlockConversationTopic "Munir" "CharacterToPlayer" "NodeName">>`
- `<<MarkPonderTopicDiscussed "NodeName">>`
- `<<MarkConversationTopicDiscussed "Munir" "PlayerToCharacter" "NodeName">>`
- `<<ChangeRelationship "Munir" 1>>`

Existing compatibility commands such as `AddPlayerToCharacterTopic`, `AddCharacterToPlayerTopic`, and the remove-topic commands remain available.

## Debugging

Select the persistent `GameSession` object during Play Mode. The custom `GameLoopService` Inspector shows:

- Active scene
- Full-game or standalone mode
- Current phase
- Act
- Day
- Decision index
- Action, day-ending, and completion flags

Debug buttons:

- **Start New Game**
- **Reset All Progression**
- **Skip To Next Day** — full-game mode only

Important transitions are logged with `[GameLoop]`, `[Topics]`, and `[Relationship]` prefixes.

## Validation note

The supplied archive is an Assets subfolder, not a complete Unity project. The host Unity executable and package manifest were not included, so final compilation and Play Mode validation must be performed in the host project with its installed Yarn Spinner and Cinemachine packages.
