using System.Collections.Generic;
using UnityEngine;
using Item;

namespace Player
{
    public class PlayerHand : MonoBehaviour
    {
        private readonly string OPEN_STATE = "OPEN_STATE";
        private readonly string HOLD_STATE = "HOLD_STATE";
        private readonly string HOLD_TO_RELEASE_STATE = "HOLD_TO_RELEASE_STATE";
        private readonly string GRAB_STATE = "GRAB_STATE";
        private readonly string POINT_STATE = "POINT_STATE";
        private readonly string FIST_STATE = "FIST_STATE";

        private HFSM<PlayerHand> m_stateMachine;

        public enum Hand
        {
            LeftHand,
            RightHand
        }
        public Hand m_handType = Hand.LeftHand;
        public Hand HandType
        {
            get { return m_handType; }
        }

        public Collider m_interactiveCollider;
        public Collider m_punchCollider;
        private ItemBase m_curInteractiveItem = null;

        private Dictionary<ItemType, string> m_itemToStateMap;

        public HandAnimator m_handAnim;

        private SteamVR_TrackedObject m_trackedObj;
        private SteamVR_Controller.Device m_device;

        [SerializeField]
        private Transform m_holdPoint;

        public ItemBase CurInteractiveItem
        {
            get
            {
                return m_curInteractiveItem;
            }

            set
            {
                m_curInteractiveItem = value;
            }
        }

        public Transform HoldPoint
        {
            get
            {
                return m_holdPoint;
            }

            set
            {
                m_holdPoint = value;
            }
        }

        private void Awake()
        {
            InitVariables();
            InitStateMachine();
            m_stateMachine.StartStateMachine();
        }

        // Update is called once per frame
        void Update()
        {
            m_device = SteamVR_Controller.Input((int)m_trackedObj.index);

            m_stateMachine.UpdateStateMachine();
        }

        private void OnOpenStateEnter()
        {
            m_handAnim.Open();
            m_interactiveCollider.enabled = true;
            m_punchCollider.enabled = false;
            if (CurInteractiveItem != null)
            {
                CurInteractiveItem.SetFree();
                CurInteractiveItem = null;
            }
        }

        private void OnOpenStateUpdate()
        {
            if ((m_device != null && m_device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) || Input.GetKeyDown(KeyCode.A))
            {
                if(CurInteractiveItem != null && m_itemToStateMap.ContainsKey(CurInteractiveItem.ItemType))
                {
                    m_stateMachine.ChangeState(m_itemToStateMap[CurInteractiveItem.ItemType]);
                }
                else if(CurInteractiveItem == null)
                {
                    m_stateMachine.ChangeState(FIST_STATE);
                }
            }
            else if((m_device != null && m_device.GetPressDown(SteamVR_Controller.ButtonMask.Grip)) || Input.GetKeyDown(KeyCode.S))
            {
                m_stateMachine.ChangeState(POINT_STATE);
            }
        }

        private void OnHoldStateEnter()
        {
            m_handAnim.gameObject.SetActive(false);
            m_interactiveCollider.enabled = false;
            CurInteractiveItem.SetInControl(this);
        }
        private void OnHoldStateUpdate()
        {
            if ((m_device != null && m_device.GetTouchDown(SteamVR_Controller.ButtonMask.Trigger)) || Input.GetKeyDown(KeyCode.A))
            {
                m_stateMachine.ChangeState(HOLD_TO_RELEASE_STATE);
            }
        }
        private void OnHoldStateExit()
        {
            m_handAnim.gameObject.SetActive(true);}

        private void OnHoldToReleaseStateUpdate()
        {
            if ((m_device != null && m_device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) || Input.GetKeyUp(KeyCode.A))
            {
                m_stateMachine.ChangeState(OPEN_STATE);
            }
        }

        private void OnGrabStateEnter()
        {
            m_handAnim.Grab();
            m_interactiveCollider.enabled = false;
            CurInteractiveItem.SetInControl(this);
        }

        private void OnGrabStateUpdate()
        {
            if ((m_device != null && m_device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) || Input.GetKeyUp(KeyCode.A))
            {
                m_stateMachine.ChangeState(OPEN_STATE);
            }
        }

        private void OnPointStateEnter()
        {
            m_handAnim.Point();
            m_interactiveCollider.enabled = false;
        }

