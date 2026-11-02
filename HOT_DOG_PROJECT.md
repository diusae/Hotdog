# HOT DOG — Project Baseline

> Product name: **Hot Dog**
> Repository: `diusae/Hotdog`
> Upstream: `NinjaXz/VibeSkua`
> Development branch: `hot-dog-dev`
> Baseline branch: `main`
> Baseline commit: `ef8ca2bff2773d30b179696f5d278d460744e6a6`
> Baseline inspected: 2026-11-03

## 1. Product goal

Hot Dog is our own evolving client product, built by taking the existing VibeSkua codebase as its starting point.

We are **not** rewriting the client from scratch and we are **not** doing a cosmetic rename only. We will understand the current implementation, preserve working behavior, and add improvements in small, reviewable, testable steps.

The first milestone is to establish a reproducible local build and document the existing architecture. No application behavior is intentionally changed in this baseline commit.

## 2. Repository and branch rules

- `main` is kept as the clean upstream-based baseline for now.
- All Hot Dog development changes go to `hot-dog-dev`.
- Do not commit development changes directly to `main`.
- Do not force-push or rewrite shared history.
- Keep the upstream relationship to `NinjaXz/VibeSkua`; review upstream changes before merging them.
- Make focused commits and record user-visible changes.
- Do not rename namespaces, assemblies, executable names, settings directories, IPC identifiers, mutex names, or update channels as a bulk operation. Each requires impact analysis and a migration plan.

## 3. Verified solution structure

The current solution contains 12 C# projects:

| Project | Current responsibility |
|---|---|
| `Skua.App.Avalonia` | Main client executable and client UI |
| `Skua.Manager.Avalonia` | Manager / account management / client launcher |
| `Skua.Core` | Core services, scripts, plugins, game integration |
| `Skua.Core.Interfaces` | Core contracts and interfaces |
| `Skua.Core.Models` | Shared models |
| `Skua.Core.Utils` | Shared utility code |
| `Skua.Core.Generators` | Source generator project |
| `Skua.Shared.Avalonia` | Shared Avalonia controls, views, themes and view models |
| `Skua.AS3` | ActionScript 3 source/build project |
| `Skua.Plugin.DailyTracker` | Daily Tracker plugin |
| `Skua.Plugin.ButlerV5` | ButlerV5 plugin |
| `Skua.Plugin.Cosmetics` | Cosmetics plugin |

The solution also includes build/release scripts, GitHub Actions workflows, Flash/FFDec support files, and large game-data JSON files.

## 4. Technology confirmed from project files

- C#
- .NET 10
- Avalonia 11.3.12 package family (central package versions)
- MVVM via CommunityToolkit.Mvvm
- Microsoft.Extensions.DependencyInjection
- Velopack 1.2.0
- Windows-specific client target: `net10.0-windows`
- Client runtime identifiers: `win-x64` and `win-x86`
- Windows Forms and Shockwave Flash interop references in the client project

The client is Windows-specific. Flash/COM interop and native dependencies mean the first build and runtime verification must be performed on Windows with the required dependencies available.

## 5. Important architecture observations

### Separate applications

`Skua.App.Avalonia` and `Skua.Manager.Avalonia` are separate executables. Their project files reference Core/Shared layers rather than referencing one another directly.

`Directory.Build.targets` contains explicit MSBuild checks that reject direct Manager-to-App and App-to-Manager project references, and also prevents Shared from depending on App/Manager projects. These constraints must be preserved.

### Client startup

The client entry point is `Skua.App.Avalonia/Program.cs`. It initializes managed/native assembly resolution, initializes client file sources, preloads the SWF asynchronously, runs Velopack startup handling, and starts the Avalonia desktop lifetime.

### Dependency registration

- Client services and client view models are registered in `Skua.App.Avalonia/App.axaml.cs` and `Skua.App.Avalonia/AppStartup/Services.cs`.
- Shared/common services are registered through `Skua.Core/AppStartup/Services.cs`.
- Manager services and view models are registered in `Skua.Manager.Avalonia/App.axaml.cs` and `Skua.Manager.Avalonia/AppStartup/Services.cs`.

### Script and plugin foundations

Core script functionality is primarily under `Skua.Core/Scripts`. Plugin loading is implemented by `Skua.Core/Plugins/PluginManager.cs`. Script assemblies use a collectible `AssemblyLoadContext` in `Skua.Core/ScriptLoadContext.cs`.

