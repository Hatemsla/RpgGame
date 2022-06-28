using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;

public class MenuManager : MonoBehaviourPunCallbacks
{
    public static MenuManager instance;

    [SerializeField] private List<Menu> _menus;

    public void OpenMenu(string menuName)
    {
        foreach (Menu menu in _menus)
        {
            if (menu.menuName == menuName)
            {
                menu.Open();
            }
            else
            {
                menu.Close();
            }
        }
    }

    private void Awake()
    {
        instance = this;
    }
}
