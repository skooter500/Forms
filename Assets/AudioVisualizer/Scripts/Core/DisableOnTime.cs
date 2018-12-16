using UnityEngine;
using System.Collections;

namespace AudioVisualizer
{
    /// <summary>
    /// Disables an object after it's been active for a set amount of time.
    /// </summary>
    public class DisableOnTime : MonoBehaviour
    {
        //____________Public Variables

        [Tooltip("Disable this object after it's awake x seconds")]
        public float disableTime;

        //____________Delegates/Actions

        //____________Protected Variables

        //____________Private Variables

        private float disableTimer = 0;

        /*________________Monobehaviour Methods________________*/

        // Use this for initialization
        void OnEnable()
        {
            disableTimer = 0;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            
            disableTimer += Time.fixedDeltaTime;

            if (disableTimer > disableTime)
            {
                this.gameObject.SetActive(false);
            }
        }

        /*________________Public Methods________________*/

        /*________________Protected Methods________________*/

        /*________________Private Methods________________*/
    }
}
