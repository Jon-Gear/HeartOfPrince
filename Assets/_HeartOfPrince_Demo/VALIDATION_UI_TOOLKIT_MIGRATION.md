# UI Toolkit Migration Validation

## Automated structural checks

- C# delimiter scan passed for all 64 source files.
- Both UXML files parse as valid XML.
- The shared USS and default runtime TSS have balanced syntax.
- The serialized Panel Settings asset references the included default runtime
  theme and is cloned at runtime; no `CreateInstance<PanelSettings>` call
  remains.
- The new `DialogueSystem.prefab` and every scene have unique YAML object IDs.
- All local YAML `fileID` references resolve.
- Every prefab override targets an object that exists in the new prefab.
- No runtime `OnGUI` implementation remains in the Presentation layer.
- No legacy Yarn dialogue prefab, `LineView`, `OptionsListView`, or
  `DialogueViewBase` reference remains in scenes or prefabs.
- `Bootstrap.unity` remains unchanged because it has no `DialogueRunner`.

## Scene replacement coverage

The new prefab is referenced by all 11 scenes that contain dialogue:

| Scene | Preserved start node | Auto Start |
|---|---|---:|
| Chapter_1/Act_1/Act_1_End | `Loop_ActEnding` | On |
| Chapter_1/Act_1/Act_1_Start | `Loop_ActOpening` | On |
| Chapter_1/Chapter_1_End | `Loop_ChapterEnding` | On |
| Chapter_1/Chapter_1_Start | `Loop_ChapterOpening` | On |
| DayLoop/Day_End | `Loop_DayEnding` | On |
| DayLoop/Day_Start | `Loop_DayOpening` | On |
| Decision/Decision | `Loop_Decision` | On |
| Dialogue/Mosque/Conversation_Munir_Evening | `Start_Munir` | On |
| Dialogue/Mosque/Conversation_Munir_Morning | `Start_Munir` | On |
| Pondering/Ponder_Evening | `Ponder_Start` | On |
| Pondering/Ponder_Morning | `Ponder_Start` | On |

All 11 scenes retain the supplied Yarn project reference.

The two Munir scenes previously contained an active dialogue hierarchy plus an
inactive duplicate presentation hierarchy. The migration preserves the active
`Start_Munir` runner and its `SceneDirector` reference while removing the
inactive legacy duplicate.

## Editor validation still required

The supplied archive is an Assets subtree and does not contain
`Packages/manifest.json`, `ProjectSettings`, or a Unity Editor installation.
Compilation, package-version verification, visual inspection, and Play Mode
testing must therefore be completed in the host Unity project. Unity will also
generate importer metadata for the new UXML and USS files on first import.
