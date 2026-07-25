# UI Toolkit Dialogue Migration

## What changed

- Added `Prefabs/DialogueSystem.prefab`.
- Added `UIToolkitDialoguePresenter`, a Yarn Spinner 3 `DialoguePresenterBase`
  implementation that presents lines, typewriter text, continue input, and
  options using UI Toolkit.
- Added runtime UXML and USS under
  `Resources/HeartOfPrince/UI`.
- Replaced the legacy Yarn Spinner dialogue prefab in every scene that used
  it.
- Replaced the two unpacked dialogue systems in the Munir conversation scenes
  while preserving their `SceneDirector` references.
- Replaced `DecisionScenePresenter.OnGUI` with a UI Toolkit activity screen.
- Dialogue and decision screens share the same card, typography, button,
  focus, and disabled-state styling.
- Restyled dialogue to closely match the supplied legacy Yarn Spinner view:
  an opaque black bottom panel, white 32 px speaker name, white 40 px line
  text, plain options, and a small continue indicator.
- Added a serialized Panel Settings asset with Unity's default runtime theme.

## Scene coverage

The new prefab is installed in all 11 scenes that contain a
`DialogueRunner`:

- Chapter opening and ending
- Act opening and ending
- Day opening and ending
- Decision
- Morning and evening pondering
- Morning and evening Munir conversations

`Bootstrap.unity` does not contain dialogue and was intentionally left
without a `DialogueSystem` instance.

Each scene keeps its original Yarn project reference, start node, and
Auto Start value.

## Runtime structure

`DialogueSystem.prefab` contains:

- `DialogueRunner`
- `InMemoryVariableStorage`
- `UIToolkitDialoguePresenter`

The presenter creates a runtime `UIDocument` and clones the serialized
`Resources/HeartOfPrince/UI/HeartOfPrincePanelSettings.asset`. That asset
already references `UnityDefaultRuntimeTheme.tss`, avoiding Unity's warning
that occurs when an unthemed `PanelSettings` is constructed in code. The
visual tree is loaded from Resources so it can be edited in UI Builder without
changing the presenter code.

The Decision Scene still uses the existing data-driven
`ActivityQueryService` and `GameLoopService`; only its presentation layer was
replaced.

## Styling

Edit:

`Resources/HeartOfPrince/UI/HeartOfPrinceRuntime.uss`

The shared classes are used by both dialogue and decision UI. The dialogue
document sorts above the decision document, so the short `Loop_Decision`
narration appears first and the activity choice panel remains underneath.

## Suggested Unity validation

1. Allow Unity to import the new UXML, USS, TSS, and Panel Settings assets.
2. Open `Prefabs/DialogueSystem.prefab` and confirm the three components are
   present.
3. Play each scene directly and through the Bootstrap flow.
4. Verify mouse, keyboard Enter/Space, disabled decisions, Yarn options,
   typewriter hurry-up, and scene transitions.
5. Check the Console for missing Resources or UXML element-name errors.

The upload did not include `Packages`, `ProjectSettings`, or a Unity editor
binary, so the migration was validated structurally rather than by running
the project in the Unity Editor.
