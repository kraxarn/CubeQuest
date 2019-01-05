using System;

namespace CubeQuest.Battle
{
	public interface IQueueAction
	{
		/// <summary>
		/// When the item is done with it's thing
		/// </summary>
		event EventHandler OnEnd;

		/// <summary>
		/// The action to execute when its next in line
		/// </summary>
		void Execute();
	}
}