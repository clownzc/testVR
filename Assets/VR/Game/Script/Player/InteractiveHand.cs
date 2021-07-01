using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class InteractiveHand : MonoBehaviour
    {
        PlayerHand m_playerHand;

        private void Awake()
        {
            m_playerHand = GetComponentInParent<PlayerHand>();
        }

        private void OnTriggerEnter(Collider other)
        {
            Item.ItemBase item = other.GetComponent<Item.ItemBase>();
            if(item != null)
            {
                m_playerHand.CurInteractiveItem = item;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Item.ItemBase item = other.GetComponent<Item.ItemBase>();
            if (item != null && m_playerHand.CurInteractiveItem == item)
            {
                m_playerHand.CurInteractiveItem = null;
            }
        }
    }
}