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

## 6. Build and safety notes

The README documents this manual build command:

```powershell
dotnet restore Skua.sln
dotnet build Skua.sln -c Release -p:WarningLevel=0 --nologo
```

The user reported a successful local Windows baseline on 2026-11-03 using .NET SDK 10.0.303: `dotnet restore Skua.sln` completed successfully, and `dotnet build Skua.sln -c Release -p:WarningLevel=0 --nologo` built all 12 solution projects successfully in 39.2 seconds. This confirms compilation only; GUI startup, Flash/COM integration, game connection, scripts and plugins have not yet been runtime-tested.

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
- [ ] Map persistent data paths and settings migration risks.
- [ ] Map script lifecycle and plugin lifecycle in detail.

### Phase 1 — Local baseline verification
- [ ] Clone the Hot Dog repository locally.
- [x] Restore NuGet dependencies (user-reported success, .NET SDK 10.0.303).
- [x] Build the solution in Release (all 12 projects succeeded, 39.2 seconds; user-reported).
- [ ] Launch Manager and client on Windows.
- [ ] Record exact build errors/warnings, if any.
- [ ] Launch the Manager and client on Windows.
- [ ] Verify a minimal existing workflow before changing code.

### Phase 2 — First Hot Dog identity changes
Only after the baseline is verified:
- [ ] Define a central product identity/configuration.
- [ ] Plan safe application display-name and icon changes.
- [ ] Plan settings/data directory compatibility.
- [ ] Plan process, IPC, mutex and update-channel identity changes.
- [ ] Implement identity changes in focused commits and test them.

### Phase 3 — Product improvements
- [ ] Establish feature priorities with the user.
- [ ] Add tests for testable Core behavior.
- [ ] Improve existing systems incrementally.
- [ ] Add new modules without breaking existing contracts.

## 10. Current status

This file is documentation only. The user has confirmed a successful Release compilation, but runtime startup and gameplay behavior are not yet verified. No application source files, project files, dependencies, scripts, settings or runtime behavior were changed by this baseline documentation step.
