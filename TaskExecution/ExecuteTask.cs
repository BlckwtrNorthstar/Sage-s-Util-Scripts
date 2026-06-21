using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace SagesUtilsRelease
{
    public class ExecuteTask : MonoBehaviour
    {
        [Tooltip("Instance that the executed task is directed to.")]
        public TaskHandler taskParent;
        [Tooltip("Automatically calls AddTaskByNumber(taskNumber); on component enable.")]
        public bool finishOnEnable = true;
        [Tooltip("Adds this number to the parent's attempted order list upon calling Finish();.")]
        public int taskNumber;

        void Start()
        {
            if (finishOnEnable)
                Finished();
        }

        public void Finished()
        {
            if (!taskParent.Completed)
                taskParent.executedOrder.Add(taskNumber);
        }

        public void AddTaskByNumber(int taskNumberTemp)
        {
            if (!taskParent.Completed)
                taskParent.executedOrder.Add(taskNumberTemp);
        }
    }
}
