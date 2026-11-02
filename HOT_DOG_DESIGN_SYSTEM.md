# HOT DOG — Visual Design System

## Direction

HOT DOG is being shaped as a premium, dark-first game automation control center. The interface should feel purposeful, compact and modern rather than like a direct re-skin of the inherited client.

The visual direction combines:
- Deep charcoal / blue-black application surfaces.
- Warm orange as the primary interaction and selection colour.
- Mustard as the secondary brand accent.
- Clear cool-grey borders and readable neutral typography.
- Consistent cards, fields, buttons, menus, navigation and window chrome across Client and Manager.

The design must not reduce information density or hide existing controls simply to make the application look minimal.

## Brand palette

| Token | Value | Use |
|---|---|---|
| Primary accent | `#F97316` | Main actions, active states and focus |
| Primary hover | `#EA580C` | Hover state |
| Primary pressed | `#C2410C` | Pressed state |
| Secondary accent | `#FBBF24` | Secondary brand highlight |
| Dark page | `#0A0F15` | Main background |
| Dark surface | `#111923` | Main panels |
| Dark surface alt | `#172230` | Secondary panels and controls |
| Dark elevated surface | `#1C2938` | Raised cards and buttons |
| Dark border | `#29394A` | Panel and control borders |
| Dark text | `#F1F5F9` | Main text |
| Dark muted text | `#9AAABD` | Secondary text |

A corresponding cool-neutral light theme is provided. Status colours (success, warning, danger and information) remain semantically distinct from the HOT DOG brand accent.

## Component rules

- Client and Manager consume the same shared Avalonia design resources.
- Buttons use a consistent compact height, clear primary/secondary/danger variants, visible keyboard focus and explicit hover/pressed/disabled states.
- Text fields and combo boxes share the same height, border, radius and focus treatment.
- Cards, account rows, expanders and option panels use shared corner radii, surfaces and borders.
- Tabs and list navigation use the same active, hover and inactive states.
- Menu popups use the elevated surface, border and shadow tokens.
- Page layouts retain enough density for account lists, multi-client operations and automation tools.

## Current implementation

The first visual-foundation pass on `hot-dog-dev` includes:
- Rebuilt shared light/dark colour tokens in `Skua.Shared.Avalonia/Theming/Colors.axaml`.
- Standardised the shared button, toggle, text field, combo box, card, tab, menu, expander and list styling.
- Updated the shared title bar to a 40px shell with a HOT DOG monogram and consistent window controls.
- Changed Client top navigation and tab strip sizing/surfaces to match the new shell.
- Rebuilt the Manager navigation as a persistent left sidebar with a dedicated content region.
- Applied Segoe UI as the common UI font.
- Kept existing application workflows, bindings, commands and page view models intact.

## Compatibility boundaries

This is a visual-layer change. It intentionally does not rename or alter:
- Executables, assembly names or namespaces.
- AppData and settings paths or saved theme data.
- IPC, mutex, process, temporary tab or plugin identifiers.
- Game protocol, Flash integration, scripts, packet handling or account workflows.
- Release and update identities.

Existing saved themes and custom background artwork remain supported. A user's saved accent theme can override the HOT DOG accent until the HOT DOG preset is selected.

The native executable/tray icon files still use inherited Skua artwork. Replacing those files is a separate asset task and must be followed by Windows shell / installer verification.

## Remaining design work

1. Create original HOT DOG application, Manager, tray and installer icon assets.
2. Review the Client's menu hierarchy, tab strip, toolbar and multi-client workspace in detail.
3. Rework high-traffic Client pages individually, preserving every command and binding.
4. Rework Manager pages (Accounts, Army Queue, Launcher, Script tools and settings) with consistent page headers, filters, empty states and data density.
5. Audit data grids, tables, dialogs, popups, notifications, status chips and plugin-provided screens.
6. Verify contrast, keyboard focus, scaling, window resizing and the compact layouts on Windows.
7. Build and launch the complete solution after each design milestone.

## Verification status

The user previously confirmed both the Client and Manager open after the first text rebrand. The current visual-foundation change has not yet been rebuilt or visually inspected on the user's Windows machine. Do not treat this design pass as runtime-verified until the user builds and checks it.