### Persistent user data

Client-owned files/directories are initialized through `ClientFileSources` and `Skua.Core/Services/ClientFilesService.cs`. Any future branding or product-identity migration must first inspect how these paths are derived, so existing settings, scripts, plugins, themes and user data are not orphaned.

### Initial product-identity and data-compatibility map

The first read-only identity audit found several coupled identifiers. They must not be changed as one blind rename:

- The original client window and tabbed host labels were `VibeSkua`. The current branch now resolves visible client labels through `Skua.Core.Models/ProductIdentity.cs` and displays `HOT DOG`.
- The client executable and assembly remain `VibeSkua.exe` / `VibeSkua` for compatibility. The Manager's `SkuaExecutableLocator` explicitly searches for `VibeSkua.exe`, with `Skua.exe` as a legacy fallback.
- The client project declares assembly metadata `Skua.ClientId=VibeSkuaAvalonia`.
- The tabbed-host single-instance and inter-process communication names are currently `VibeSkuaTabHostPipe` and `VibeSkuaTabHostMutex`; per-instance names are derived from these.
- Client data currently lives under `%APPDATA%\\VibeSkua`. The code can copy legacy data from `%APPDATA%\\Skua` into that directory on first initialization, guarded by `Global\\VibeSkua.Migration.Init` and a migration marker. The existing migration excludes the top-level `plugins` directory from that copy.
- User data includes settings, scripts, themes, options, plugins and supporting JSON/cache files. A future move to a `HotDog` data directory must explicitly preserve existing VibeSkua data and account for the existing legacy Skua migration; simply changing the folder constant would risk making the user's existing setup appear empty.

**Safe first identity milestone:** change user-visible product labels and add brand styling while keeping executable names, assembly identities, AppData paths, IPC/mutex names, plugin contracts and update identifiers unchanged. Plan and test any deeper identity migration separately.

## 6. Build and safety notes

The README documents this manual build command:

```powershell
dotnet restore Skua.sln
dotnet build Skua.sln -c Release -p:WarningLevel=0 --nologo
```

The user reported a successful local Windows baseline on 2026-11-03 using .NET SDK 10.0.303: `dotnet restore Skua.sln` completed successfully, and `dotnet build Skua.sln -c Release -p:WarningLevel=0 --nologo` built all 12 solution projects successfully in 39.2 seconds. The user also confirmed that the client executable starts, displays its window and is currently stable. This does not yet verify Manager startup, Flash/COM integration, game connection, scripts or plugins.

Important: `BuildProject.bat` is not a passive build command. When run from a branch named `main`, it performs Git synchronization steps involving `origin` and `upstream`, and may push to `origin`. Do not run it from `main` until its behavior and remotes are deliberately configured. Prefer the explicit `dotnet restore` / `dotnet build` commands for the initial baseline check.

The automated release workflow and `Publish-Release.bat` package the Manager as the Velopack entry executable. Release packaging and update endpoints must be reviewed before creating a Hot Dog release.

## 7. Preservation rules

1. Do not remove or replace existing features without a documented reason and explicit scope.
2. Do not change game protocol, Flash integration, packet handling, or script execution behavior as part of branding work.
3. Do not bulk-rename `Skua` identifiers. First map public APIs, file paths, settings keys, IPC names, mutexes, process names, update identifiers and plugin contracts.
4. Preserve backwards compatibility for settings and user data wherever possible.
5. Keep changes small and build after each meaningful change.
6. Add automated tests around logic that can be tested without the Flash/COM runtime.
7. Report clearly when runtime behavior cannot be verified without the user's Windows environment.
8. Preserve upstream credits and all applicable third-party notices.

## 8. Licensing follow-up

The VibeSkua README describes the project as MIT-licensed and credits the original Skua and the early Avalonia port. However, the repository API currently reports no detected repository license, and the root tree inspected for this baseline does not contain a root `LICENSE` file.

Before distributing Hot Dog binaries or source, inspect the upstream and inherited project files and all bundled third-party notices. Do not assume that every asset, game-data file, binary or dependency has the same license as the main source code.

## 9. Initial roadmap

### Phase 0 — Baseline understanding
- [x] Confirm the repository fork and permissions.
- [x] Create a dedicated development branch.
- [x] Record the first-pass solution architecture.
- [ ] Inspect the full startup path and process/IPC lifecycle.
- [x] Map persistent data paths and the initial settings migration risks; deeper migration testing remains required.
- [ ] Map script lifecycle and plugin lifecycle in detail.

