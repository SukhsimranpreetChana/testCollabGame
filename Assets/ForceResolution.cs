using UnityEngine;

public class ForceResolution : MonoBehaviour
{
    private void Awake()
    {
        Screen.SetResolution(
            2560,
            1440,
            FullScreenMode.FullScreenWindow
        );
    }
}