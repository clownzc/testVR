using UnityEngine;
using System.Collections;
using System.Collections.Generic;

    public class HFSMState<T>
    {
        private string m_tag;
        public string Tag
        {
            get
            {
                return m_tag;
            }
        }

        //level数字越大，级别越低
        private int m_level;
        public int Level
        {
            get
            {
                return m_level;
            }
        }

        private string m_fatherTag;
        public string FatherTag
        {
            get
            {
                return m_fatherTag;
            }
        }

        public delegate void Handler();
        private event Handler m_enter;
        private event Handler m_update;
        private event Handler m_exit;

        //没有父状态则赋值为null即可
        public HFSMState(string tag, int level, string father)
        {
            m_tag = tag;
            m_level = level;
            m_fatherTag = father;
        }

        public HFSMState(string tag, int level, string father, Handler enter, Handler update, Handler exit)
        {
            m_tag = tag;
            m_level = level;
            m_fatherTag = father;

            m_enter = enter;
            m_update = update;
            m_exit = exit;
        }

        public virtual void OnEnter()
        {
            if(m_enter != null)
            {
                m_enter();
            }
        }

        public virtual void OnUpdate()
        {
            if (m_update != null)
            {
                m_update();
            }
        }

        public virtual void OnExit()
        {
            if (m_exit != null)
            {
                m_exit();
            }
        }
    }

    /*此状态机为分层状态机，可以满足任意层数的状态跳转模拟
             A                      B
       A1         A2          B1            B2
    A11  A12   A21  A22    B11  B12      B21  B22 
    配置时需配置:1、每个状态的父状态的tag
                2、本身的层次level（需满足A的level值小于A1的level值，同层状态的level值必须相同）
                3、状态转移表须配置本层状态转移的合法性，如：配置A->B合法，A11->B22合法，需注意虽然A11->B22合法，但是如果A->B不合法，则无法完成跳转
                   父与子之间的跳转是合法的，无需配置。
    假设所有跳转均合法：
      如初始状态为A11，则start的时候，函数执行顺序为:A.enter() -> A1.enter() -> A11.enter()。
      继续跳转到B2状态，则函数执行顺序为:A11.exit() -> A1.exit() -> A.exit() -> B.enter() -> B2.enter()。*/
    public class HFSM<T>
    {
        private bool m_isRunning;

        private HFSMState<T> m_curState;
        public HFSMState<T> CurState
        {
            get
            {
                return m_curState;
            }
        }

        private HFSMState<T> m_prevState;
        public HFSMState<T> PreviousState
        {
            get
            {
                return m_prevState;
            }
        }

        private Dictionary<string, HFSMState<T>> m_tagToState;
        //[nextState:[formerState:true/false]]，从formerState跳转到nextState是否合法,每个状态只需加入本层的跳转是否合法信息即可
        private Dictionary<string, Dictionary<string, bool>> m_stateChangeAvaibleMap;
        public Dictionary<string, Dictionary<string, bool>> StateChangeMap
        {
            get
            {
                return m_stateChangeAvaibleMap;
            }
            set
            {
                m_stateChangeAvaibleMap = value;
            }
        }

        public HFSM()
        {
            Init();
        }

        public bool AddStateWithTag(string tag, HFSMState<T> state)
        {
            bool isLegal = true;

            if(m_tagToState != null && !m_tagToState.ContainsKey(tag))
            {
                m_tagToState.Add(tag, state);
            }
            else
            {
                isLegal = false;
                Debugger.LogError(string.Format("HFSM add state {0} error!!!", tag));
            }

            return isLegal;
        }

        public void SetCurrentState(string tag)
        {
            if(m_isRunning)
            {
                ChangeState(tag);
            }
            else if(m_tagToState != null && m_tagToState.ContainsKey(tag))
            {
                m_curState = m_tagToState[tag];
            }
        }

        public void StartStateMachine()
        {
            if (m_isRunning)
            {
                Debugger.Log(string.Format("HFSM is already started!"));
                return;
            }
            m_isRunning = true;

            Stack<HFSMState<T>> sequenceStack = new Stack<HFSMState<T>>();
            HFSMState<T> current = m_curState;
            while(current != null)
            {
                sequenceStack.Push(current);
                current = current.FatherTag == null ? null : m_tagToState[current.FatherTag];
            }
            while (sequenceStack.Count != 0)
            {
                HFSMState<T> tmpState = sequenceStack.Pop();
                tmpState.OnEnter();
            }
        }

        public void StopStateMachine()
        {
            if (!m_isRunning)
            {
                Debugger.Log(string.Format("HFSM is already stopped!"));
                return;
            }
            m_isRunning = false;

            HFSMState<T> current = m_curState;
            while (current != null)
            {
                current.OnExit();
                current = current.FatherTag == null ? null : m_tagToState[current.FatherTag];
            }
        }

        public void PauseStateMachine()
        {
            m_isRunning = false;
        }

        public void ResumeStateMachine()
        {
            m_isRunning = true;
        }

        public void UpdateStateMachine()
        {
            if (!m_isRunning)
            {
                return;
            }

            if (m_curState != null)
            {
                m_curState.OnUpdate();
            }
        }

        public void ChangeState(string stateTag)
        {
            if (m_tagToState.ContainsKey(stateTag))
            {
                Debugger.LogWarning(stateTag);
                HFSMState<T> stateFound = m_tagToState[stateTag];
                if (stateFound != null)
                {
                    ChangeState(stateFound);
                }
            }
        }

        private bool availableTransform(HFSMState<T> current, HFSMState<T> next)
        {
            if (current == null || next == null || current == next) return true;
            
            if (current.Level == next.Level)
            {
                string tagCurrent = current == null ? null : current.Tag;
                string tagNext = next == null ? null : next.Tag;
                if (m_stateChangeAvaibleMap == null || !m_stateChangeAvaibleMap.ContainsKey(tagNext) || 
                    !m_stateChangeAvaibleMap[tagNext].ContainsKey(tagCurrent) || m_stateChangeAvaibleMap[tagNext][tagCurrent] != true)
                    return false;
                //循环最终判断可跳转节点跳转合法性
                while(current.FatherTag != next.FatherTag)
                {
                    current = m_tagToState[current.FatherTag];
                    next = m_tagToState[next.FatherTag];
                }
                tagCurrent = current == null ? null : current.Tag;
                tagNext = next == null ? null : next.Tag;
                if (!m_stateChangeAvaibleMap.ContainsKey(tagNext) || !m_stateChangeAvaibleMap[tagNext].ContainsKey(tagCurrent) || m_stateChangeAvaibleMap[tagNext][tagCurrent] != true)
                    return false;
                return true;
            }
            else if(current.Level > next.Level)
            {
                while (current.Level != next.Level && current != null)
                {
                    current = current.FatherTag == null ? null : m_tagToState[current.FatherTag];
                }
                return availableTransform(current, next);
            }
            else
            {
                while (current.Level != next.Level && next != null)
                {
                    next = next.FatherTag == null ? null : m_tagToState[next.FatherTag];
                }
                return availableTransform(current, next);

            }
        }

        public void ChangeState(HFSMState<T> newState)
        {
            if (newState == null)
            {
                Debugger.LogError("can't find this state");
                return;
            }
            
            if (availableTransform(m_curState, newState))
            {
                Stack<HFSMState<T>> newStateStack = new Stack<HFSMState<T>>();
                HFSMState<T> current = m_curState;
                HFSMState<T> next = newState;
                if(current.Level > next.Level)
                {
                    while (current != null && current.Level != next.Level)
                    {
                        current.OnExit();
                        current = current.FatherTag == null ? null : m_tagToState[current.FatherTag];
                    }
                }
                else if (current.Level < next.Level)
                {
                    while (next != null && current.Level != next.Level)
                    {
                        newStateStack.Push(next);
                        next = next.FatherTag == null ? null : m_tagToState[next.FatherTag];
                    }
                }

                while (current != null && m_tagToState[current.Tag] != m_tagToState[next.Tag])
                {
                    current.OnExit();
                    current = current.FatherTag == null ? null : m_tagToState[current.FatherTag];
                    newStateStack.Push(next);
                    next = next.FatherTag == null ? null : m_tagToState[next.FatherTag];
                }

                m_prevState = m_curState;
                while (newStateStack.Count != 0)
                {
                    HFSMState<T> tmpState = newStateStack.Pop();
                    tmpState.OnEnter();
                }

                m_curState = newState;
            }
            else
            {
                Debugger.LogError(string.Format("HFSM can't change state from {0} to {1}!", m_curState == null ? "null" : m_curState.Tag, newState.Tag));
            }
        }

        private void Init()
        {
            m_isRunning = false;
            m_curState = null;
            m_prevState = null;

            if (m_tagToState == null)
                m_tagToState = new Dictionary<string, HFSMState<T>>();
        }

        public HFSMState<T> GetStateByName(string name)
        {
            if(m_tagToState != null && m_tagToState.ContainsKey(name))
            {
                return m_tagToState[name];
            }
            return null;
        }
    }
