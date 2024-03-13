using Aberration.Assets.Scripts;
using UnityEngine;

namespace Aberration
{
	public abstract class Controller : MonoBehaviour
    {
        public abstract TeamOwnerType OwnerType
		{
            get;
		}

		private EventDispatcher eventDispatcher;
		public EventDispatcher EventDispatcher
		{
			get 
			{
				if (eventDispatcher == null)
					eventDispatcher = new EventDispatcher();

				return eventDispatcher; 
			}
		}

		protected virtual void Awake()
		{
			if (eventDispatcher == null)
				eventDispatcher = new EventDispatcher();
		}
	}
}
