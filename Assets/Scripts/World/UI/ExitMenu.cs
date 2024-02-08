using Leopotam.EcsLite;
using UnityEngine;
using UnityEngine.SceneManagement;
using World.SaveGame;

namespace World.UI
{
    public class ExitMenu : MonoBehaviour
    {
        private EcsWorld _eventWorld;

        public void SetWorld(EcsWorld eventWorld)
        {
            _eventWorld = eventWorld;
        }
        
        public void OnClickCloseExitMenuBtn()
        {
            gameObject.SetActive(false);
        }

        public void OnClickSaveExitMenuBtn()
        {
            var saveEventPool = _eventWorld.GetPool<SaveEventComp>();
            
            var saveEventEntity = _eventWorld.NewEntity();
            saveEventPool.Add(saveEventEntity);
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