using UnityEngine;

namespace Aberration
{
	/// <summary>
	/// Based on  https://www.youtube.com/watch?v=7XVSLpo97k0&ab_channel=MercenaryCamp
	/// </summary>
	public class FollowWorld : MonoBehaviour
    {
        [SerializeField]
        private Transform followTransform;

        [SerializeField]
        private Vector3 offset;

        [SerializeField]
        private Camera mainCamera;

        // Update is called once per frame
        void Update()
        {
            Vector3 position = mainCamera.WorldToScreenPoint(followTransform.position + offset);

            if (transform.position != position)
                transform.position = position;
        }
    }
}