### Phase 1 — Local baseline verification
- [ ] Clone the Hot Dog repository locally.
- [x] Restore NuGet dependencies (user-reported success, .NET SDK 10.0.303).
- [x] Build the solution in Release (all 12 projects succeeded, 39.2 seconds; user-reported).
- [x] Launch the client executable on Windows; user confirms the client window opens and is currently stable.
- [x] Launch the Manager separately; user confirms both client and Manager windows open.
- [ ] Record exact build errors/warnings, if any.
- [ ] Verify a minimal existing workflow before changing code.

### Phase 2 — First Hot Dog identity changes
Only after the baseline is verified and identity dependencies are mapped:
- [x] Map the key visible product-label locations in the client, Manager, tray menu, title bar and About page.
- [x] Change the visible client and Manager labels to **HOT DOG** in the two application shells and About page.
- [x] Keep executable names, assembly metadata, AppData paths, IPC/mutex names, temporary tab paths, plugin contracts and update identifiers unchanged in this first branding pass.
- [ ] Create and integrate original HOT DOG icon / artwork assets; existing Skua artwork is intentionally still in place.
- [x] Add a built-in HOT DOG dark theme preset with orange and mustard accents. New installs use it as the first default preset; an existing saved `CurrentTheme` is preserved.
- [x] Define `Skua.Core.Models.ProductIdentity` as the central source for user-visible product names and initial brand colors.
- [ ] Plan settings/data directory compatibility.
- [ ] Plan process, IPC, mutex and update-channel identity changes.
- [x] Rebuild and verify the first visible-label rebrand on the user's Windows machine; user confirms both client and Manager open.
- [ ] Rebuild and verify the new central identity and HOT DOG theme palette on the user's Windows machine.
- [x] Enable the Windows build workflow on pushes to `hot-dog-dev`.

### Phase 3 — HOT DOG visual overhaul
- [x] Establish shared HOT DOG light/dark colour tokens and brand palette.
- [x] Standardise shared buttons, toggles, text inputs, combo boxes, cards, tabs, menus, expanders and list controls.
- [x] Refresh shared window title bar dimensions, controls and HOT DOG monogram.
- [x] Refresh Client top navigation and multi-client tab strip proportions.
- [x] Rebuild Manager primary navigation as a dedicated left sidebar.
- [x] Add a documented shared design system in `HOT_DOG_DESIGN_SYSTEM.md`.
- [ ] Build and visually verify the design-foundation change on Windows.
- [ ] Create original native app, tray and installer icon assets.
- [ ] Redesign the Client's high-traffic pages individually.
- [ ] Redesign Manager pages individually, preserving all commands and bindings.
- [ ] Audit dialogs, tables, popups, notifications, plugin screens, accessibility and DPI scaling.

### Phase 4 — Product improvements
- [ ] Establish feature priorities with the user.
- [ ] Add tests for testable Core behavior.
- [ ] Improve existing systems incrementally.
- [ ] Add new modules without breaking existing contracts.

Current milestone: the user confirmed the original baseline Release build succeeds and the first visible-label rebrand opens in both Client and Manager. The current branch now includes the first full design-foundation pass: shared dark/light colour tokens, updated reusable control styles, a 40px shared title bar with an HOT DOG monogram, refreshed Client navigation sizing, and a new Manager left-sidebar layout. The design system is documented in `HOT_DOG_DESIGN_SYSTEM.md`. These latest visual changes have not yet been rebuilt or visually verified by the user. Executable/assembly identities, AppData locations and migration logic, saved themes, IPC/mutex names, temporary tab paths, plugin contracts, game workflows and update identifiers remain unchanged. Native app/tray/installer icons still require original HOT DOG assets. Individual Client and Manager pages still need screen-by-screen redesign and testing.

## 10. Current status

Current milestone: the user confirmed the baseline Release build succeeds and the client window opens stably. The first branding pass has been committed on `hot-dog-dev`: client window/title-bar labels, Manager title, Manager tray labels, shared title-bar default, and the About page heading now display HOT DOG. This is a user-interface text change only. Executable/assembly identities, AppData locations and migration logic, IPC/mutex names, temporary tab paths, plugin contracts, update identifiers and existing Skua icon/artwork assets were intentionally left unchanged. The modified solution has not yet been rebuilt by the user; Manager startup and deeper game/Flash/script/plugin workflows remain unverified.
