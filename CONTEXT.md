# Heart of Prince

Heart of Prince is a narrative game about Prince's inner and relational development. Its current domain centers on talking, pondering, and topic progression, with activities acting as pacing rather than the primary game.

## Language

**Prince**:
The protagonist whose inner state, responsibilities, relationships, and future are the game's main subject.

**Talk**:
An activity where Prince speaks with another character and advances conversation topics.
_Avoid_: Chat, dialogue activity

**Ponder**:
An activity where Prince reflects alone and advances personal reflection topics.
_Avoid_: Think, monologue

**Topic**:
A conversation subject that can become available to talk about or ponder, then becomes discussed once selected in an activity. Topics are the primary unit of progression in Heart of Prince and can open up other topics.
_Avoid_: Quest, prompt

**Topic Progression**:
The movement of topics from unavailable, to available, to discussed across talks and pondering.
_Avoid_: Dialogue tree completion, quest progress

**Topic Milestone**:
A topic or set of topics whose discussion marks meaningful narrative progress, such as completing an act.
_Avoid_: Day-count gate

**Prince-Raised Topic**:
A talk topic Prince brings to another character.
_Avoid_: Player-to-character topic

**Character-Raised Topic**:
A talk topic another character brings to Prince.
_Avoid_: Character-to-player topic

**Character**:
Someone Prince can talk to. A character may have a narrative role, such as mentor or guide, but the domain model should not make one character's role special unless the game needs it.
_Avoid_: NPC, quest giver

**Character Relationship**:
A character-specific state that records how another character responds to Prince. Relationships can gate or color topics, but they are not the primary progression track.
_Avoid_: Relationship route, affinity grind

**Activity**:
Something Prince chooses to spend time on. Talking and pondering are the current main activities; future activities may be added without replacing topic progression as the primary game.
_Avoid_: Action, task
