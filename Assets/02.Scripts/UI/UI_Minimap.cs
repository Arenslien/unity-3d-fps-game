using UnityEngine;

public class UI_Minimap : MonoBehaviour
{
    [SerializeField] private MinimapCamera _minimapCamera;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            _minimapCamera.ZoomIn();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            _minimapCamera.ZoomOut();
        }
    }
}