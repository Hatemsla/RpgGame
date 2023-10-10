using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace World.Inventory
{
    public sealed class ItemView : MonoBehaviour
    {
        public int itemIdx;
        public Image itemImage;
        public TMP_Text itemName;
        public TMP_Text desc;
    }
}