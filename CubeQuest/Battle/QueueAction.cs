using System;

namespace CubeQuest.Battle
{
	/// <summary>
	/// Generic <see cref="IQueueAction"/>
	/// </summary>
	public class QueueAction : IQueueAction
	{
		private readonly Action action;
		
		public event EventHandler OnEnd;
		
		public QueueAction(Action executeAction) => 
			action = executeAction;

		public void Execute() => 
			action.Invoke();

		public void End() => 
			OnEnd?.Invoke(this, null);
	}
}