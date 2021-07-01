using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player;

namespace Item
{
    public enum ItemType
    {
        None,
        Hold,
        Grab
    }

    [RequireComponent(typeof(Collider))]
    public class ItemBase : MonoBehaviour
    {
        [SerializeField]
        private ItemType m_itemType = ItemType.None;
        private PlayerHand m_playerHand = null;

        [SerializeField]
        private Collider m_collider;
        [SerializeField]
        private Rigidbody m_rigidbody;

        public ItemType ItemType
        {
            get
            {
                return m_itemType;
            }

            set
            {
                m_itemType = value;
            }
        }

        public PlayerHand PlayerHand
        {
            get
            {
                return m_playerHand;
            }

            set
            {
                m_playerHand = value;
            }
        }

        public void SetInControl(PlayerHand playerHand)
        {
            PlayerHand = playerHand;
            m_collider.enabled = false;
            m_rigidbody.isKinematic = true;
        }

        public void SetFree()
        {
            PlayerHand = null;
            m_collider.enabled = true;
            m_rigidbody.isKinematic = false;
        }
    }
}