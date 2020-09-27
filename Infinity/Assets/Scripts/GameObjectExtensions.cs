using System;
using System.Collections;
using UnityEngine;

namespace Assets.Scripts
{
    public static class GameObjectExtensions
    {
        public static void ExecuteDelayed(this MonoBehaviour monoBehaviour, Action action, float delay)
        {
            IEnumerator ExecuteDelayedCoroutine()
            {
                yield return new WaitForSeconds(delay);
                action();
            }

            monoBehaviour.StartCoroutine(ExecuteDelayedCoroutine());
        }
        public static void Invoke(this MonoBehaviour monoBehaviour, Action action)
        {
            IEnumerator InvokeCoroutine()
            {
                yield return new WaitForEndOfFrame();
                action();
            }

            monoBehaviour.StartCoroutine(InvokeCoroutine());
        }
    }
}