using System;
using System.Collections;
using UnityEngine;

public class AnimatorWatcher
{
    public const float MinimumWaitForAnimation = 0.05f;

    /// <summary>
    /// Watch an animator and invoke a callback once it is not animating anything on layerIndex
    /// </summary>
    /// <param name="animator">the animator to watch</param>
    /// <param name="layerIndex">the layer index of the animations</param>
    /// <param name="minimumWait">the amount of time in SECONDS to wait before checking the animator</param>
    /// <param name="callback">invoked once the animator is not animating anything</param>
    /// <param name="useRealtime">if true, will wait mimimumWait in realtime</param>
    /// <returns></returns>
    public static IEnumerator WaitForAnimatorFinished(Animator animator, int layerIndex, Action callback, float minimumWait = MinimumWaitForAnimation, bool useRealtime = false)
    {
        if (useRealtime)
        {
            yield return new WaitForSecondsRealtime(minimumWait);
        }
        else
        {
            yield return new WaitForSeconds(minimumWait);
        }

        while (animator.IsInTransition(layerIndex) || animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime < 1)
        {
            yield return null;
        }
        callback?.Invoke();
        yield break;
    }
}
