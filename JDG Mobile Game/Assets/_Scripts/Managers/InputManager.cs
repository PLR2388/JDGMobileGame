using UnityEngine;

public class InputManager : Singleton<InputManager>
{
    public static Vector3 TouchPosition
    {
        get
        {
#if UNITY_EDITOR
            var position = Input.mousePosition;
#elif UNITY_ANDROID
        var position = Input.GetTouch(0).position;
#endif
            return position;
        }
    }
}
