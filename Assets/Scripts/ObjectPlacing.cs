using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ObjectPlacing : MonoBehaviour
{
    public GameObject item;
    public GameObject interactionUI;
    public Color hover = new Color(0.07f, 0.6f, 0.2f);
    public Color idle = new Color(255, 255, 255, 78);
    public bool hovering = false;

    private void FixedUpdate()
    {
        if (hovering) hovering = !hovering;
    }

    private void LateUpdate()
    {
        if (hovering)
        {
            GetComponent<Image>().color = hover;
        }
        else
        {
            GetComponent<Image>().color = idle;
        }
    }

    public void PlaceItem()
    {
        item.SetActive(true);
        interactionUI.SetActive(false);
        var gM = FindObjectOfType<GameManager>();
        gM.itemCounter++;
        if (gM.itemCounter > 3)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            gM.sceneIndex = 3;
        }
    }
}