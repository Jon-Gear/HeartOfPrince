# Heart of Prince — YarnSpinner Dialogue Writing Context

This file is the instruction set for writing YarnSpinner dialogue for Heart of Prince.
Its sole job: **write new topic nodes** for talks and ponders. Nothing else.

## 1. Purpose

- Write **topic nodes** only: the `.yarn` nodes that the talk and ponder hubs detour into.
- A topic is one subject of discussion. Every topic must read as "Talk about [X]" (talk topic)
  or "Ponder about [X]" (ponder topic).
- Topics are the primary unit of progression. Discussing a topic can unlock other topics.

## 2. Domain language → Yarn mechanics

Use the game's domain language when writing:

- **Talk** = a conversation activity between Prince and another character.
- **Ponder** = Prince reflecting alone.
- **Topic** = a subject, the unit of progression.
- **Prince-Raised Topic** = `PlayerToCharacter` direction (Prince brings it up).
- **Character-Raised Topic** = `CharacterToPlayer` direction (the other character brings it up).

When asked to write a "character-raised topic", that means a `CharacterToPlayer` node in that
character's folder. When asked for a "prince-raised topic", that means a `PlayerToCharacter` node.

## 3. Scope boundary — never touch game mechanics

The following are game mechanics. Do not write, edit, or reconstruct them:

- Talk hubs (`TopicHub`, `TopicHubNPC`, `PrepareTopicHubNPC`) and their Start/End/Greet nodes.
- The Ponder hub and flow (`Ponder_Start`, `Ponder_TopicHub`, `Ponder_End`).
- The game loop, chapter, act, and day nodes (`Loop_*`, `Chapter_*`, `Act_*`).
- Turn accounting, topic selection, activity scenes, UI.

You only author **topic nodes** that fit inside these systems.

## 4. Character files

- Per-character voice and background live in `docs/<Character>_Character_File.md` (filled in by the
  project owner, not by the assistant). The file format is defined in
  `docs/Character_File_Standard.md` — Identity, Voice, Mannerisms, Traits, Relationships, Memory Index.
- **Before writing any line for a character, read that character's file.**
- If a character's file is missing or empty, write nothing and flag it first.
- **Ground every reference.** Any off-hand reference a character makes must be grounded in that
  character's Memory Index. Never invent a memory the owner didn't author. If the file lacks the
  memory a hook needs, flag the gap and write the hook against what exists — do not fabricate
  backstory.

## 5. The command whitelist

These are the ONLY commands allowed inside a topic node:

- `<<SetShot "ShotName">>` — camera direction. One of the six shots (see section 8).
- `<<UnlockPonderTopic "topicNode">>` — opens a ponder topic.
- `<<UnlockConversationTopic "characterId" "PlayerToCharacter|CharacterToPlayer" "topicNode">>` —
  opens a talk topic for that character in that direction.
- `<<wait seconds>>` — used sparingly, only for dramatic pauses.

### How unlocks behave (facts to rely on)

- Selecting a topic auto-marks it discussed, auto-removes it from the available pool (so it can
  never be re-selected), and auto-decrements the turn budget. Do nothing manually.
- Unlocking an already-discussed topic is a silent no-op. It is fine to write the unlock anyway.
- The `Add*` and `Remove*` aliases are redundant: `Add*` is the same as `Unlock*`, and removal is
  handled automatically when a topic is discussed. **Never use them.**

## 6. Forbidden commands

Everything not in the whitelist is forbidden in a topic node, including:

- `AddPlayerToCharacterTopic`, `AddCharacterToPlayerTopic`, `RemovePlayerToCharacterTopic`,
  `RemoveCharacterToPlayerTopic`
- `MarkPonderTopicDiscussed`, `MarkConversationTopicDiscussed`
- `ChangeRelationship`
- `StartActivity`, `CompleteActivity`, `CompleteDayOpening`, `CompleteDay`,
  `CompleteChapterStart`, `CompleteActStart`, `CompleteAct`, `CompleteChapter`
- `StartConversation`, `StartConversationWithActor`, `EndConversation`, `PrepareTopics`,
  `TakeTurn`, `CountPlayerTurn`, `CountCurrentActorTurn`, `SelectTopic`, `SelectRandomTopic`
- `StartPonder`, `EndPonder`, `PreparePonderTopics`, `TakePonderTurn`, `CountPonderTurn`,
  `SelectPonderTopic`, `SelectRandomPonderTopic`
