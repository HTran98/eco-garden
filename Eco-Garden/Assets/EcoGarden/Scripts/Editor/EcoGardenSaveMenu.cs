using EcoGarden.Save;
using UnityEditor;
using UnityEngine;

namespace EcoGarden.Editor
{
    public static class EcoGardenSaveMenu
    {
        [MenuItem("Eco Garden/Save/Clear Local Save Data")]
        public static void ClearLocalSaveData()
        {
            SaveService.Clear();
            Debug.Log("Eco Garden local save data cleared.");
        }
    }
}
