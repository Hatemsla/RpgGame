using UnityEngine;
using UnityEngine.SceneManagement;

namespace World.UI
{
    public class ExitMenu : MonoBehaviour
    {
        public void OnClickCloseExitMenuBtn()
        {
            gameObject.SetActive(false);
        }

        public void OnClickSaveExitMenuBtn()
        {
            
        }

        public void OnClickGoToMenuBtn()
        {
            SceneManager.LoadScene("MainMenu");
        }

        public void OnClickExitFromGameBtn()
        {
            Application.Quit();
        }
    }
}