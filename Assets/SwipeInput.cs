using UnityEngine;
using UnityEngine.InputSystem;

public class SwipeInput : MonoBehaviour
{
    public Vector2 SwipeDirection { get; private set; }

    public void OnSwipe(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>();

        if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            SwipeDirection = delta.x > 0 ? Vector2.right : Vector2.left;
        }
        else
        {
            SwipeDirection = delta.y > 0 ? Vector2.up : Vector2.down;
        }
    }
}