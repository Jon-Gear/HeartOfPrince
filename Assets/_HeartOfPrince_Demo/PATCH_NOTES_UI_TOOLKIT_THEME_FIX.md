# UI Toolkit Theme and Dialogue Appearance Fix

## Root cause

The first migration constructed `PanelSettings` with
`ScriptableObject.CreateInstance<PanelSettings>()`. Unity validates the panel
at construction time, before the next line can assign a Theme Style Sheet.
That produced:

`No Theme Style Sheet set to PanelSettings, UI will not render properly`

## Fix

- Added `HeartOfPrincePanelSettings.asset`.
- Added `UnityDefaultRuntimeTheme.tss`, importing
  `unity-theme://default`.
- `HeartOfPrinceUIToolkit.CreateDocument` now clones the configured Panel
  Settings asset rather than constructing a blank panel.
- Kept a separate cloned panel per presenter so dialogue and Decision UI can
  retain independent sorting.
- Restyled dialogue to closely reproduce the legacy Yarn Spinner presentation
  found in the supplied scenes.
- Prevented Enter/Space handled by the dialogue overlay from swallowing Yarn
  option-button activation.

## Import

Copy the supplied `_HeartOfPrince_Demo` folder over the existing
`Assets/_HeartOfPrince_Demo` folder and allow Unity to reimport. The obsolete
`HeartOfPrinceRuntimeTheme.tss` from the first patch can be deleted; it is no
longer referenced.

The source upload did not include the full Unity project or Unity Editor, so
Play Mode verification remains required in the host project.
