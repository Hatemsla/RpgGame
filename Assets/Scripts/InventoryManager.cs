using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public Transform inventoryPanel;
    public List<InventorySlot> slots = new List<InventorySlot>();
    public float reachDistance = 10f;
    public Camera _camera;
    public UIManager uiManager;

    private bool isOpened = false;

    private void Awake()
    {
        uiManager.UIPanel.SetActive(true);
    }

    private void Start()
    {
        uiManager.UIPanel.SetActive(false);
        for (int i = 0; i < inventoryPanel.childCount; i++)
        {
            if (inventoryPanel.GetChild(i).GetComponent<InventorySlot>() != null)
                slots.Add(inventoryPanel.GetChild(i).GetComponent<InventorySlot>());
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpened = !isOpened;
            if (isOpened)
            {
                uiManager.indicatorsPanel.SetActive(false);
                uiManager.crosshireImg.SetActive(false);
                uiManager.UIPanel.SetActive(true);
                _camera.gameObject.GetComponent<MouseLook>().enabled = false;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                uiManager.indicatorsPanel.SetActive(true);
                uiManager.crosshireImg.SetActive(true);
                uiManager.UIPanel.SetActive(false);
                _camera.gameObject.GetComponent<MouseLook>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (Physics.Raycast(ray, out hit, reachDistance))
            {
                if (hit.collider.gameObject.GetComponent<Item>() != null)
                {
                    AddItem(hit.collider.gameObject.GetComponent<Item>().item, hit.collider.gameObject.GetComponent<Item>().amount);
                    Destroy(hit.collider.gameObject);
                }
            }
        }
    }

    private void AddItem(ItemScriptableObject _item, int _amount)
    {
        foreach (InventorySlot slot in slots)
        {
            if (slot.item == _item)
            {
                if (slot.amount + _amount <= _item.maxAmount)
                {
                    slot.amount += _amount;
                    slot.itemAmountText.text = slot.amount.ToString();
                    return;
                }
                break;
            }
        }
        foreach (InventorySlot slot in slots)
        {
            if (slot.isEmpty == true)
            {
                slot.item = _item;
                slot.amount = _amount;
                slot.isEmpty = false;
                slot.SetIcon(_item.icon);
                slot.itemAmountText.text = _amount.ToString();
                break;
            }
        }
    }
}