- All read functions (`IsCurrentActor`, `GetCurrentActor`, `TurnsLeft`, `HasPreparedTopic`,
  `TopicDisplayName`, `HasTopicsForCurrentActor`, `CanRefreshPreparedTopics`, turn counters,
  `HasPonderTopic`, `HasConversationTopic`, `RelationshipTrust`, `loop_current_*`, `CurrentTime`,
  `Actions*`, `MaximumActionsPerDay`)
- `<<if>>`, `<<elseif>>`, `<<else>>` — no conditionals in topic nodes.

## 7. Topic node anatomy

```
title: <TopicName>
when: IsCurrentActor("<character>")
---
<<SetShot "Wide">>
Prince: First line.
<<SetShot "CloseUp_A">>
Character: Reply.
Prince: A hook line.
<<UnlockPonderTopic "NewPonderTopic">>
<<SetShot "CloseUp_B">>
Character: Resolution.
===
```

- **Headers:** talk topics must carry `when: IsCurrentActor("<character>")` — this is what lets
  two characters share the same topic name without collision. Ponder topics carry no `when:`.
- **`when: once`** is not needed for normal topics — discussing a topic removes it automatically.
- **Length:** varies. A topic ends when its subject is logically resolved — short or long is fine.
- **Shots:** change `SetShot` on every speaker switch, mood shift, or new beat.
- **Unlocks:** place an unlock command immediately at the "hook" — the line or option that mentions
  the new subject (e.g., the character mentions their family → unlock the family topic right there).
  This also applies inside option blocks.
- **Options:** `->` choices are allowed for player interactivity and flavor. A specific option may
  unlock a topic. No `<<if>>` gating on options.
- **Ending:** the node ends with `===`. Control returns to the hub automatically.

## 8. SetShot conventions

Six shots. **A is always Prince, B is always the other character.**

- `Wide` — establishing; scene or topic entry.
- `TwoShot` — both in frame; a shared beat.
- `CloseUp_A` — close-up on Prince.
- `CloseUp_B` — close-up on the other character.
- `OTS_A` — over Prince's shoulder, looking at the other character.
- `OTS_B` — over the other character's shoulder, looking at Prince.

## 9. Naming

- Every topic title must read naturally as "Talk about [X]" or "Ponder about [X]".
  Example: `SecondChances` → "Talk about Second Chances"; `Duty` → "Ponder about Duty".
- PascalCase, no spaces, no "Topic" or character prefix (the `when:` header handles
  disambiguation).
- **Never** start a title with `Prototype` — the engine special-cases that prefix for
  selection priority (reserved for test topics).

## 10. Storage

- Talk topics: one file per topic, in the character's folder.
  - Prince-Raised: `<Character>/PlayerToCharacter/<TopicName>.yarn`
  - Character-Raised: `<Character>/CharacterToPlayer/<TopicName>.yarn`
- Ponder topics: one file per topic, directly in `Ponder/` — `<TopicName>.yarn`.
  Do not group multiple ponder topics into a single file.

## 11. Line formatting

- **One breath per line** (~60–80 characters maximum). A line must always fit in the dialogue box.
- Longer speeches are split across multiple lines (the speaker speaks again).
- Format: `Speaker: text` — `Prince:` for the protagonist, the character's name for everyone else.
- Curly apostrophes (`’`), never straight (`'`).
- No emoji, no markdown, no italicized stage directions.
- No `Narrator:` lines inside topics (that is game-loop narration).
- Option labels are short and spoken like choices (e.g., `-> Ask about his family`), with the
  follow-up dialogue indented under the option.
- `<<wait>>` only for a dramatic pause, used sparingly.

## 12. Self-check checklist

Run this against every topic node before handing it back:

1. `title:` reads naturally as "Talk about [X]" / "Ponder about [X]".
2. Talk topics carry `when: IsCurrentActor("<character>")`; ponder topics carry no `when:`.
3. Every `Unlock*` string matches an actual node `title:` in the project (this file's topic
   included).
4. `SetShot` value is one of the six; shots change on speaker switch / mood shift.
5. Every line fits one breath; longer speeches are split across lines.
6. Unlock commands sit at the hook (line or option), not batched at the end.
7. No forbidden commands (section 6).
8. Curly apostrophes, `Speaker:` format, no emoji/markdown/Narrator.
9. File lands in the correct folder (section 10).
