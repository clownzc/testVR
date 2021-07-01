using UnityEngine.Networking;

namespace BehaviorDesigner.Runtime.Tasks.Basic.UnityNetwork
{
    public class IsClient : Conditional
    {
        public override TaskStatus OnUpdate()
        {
            return TaskStatus.Failure;
            //return NetworkClient.active ? TaskStatus.Success : TaskStatus.Failure;
        }
    }
}