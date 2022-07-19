using UnityEngine;

namespace Canvas
{
    public class SettingActiveButton : MonoBehaviour
    {
        
        [Header("Components")]
        public GameObject blackout;
        public GameObject setting;
        
        public void ActiveSetting()
        {
            blackout.SetActive(true);
            setting.SetActive(true);
        }
    
        public void DisableSetting()
        {
            blackout.SetActive(false);
            setting.SetActive(false);
        }
    }
}