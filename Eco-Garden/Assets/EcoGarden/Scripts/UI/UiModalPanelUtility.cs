using UnityEngine;

namespace EcoGarden.UI
{
    public static class UiModalPanelUtility
    {
        public static readonly string[] ModalPanelNames =
        {
            "ResultPanel",
            "PausePanel",
            "LevelPanel",
            "ShopPanel",
            "InventoryPanel",
            "MissionPanel"
        };

        public static readonly string[] BackdropDismissPanelNames =
        {
            "LevelPanel",
            "ShopPanel",
            "InventoryPanel",
            "MissionPanel"
        };

        public static void HideOtherModalPanels(params string[] visiblePanelNames)
        {
            for (int i = 0; i < ModalPanelNames.Length; i++)
            {
                string panelName = ModalPanelNames[i];
                if (ShouldKeepVisible(panelName, visiblePanelNames))
                {
                    continue;
                }

                SetSceneObjectsInactive(panelName);
            }
        }

        public static void HideBackdropDismissiblePanels()
        {
            for (int i = 0; i < BackdropDismissPanelNames.Length; i++)
            {
                SetSceneObjectsInactive(BackdropDismissPanelNames[i]);
            }
        }

        public static void RaiseModalPanel(GameObject panel)
        {
            if (panel == null || panel.transform == null)
            {
                return;
            }

            panel.transform.SetAsLastSibling();
        }

        public static GameObject FindObjectIncludingInactive(string objectName)
        {
            if (string.IsNullOrEmpty(objectName))
            {
                return null;
            }

            GameObject activeObject = GameObject.Find(objectName);
            if (activeObject != null)
            {
                return activeObject;
            }

            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate != null &&
                    candidate.gameObject.scene.IsValid() &&
                    candidate.name == objectName)
                {
                    return candidate.gameObject;
                }
            }

            return null;
        }

        private static bool ShouldKeepVisible(string panelName, string[] visiblePanelNames)
        {
            if (visiblePanelNames == null)
            {
                return false;
            }

            for (int i = 0; i < visiblePanelNames.Length; i++)
            {
                if (visiblePanelNames[i] == panelName)
                {
                    return true;
                }
            }

            return false;
        }

        private static void SetSceneObjectsInactive(string objectName)
        {
            Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();
            for (int i = 0; i < transforms.Length; i++)
            {
                Transform candidate = transforms[i];
                if (candidate != null &&
                    candidate.gameObject.scene.IsValid() &&
                    candidate.name == objectName)
                {
                    candidate.gameObject.SetActive(false);
                }
            }
        }
    }
}
