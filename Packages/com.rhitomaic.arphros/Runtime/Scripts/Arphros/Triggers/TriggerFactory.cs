using System;

namespace ArphrosFramework.Triggers
{
    public static class TriggerFactory
    {
        public static ITriggerBehavior Create(Trigger owner, TriggerType type)
        {
            return type switch
            {
                TriggerType.Camera => new CameraTrigger(owner),
                TriggerType.Jump => new JumpTrigger(owner),
                TriggerType.Speed => new SpeedTrigger(owner),
                // TriggerType.FreezePlayer => new FreezePlayerTrigger(...),
                _ => throw new NotImplementedException(type.ToString())
            };
        }
    }
}