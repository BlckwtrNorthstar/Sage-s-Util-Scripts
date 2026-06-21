using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace SagesUtilsRelease
{
    public class TaskHandler : MonoBehaviour
    {
        [Tooltip("Will automatically call Check(); > Reset(); when executedOrder.Length = taskOrder.Length if the arrays are not equivilent.")]
        public bool resetOnWrongOrder = true;
        [Tooltip("Will automatically call Check(); > Complete(); when executedOrder.Length = taskOrder.Length if the arrays are equivilent.")]
        public bool executeWhenCorrect = true;
        [Tooltip("Delay upon which Complete(); will be called after running Check();.")]
        public float activationDelay;
        [Tooltip("Delay upon which Reset(); will be called after running Check();.")]
        public float resetDelay;
        [Tooltip("Order in which an int should be fed to the executedOrder array to result in success.")]
        public int[] taskOrder;

        [HideInInspector]
        public List<int> executedOrder;
        [HideInInspector]
        public bool Completed;

        private bool Checking = false;

        void Update()
        {
            if (!Completed)
            {
                if (!Checking)
                    AutoCheck();
            }
        }

        public void Check()
        {
            bool check = executedOrder.SequenceEqual(taskOrder);

            if (check && taskOrder.Length == executedOrder.ToArray().Length)
            {
                onSuccess.Invoke();
                Completed = true;
            }
            else
                Reset();
        }

        public void Reset()
        {
            executedOrder.Clear();
            onFailure.Invoke();
        }

        void AutoCheck()
        {
            Checking = true;

            bool check = executedOrder.SequenceEqual(taskOrder);

            if (taskOrder.Length == executedOrder.ToArray().Length && executeWhenCorrect && check)
                Invoke(nameof(Complete), activationDelay);

            if (taskOrder.Length == executedOrder.ToArray().Length && resetOnWrongOrder && !check)
                Invoke(nameof(Reset), activationDelay);

            Checking = false;
        }

        void Complete()
        {
            onSuccess.Invoke();
            Completed = true;
        }

        public UltrakillEvent onFailure;
        public UltrakillEvent onSuccess;
    }
}
