using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Helpers
{
    public class DestroyWithinDelay : MonoBehaviour
    {
        static WaitForSeconds waitDelay = new WaitForSeconds(1.5f);

        // Start is called before the first frame update
        IEnumerator Start()
        {
            yield return waitDelay;
            Destroy(gameObject);
        }
    }
}