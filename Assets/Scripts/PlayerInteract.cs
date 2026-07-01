using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

interface IInteractable
{
    public void Interact();
}

public class PlayerInteract : MonoBehaviour
{
    public TextMeshProUGUI instructText;
    public GameObject text;
    public bool once;
    public bool open;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1.0f))
        {
            GameObject hitObject = hit.collider.gameObject;
            if (hitObject.CompareTag("Interactable"))
            {
                text.SetActive(true);
            }
            else
            {
                text.SetActive(false);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (hit.collider.gameObject.TryGetComponent(out IInteractable interactObj))
                {
                    interactObj.Interact();
                }
            }
        }
    }
}