        private void OnPointStateUpdate()
        {
            if ((m_device != null && m_device.GetPressUp(SteamVR_Controller.ButtonMask.Grip)) || Input.GetKeyUp(KeyCode.S))
            {
                m_stateMachine.ChangeState(OPEN_STATE);
            }
        }

        private void OnFistStateEnter()
        {
            m_handAnim.Close();
            m_interactiveCollider.enabled = false;
            m_punchCollider.enabled = true;
        }

        private void OnFistStateUpdate()
        {
            if ((m_device != null && m_device.GetTouchUp(SteamVR_Controller.ButtonMask.Trigger)) || Input.GetKeyUp(KeyCode.A))
            {
                m_stateMachine.ChangeState(OPEN_STATE);
            }
        }

        private void InitStateMachine()
        {
            m_stateMachine = new HFSM<PlayerHand>();

            m_stateMachine.AddStateWithTag(OPEN_STATE,
                new HFSMState<PlayerHand>(OPEN_STATE, 0, null,
                OnOpenStateEnter, OnOpenStateUpdate, null));

            m_stateMachine.AddStateWithTag(HOLD_STATE,
                new HFSMState<PlayerHand>(HOLD_STATE, 0, null,
                OnHoldStateEnter, OnHoldStateUpdate, OnHoldStateExit));
            m_stateMachine.AddStateWithTag(HOLD_TO_RELEASE_STATE,
                new HFSMState<PlayerHand>(HOLD_TO_RELEASE_STATE, 1, HOLD_STATE,
                null, OnHoldToReleaseStateUpdate, null));

            m_stateMachine.AddStateWithTag(GRAB_STATE,
                new HFSMState<PlayerHand>(GRAB_STATE, 0, null,
                OnGrabStateEnter, OnGrabStateUpdate, null));

            m_stateMachine.AddStateWithTag(POINT_STATE,
                new HFSMState<PlayerHand>(POINT_STATE, 0, null,
                OnPointStateEnter, OnPointStateUpdate, null));

            m_stateMachine.AddStateWithTag(FIST_STATE,
                new HFSMState<PlayerHand>(FIST_STATE, 0, null,
                OnFistStateEnter, OnFistStateUpdate, null));

            m_stateMachine.StateChangeMap = new Dictionary<string, Dictionary<string, bool>>();
            m_stateMachine.StateChangeMap.Add(
                OPEN_STATE, new Dictionary<string, bool>
                    {
                        { GRAB_STATE, true },
                        { HOLD_STATE, true },
                        { POINT_STATE, true },
                        { FIST_STATE, true },
                    }
                );

            m_stateMachine.StateChangeMap.Add(
                GRAB_STATE, new Dictionary<string, bool>
                    {
                        { OPEN_STATE, true },
                    }
                );

            m_stateMachine.StateChangeMap.Add(
                HOLD_STATE, new Dictionary<string, bool>
                    {
                        { OPEN_STATE, true },
                    }
                );

            m_stateMachine.StateChangeMap.Add(
                POINT_STATE, new Dictionary<string, bool>
                    {
                        { OPEN_STATE, true },
                    }
                );

            m_stateMachine.StateChangeMap.Add(
                FIST_STATE, new Dictionary<string, bool>
                    {
                        { OPEN_STATE, true },
                    }
                );
                m_stateMachine.SetCurrentState(OPEN_STATE);
        }

        private void InitVariables()
        {
            m_trackedObj = GetComponent<SteamVR_TrackedObject>();
            if (m_handAnim == null)
                m_handAnim = GetComponentInChildren<HandAnimator>();

            m_itemToStateMap = new Dictionary<ItemType, string>();
            m_itemToStateMap.Add(ItemType.Grab, GRAB_STATE);
            m_itemToStateMap.Add(ItemType.Hold, HOLD_STATE);
        }
        
        #region Movement Inputs

        public bool IsMoveButtonPress()
        {
            return (m_device != null && m_device.GetPress(SteamVR_Controller.ButtonMask.Touchpad)) ||
                    Input.GetKey(KeyCode.F);
        }
        
        #endregion

        public void HandShock(ushort strength)
        {
            m_device.TriggerHapticPulse(strength);             
        }
    }
}