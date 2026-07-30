# Heart of Prince Current Design

## Purpose

This document captures the current domain and design direction for Heart of Prince as it develops beyond the present demo. It is intentionally higher-level than code architecture: it records what the game is about, what progression means, and which systems should remain central.

## Current Center

Heart of Prince revolves around talking, pondering, and topic progression.

Activities pace the experience by limiting how many meaningful opportunities Prince can spend in a day. More activity types may be added later, but the current main loop is built around Prince talking with characters and pondering alone.

## Primary Progression Unit

Topics are the primary unit of progression.

A topic is a conversation topic, usually represented in authored content by a dialogue node. A topic can be unavailable, available, or discussed. Talks and pondering consume available topics and may unlock future topics. Scenes present this progression, relationships can react to it, and activities/days pace it, but none of those are currently the primary progression unit.

For now, one topic maps to one authored dialogue node. If a future topic needs multiple variants, that can be introduced later without changing the current progression language.

Topics are non-repeatable by default. Once a topic is discussed, it leaves the available pool. If Prince revisits the same subject later, that should usually be a new topic representing the changed context.

## Talk Topic Direction

Talk topics distinguish who raises the subject in-world:

- Prince-raised topics are subjects Prince brings to another character.
- Character-raised topics are subjects another character brings to Prince.

The design language should avoid treating the player as a separate in-world participant. Code may still use older `PlayerToCharacter` / `CharacterToPlayer` names until renamed, but design discussion should use Prince-raised and character-raised.

Prince-raised topics are usually selected by the player from available prepared topics. Character-raised topics may be selected by the character/system from that character's available topics.

Talk topics are character-specific. If Prince can discuss a similar subject with multiple characters, each character should have a separate topic because the content and consequences differ.

## Ponder Topics

Ponder topics use the same topic lifecycle as talk topics.

The distinction is context, not progression mechanics: Prince is alone, so there is no other character and no raised-by direction. A ponder topic can still open future talk or ponder topics.

## Topic Turns

One Talk or Ponder activity can contain multiple topic turns.

The activity is the time block. Inside that block, Prince may discuss or ponder a small number of topics before the activity completes and time advances.

## Topic Unlocks

For the next stage, topic unlocks remain authored in Yarn and initial presets rather than moving into a declarative topic graph.

A talk or ponder can explicitly open later Prince-raised, character-raised, or ponder topics. This keeps content authoring simple while the shape of the topic system is still emerging.

For now, topic availability is only the available topic pool. If trust, day, flags, or story state matter, authored content should open the topic only when those conditions are satisfied. Reusable topic conditions can be introduced later if authored unlocks become difficult to manage.

## Topic Consequences

Using a topic only guarantees that the topic becomes discussed.

Other consequences are optional authored effects. A topic may unlock future topics, change trust, set flags, or contribute to act progress, but lightweight reflection topics do not need forced downstream effects.

## Relationships

Relationship state is secondary to topic progression.

Relationships record how a character responds to Prince. They may gate or color specific topics and may react to what Prince says, but they should not become the primary progression track or turn the game into a trust grind.

## Characters

Characters are people Prince can talk to.

Munir is currently the only configured talk target. Narratively, he is a mentor/guide figure, but the domain model should treat him as a normal character because future characters will also be talk targets with their own topics and relationships.

## Day Loop

A day limits meaningful activities.

Prince wakes up, chooses an activity, performs it, time passes, and he returns to the decision scene. This repeats until the day hours end. The final game may divide the day into more arbitrary time periods, such as 8:00 or later slices, where choosing an activity progresses the day.

## Future Activities

Future activity types should feed topic progression.

An activity may have its own presentation or local interaction, but it should reveal, unlock, transform, or contextualize topics. Future activities should not become isolated minigames with separate progression tracks unless that decision is made deliberately later.

## Act Completion

Acts should complete through topic milestones.

The current demo uses a two-day completion condition, but that is scaffolding. In the final design, an act should complete when Prince has discussed the required topics for that act's narrative movement.

## Current Demo Shape

The current playable configuration is Chapter 1, Act 1, with a short day loop. Each day currently allows two activities. The available activity types are Talk and Ponder. Talk currently targets Munir, and Ponder uses Prince's personal reflection topics.

## Recommended Next Development

Topic milestone completion should be the next development priority.

The current day-count act completion should be treated as demo scaffolding. Replacing it with topic milestone completion would align the code with the core design: topics are primary, and days pace topic opportunities.

## Open Questions

- None yet.
