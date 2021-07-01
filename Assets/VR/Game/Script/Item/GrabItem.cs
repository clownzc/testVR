using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Item
{
    public class GrabItem : ItemBase
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(PlayerHand != null)
            {
                transform.position = PlayerHand.HoldPoint.position;
                transform.rotation = PlayerHand.HoldPoint.rotation;
            }
        }
    }
}