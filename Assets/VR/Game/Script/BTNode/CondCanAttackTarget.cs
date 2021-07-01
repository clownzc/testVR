
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;

namespace BTNode
{
    [TaskDescription("Condition: can attack target?")]
    [TaskCategory("Custom")]
    public class CondCanAttackTarget : Conditional
    {
        public SharedGameObject _target;
        public SharedFloat _attackDistance;
	
        public override TaskStatus OnUpdate()
        {
            var v = transform.position - _target.Value.transform.position;
            v.y = 0;
            var dist = v.magnitude;
            if (dist <= _attackDistance.Value)
            {
                return TaskStatus.Success;
            }
            else
            {
                return TaskStatus.Failure;
            }
        } 
    }
}