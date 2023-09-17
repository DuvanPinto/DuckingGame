using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIGrimoireController : MonoBehaviour
{
    [SerializeField] private Button _grimoireButton;
    [SerializeField] private GameObject _mainWindow;


    public void OpenWindow()
    {
        _grimoireButton.gameObject.SetActive(false);
        _mainWindow.gameObject.SetActive(true);
    }

    public void CloseWindow()
    {
        _grimoireButton.gameObject.SetActive(true);
        _mainWindow.gameObject.SetActive(false);
    }
}
