using Leopotam.EcsLite.Unity.Ugui;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;

namespace Utils.MainMenu
{
    public class MainMenuHandleSystem : EcsUguiCallbackSystem
    {
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.StartGameBtn, Idents.Worlds.Events)]
        private void OnClickStartGameBtn(in EcsUguiClickEvent e)
        {
            SceneManager.LoadScene("World");
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.ContinueGameBtn, Idents.Worlds.Events)]
        private void OnClickContinueGameBtn(in EcsUguiClickEvent e)
        {
            SceneManager.LoadScene("World");
        }
        
        [Preserve]
        [EcsUguiClickEvent(Idents.UI.ExitFromGameBtn, Idents.Worlds.Events)]
        private void OnClickExitGameBtn(in EcsUguiClickEvent e)
        {
            Application.Quit();
        }
    }
}