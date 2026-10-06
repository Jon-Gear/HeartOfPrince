---
name: write-topic
description: Write one Heart of Prince topic node (talk or ponder) and land it as a .yarn file.
disable-model-invocation: true
---

Write one topic node — the `.yarn` file for a single talk or ponder. The mechanics (whitelist,
anatomy, naming, storage, self-check) live in
`Assets/_HeartOfPrince_Demo/Yarn/YarnSpinner/docs/CONTEXT.md`, and each character's voice and
memory in `Assets/_HeartOfPrince_Demo/Yarn/YarnSpinner/docs/<Character>_Character_File.md`. Read
both before writing a line.

## 1. Read the brief

Pin the topic to one subject. State it as "Talk about [X]" or "Ponder about [X]", and confirm the
character (talk) or none (ponder), the direction (Prince-Raised or Character-Raised; none for
ponder), and any topics this one should unlock. Ask for anything the request left out.

Completion: the topic statement is pinned with character and direction, and the unlock list
(possibly empty) is settled.

## 2. Find the well

Read the character file of everyone in the scene — Prince and the other character for a talk,
Prince alone for a ponder. Find the well: the Memory Index entry the subject draws from. If no
entry matches, flag the gap and write against what exists — never invent a memory.

Completion: you can name the well (or the gap), and you hold each speaker's Voice section in mind.

## 3. Beat the subject

Before any prose, list the topic's beats in order: the entry beat, one beat per speaker swap, a
hook beat where each unlock fires, and a closing beat that resolves the subject.

Completion: every unlock from step 1 is pinned to a beat, and the closing beat resolves the
subject.

## 4. Write it

Turn each beat into lines, one breath per line, holding every line against its speaker's Voice
section — the "would they say that?" test. Ground every off-hand reference in a Memory Index entry.
Place each unlock on its hook line — the line or option that speaks the new subject.

Completion: every beat has prose, every reference traces to a Memory Index entry, and every unlock
sits on its hook line.

## 5. Land it

Follow CONTEXT.md §7–§10: PascalCase title reading as the topic, the `when:` header for talk
topics (none for ponder), a SetShot change on every speaker swap, and the file in its folder
(`<Character>/PlayerToCharacter/`, `<Character>/CharacterToPlayer/`, or `Ponder/`). Run the
self-check in CONTEXT.md §12 against the finished file.

Completion: the file is in place and the self-check passes, including every Unlock* target matching
a real node title.
