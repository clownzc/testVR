
using Item;
using Player;
using UnityEngine;

namespace Battle
{
    public class Weapon : MonoBehaviour
    {
        public bool chop;
        public PlayerHand hand
        {
            get
            {
                if (_item != null)
                {
                    return _item.PlayerHand;
                }
                else
                {
                    return null;
                }
            }
        }

        private HoldItem _item;

        private void Awake()
        {
            _item = GetComponentInChildren<HoldItem>();
        }
        
    }
}