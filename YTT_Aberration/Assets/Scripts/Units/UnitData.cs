using UnityEngine;

namespace Aberration
{
	public class UnitData : ScriptableObject
    {
		[Header("Movement")]
		[SerializeField]
        private float moveSpeed = 1f;
        public float MoveSpeed
		{
            get { return moveSpeed; }
		}

        [SerializeField]
        private float turnSpeed = 120f;
        public float TurnSpeed
        {
            get { return turnSpeed; }
        }

        [SerializeField]
        private float acceleration = 4f;
        public float Acceleration
        {
            get { return acceleration; }
        }

        [Header("Size")]
        [SerializeField]
        private float radius = 4f;
        public float Radius
        {
            get { return radius; }
        }

        [SerializeField]
        private float height = 4f;
        public float Height
        {
            get { return height; }
        }

        [Header("Combat")]
        [SerializeField]
        private int maxHP = 10;
        public int MaxHP
        {
            get { return maxHP; }
        }

        [SerializeField]
        private int attack = 2;
        public int Attack
        {
            get { return attack; }
        }

        [SerializeField]
        private int armour = 1;
        public int Armour
        {
            get { return armour; }
        }

        [SerializeField]
        private int range = 3;
        public int Range
        {
            get { return range; }
        }

        [Header("Yeeting")]
        /// <summary>
        /// Minimum velocity at which the Unit is still classed as yeeting
        /// </summary>
        [SerializeField]
        private float stillYeetingMinVelocity = 0.5f;
        public float StillYeetingMinVelocity
		{
            get { return stillYeetingMinVelocity; }
		}

        [Header("Animation")]
        [SerializeField]
        private string recoverAnimClipName;
        public string RecoverAnimClipName
        {
            get { return recoverAnimClipName; }
        }

        [SerializeField]
        private float timeToResetBones = 2f;
        public float TimeToResetBones
        {
            get { return timeToResetBones; }
        }

        [SerializeField]
        private string recoverAnimStateName;
        public string RecoverAnimStateName
        {
            get { return recoverAnimStateName; }
        }

        [SerializeField]
        private float recoverTimeSecs = 2f;
        public float RecoverTimeSecs
        {
            get { return recoverTimeSecs; }
        }

        [SerializeField]
        private string combatAnimStateName;
        public string CombatAnimStateName
        {
            get { return combatAnimStateName; }
        }

        [SerializeField]
        private float delayBetweenAttacks = 0.2f;
        public float DelayBetweenAttacks
        {
            get { return delayBetweenAttacks; }
        }

        [Header("Sound")]
        [SerializeField]
        private AudioClip attackSound;
        public AudioClip AttackSound
        {
            get { return attackSound; }
        }
    }
}
