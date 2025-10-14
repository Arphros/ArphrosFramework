using System;

namespace ArphrosFramework
{
    public static class TriggerFactory
    {
        public static ITriggerBehavior Create(Trigger owner, TriggerType type)
        {
            return type switch
            {
                TriggerType.Camera => new CameraTrigger(owner),
                // TriggerType.Jump => new JumpTrigger(...),
                // TriggerType.FreezePlayer => new FreezePlayerTrigger(...),
                _ => throw new NotImplementedException(type.ToString())
            };
        }
    }
}