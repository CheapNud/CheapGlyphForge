<!--
  TODO.md — CheapGlyphForge project work tracker
  Last updated: 2026-07-23

  RULES FOR AI AGENTS:
  - Update the "Last updated" date above whenever you modify this file
  - Items use checkbox format: - [ ] incomplete, - [x] complete
  - Never remove completed items — they serve as history. Move them to "## Done" when a category gets cluttered.
  - Each item gets ONE line. Details go in sub-bullets indented with 2 spaces.
  - Prefix each item with the date it was added: - [ ] (2026-03-17) Description
  - When completing, change to: - [x] (2026-03-17 → 2026-03-18) Description
  - Tag the SOURCE of each item at the end in brackets:
      [code-todo] = from // TODO comment in source code
      [plan] = from a plan document or planning session
      [bug] = from a bug encountered during dev/deploy
      [audit] = from a code audit or review
      [user] = explicitly requested by the user
  - For [code-todo] items, ALWAYS include file:line reference so devs can navigate directly
  - Categories: Blocking, Planned, Future, Done
  - New items go at the TOP of their category
  - Do not create separate TODO_*.md files — everything goes here
  - Keep it terse. If it needs more than 3 sub-bullets, link to a plan document.
  - Do NOT create, rename, or remove categories — the fixed set is: Blocking, Planned, Future, Done
  - When asked for planned work or TODO analysis, ALWAYS include Future items too — list them below Planned and note them as future work
-->

# TODO

## Blocking

_Nothing blocking._

## Planned

- [ ] (2026-07-23) Fix sequence keyframe UX: AddKeyframeAtCurrentTime always adds at t=0 when not playing [audit]
  - Playback needs >=2 keyframes, so sequences can't be built through normal use
  - Add a time scrubber usable while paused so keyframes land at the scrub position
- [ ] (2026-07-23) Verify/fix _activeTabIndex desync in OnDeviceChanged when glyph tab is conditionally hidden (Phone 3) [audit]
- [ ] (2026-07-23) Advanced mode: sub-zones (C1-C4, D1-D8) have sliders but no visual representation; label says 13 zones but provider generates 14 [audit]
- [ ] (2026-07-23) Expose new devices from SDK 25111 in DeviceDetector + GlyphChannelInfoProvider [plan]
  - Common now has Is24111, Is25111, Is25111p, Is25131 (Phone 3 / 4a / 4a+ / 4b)
- [ ] (2026-07-23) Decide fate of GlyphInterface.Binding: unified aar duplicates the matrix binding's classes; MAUI only references GlyphMatrix.Binding [plan]

## Future

- [ ] (2026-07-23) Explore new Com.Nothinglondon.Text SDK surface (Font, ScrollingText) for matrix text rendering [plan]
- [ ] (2026-07-23) Update Home.razor: still says ".NET 9.0 Runtime" / "Blazor Hybrid" [audit]
- [ ] (2026-07-23) Sequence interpolation truncates instead of rounds in GetStateAtTime [audit]
- [ ] (2026-07-23) Change ApplicationId from template default com.companyname.cheapglyphforge.maui before store distribution [audit]
- [ ] (2026-07-23) Bump MAUI packages to 11.0.0-preview.6 once VS ships .NET 11 SDK preview.6 (preview.6 packages fail on preview.1 workloads) [bug]
- [ ] (2026-07-23) Retest AAB packaging on future SDK updates — CheapGlyphForge.MAUI.csproj:59 [code-todo]

## Done

_Nothing done yet._
