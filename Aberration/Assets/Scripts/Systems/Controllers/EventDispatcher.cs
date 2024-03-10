using Aberration.Assets.Scripts;
using System;

namespace Aberration
{
	public class EventDispatcher
    {
        public event Action<Unit> UnitSpawned;

        public event Action<Unit> UnitSelected;

        public event Action<Unit> UnitDefeated;

        public event Action<TeamActionState> ActionSelected;

        public event Action<TeamActionState> ActionExecuted;

        public event Action<TeamActionState> ActionCancelled;

        public event Action YeetStart;

        public event Action YeetCancel;

        public event Action YeetEnd;

        public void FireUnitSpawned(Unit unit)
		{
            if (UnitSpawned != null)
                UnitSpawned(unit);
        }

        public void FireUnitSelected(Unit unit)
        {
            if (UnitSelected != null)
                UnitSelected(unit);
        }

        public void FireUnitDefeated(Unit unit)
        {
            if (UnitDefeated != null)
                UnitDefeated(unit);
        }

        public void FireActionSelected(TeamActionState actionState)
        {
            if (ActionSelected != null)
                ActionSelected(actionState);
        }

        public void FireActionExecuted(TeamActionState actionState)
        {
            if (ActionExecuted != null)
                ActionExecuted(actionState);
        }

        public void FireActionCancelled(TeamActionState actionState)
        {
            if (ActionCancelled != null)
                ActionCancelled(actionState);
        }

        public void FireYeetStart()
        {
            if (YeetStart != null)
                YeetStart();
        }

        public void FireYeetCancel()
        {
            if (YeetCancel != null)
                YeetCancel();
        }

        public void FireYeetEnd()
        {
            if (YeetEnd != null)
                YeetEnd();
        }
    }
}
