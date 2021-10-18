using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HmsPlugin
{
    public class HMSSafeArea : MonoBehaviour
    {
        RectTransform Panel;
        Rect LastSafeArea = new Rect(0, 0, 0, 0);

        void Start()
        {
            Panel = GetComponent<RectTransform>();
            StartCoroutine(FirstStart());
        }

        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
                ApplySafeArea(safeArea);
        }

        Rect GetSafeArea()
        {
            return Screen.safeArea;
        }

        IEnumerator FirstStart()
        {
            var safeArea = GetSafeArea();
            while (float.IsNaN(safeArea.x))
            {
                safeArea = GetSafeArea();
                yield return new WaitForEndOfFrame();
            }
            ApplySafeArea(safeArea);
        }

        void ApplySafeArea(Rect r)
        {
            LastSafeArea = r;

            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;
            Panel.anchorMin = anchorMin;
            Panel.anchorMax = anchorMax;

            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);
        }
    }
}
