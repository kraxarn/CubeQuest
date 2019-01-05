using System;

namespace CubeQuest.Battle
{
    public class Attack : IQueueAction
    {
        public event EventHandler OnEnd;
        public void Execute()
        {
            throw new NotImplementedException();
        }
    }
}