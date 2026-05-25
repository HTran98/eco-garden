# Eco Garden - Android Build Verification

Date: 2026-05-19

## Scope

Task 7.4 verifies whether the Level 15 vertical slice can build for Android.

Build target scene:

```text
Assets/EcoGarden/Scenes/EcoGarden_Level15_VerticalSlice.unity
```

Build output:

```text
Eco-Garden/Builds/Android/EcoGarden_Level15_VerticalSlice.apk
```

## Batchmode Entry Point

Added editor build method:

```text
EcoGarden.Editor.EcoGardenAndroidBuildVerification.BuildLevel15Android
```

Command:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe' `
  -batchmode `
  -quit `
  -projectPath 'D:\Project\Game\Eco-Garden' `
  -executeMethod EcoGarden.Editor.EcoGardenAndroidBuildVerification.BuildLevel15Android `
  -logFile 'D:\Project\Game\Eco-Garden\Logs\AndroidBuildVerification.log'
```

## Attempt 1 Result

Status: Blocked before build method execution.

Unity exited with code `1` before running the Android build method. The project already had an active Unity editor instance and lock file:

```text
Eco-Garden/Temp/UnityLockfile
```

Observed active process:

```text
Unity.exe 6000.4.7f1
```

Relevant log tail:

```text
Successfully changed project path to: D:\Project\Game\Eco-Garden
D:/Project/Game/Eco-Garden
Exiting without the bug reporter. Application will terminate with return code 1
```

## Required Follow-Up

1. Close the open Unity editor instance for `Eco-Garden`.
2. Rerun the batchmode command above.

## Attempt 2 Result

Status: Blocked by Android platform support/licensing.

After closing the Unity editor, batchmode progressed into project import but could not switch to Android build target. The command exceeded the 15-minute shell timeout while Unity repeatedly retried licensing. No APK was produced.

Observed blockers:

```text
Missing types referenced from component BuildProfile on game object Androidâ„¢:
    UnityEditor.Android.AndroidPlatformBuildSettings, UnityEditor.Android.Extensions (1 object)
Switching to AndroidPlayer is disabled
```

Additional repeated licensing lines appeared in the log:

```text
[Licensing::Module] Error: The connection with the Unity Licensing Client has been lost.
[Licensing::Module] Error: 'com.unity.editor.headless' was not found.
```

## Required Follow-Up

1. Install or repair Unity Android Build Support for Unity `6000.4.7f1`, including SDK/NDK/OpenJDK.
2. Open Unity Hub once and confirm licensing is active for this editor version.
3. Open the project and verify Android is available as a build target.
4. Close Unity.
5. Rerun the batchmode command above.
6. If build succeeds, confirm the APK exists at `Eco-Garden/Builds/Android/EcoGarden_Level15_VerticalSlice.apk`.
7. If build fails after Android target switching works, inspect `Eco-Garden/Logs/AndroidBuildVerification.log` and document the first blocking build error here.

## Attempt 3 Result

Status: Succeeded manually in Unity Editor.

The user confirmed Android build completed successfully from Unity after Android build settings were available. Batchmode can be rerun later to verify the same path from CI/command line.

## Task 8.16 IAP Build Check

Date: 2026-05-20

Status: Superseded by the 2026-05-25 current Android build check below.

Historical result: Unity IAP was not installed yet at the time of Task 8.16. This has since changed: `com.unity.purchasing` 5.3.0 is installed, `UnityIapProvider` has a first-pass implementation, and persistent processed transaction ids are saved.

Required product ids:

```text
eco_garden_gems_small
eco_garden_gems_medium
```

Remaining follow-up before production IAP release:

1. Configure Google Play Console managed products with the product ids above.
2. Wire production `UnityIapProvider` only after backend receipt validation is available.
3. Run an internal Google Play test purchase on device.

## Current Android Build With Unity IAP

Date: 2026-05-25

Status: Succeeded in Unity batchmode.

Command:

```powershell
& 'C:\Program Files\Unity\Hub\Editor\6000.4.7f1\Editor\Unity.exe' `
  -batchmode `
  -quit `
  -projectPath 'D:\Project\Game\Eco-Garden' `
  -executeMethod EcoGarden.Editor.EcoGardenAndroidBuildVerification.BuildLevel15Android `
  -logFile 'D:\Project\Game\Eco-Garden\Logs\AndroidBuildCurrent.log'
```

Result:

```text
Eco Garden Android build result: Succeeded
Eco Garden Android build output: D:/Project/Game/Eco-Garden/Builds/Android/EcoGarden_Level15_VerticalSlice.apk
Eco Garden Android build size bytes: 1240269284
Eco Garden Android build total time: 00:04:48.7430787
```

Verified output file:

```text
D:\Project\Game\Eco-Garden\Builds\Android\EcoGarden_Level15_VerticalSlice.apk
Size: 62,825,736 bytes
```

Notes:

- Build output and logs are generated artifacts and remain ignored by git.
- The build uses the current project with Unity IAP package imported, but it is still a development APK and not a signed release candidate.
- This does not replace Google Play internal-track purchase testing.
