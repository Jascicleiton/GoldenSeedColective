using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Get the gameObject we have collided with, and then get all the ObscuringItemFader components on it and its children - and then trigger the fade out
        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        if(obscuringItemFaders.Length > 0)
        {
            for (int i = 0; i < obscuringItemFaders.Length; i++)
            {
                obscuringItemFaders[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Get the gameObject we have stoped colliding with, and then get all the ObscuringItemFader components on it and its children - and then trigger the fade in
        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();
        if (obscuringItemFaders.Length > 0)
        {
            for (int i = 0; i < obscuringItemFaders.Length; i++)
            {
                obscuringItemFaders[i].FadeIn();
            }
        }
    }
}
