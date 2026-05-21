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

Status: Platform IAP provider decision documented; Android store build with Unity IAP not run yet.

Current result:

```text
Packages/manifest.json does not include com.unity.purchasing.
```

The current vertical slice uses `MockIapProvider`, so Runtime, Editor, and EditMode test assemblies build without a store SDK. A production Android IAP build requires installing Unity IAP, adding Google Play managed products, and adding a `UnityIapProvider` implementation behind `IIapProvider`.

Required product ids:

```text
eco_garden_gems_small
eco_garden_gems_medium
```

Required follow-up before production IAP build:

1. Install `com.unity.purchasing` through Unity Package Manager.
2. Confirm Android build still succeeds after package import.
3. Configure Google Play Console managed products with the product ids above.
4. Implement `UnityIapProvider`.
5. Persist processed transaction ids in save data.
6. Run an internal Google Play test purchase on device.
