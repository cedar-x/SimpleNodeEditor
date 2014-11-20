﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

namespace SimpleNodeEditor
{
    public class DelayedSignal
    {
        public float RunningTime = 0.0f;
        public Signal Signal = null;
    }

    [NodeMenuItem("DelayNode", typeof(DelayNode))]
    public class DelayNode : BaseNode
    {
        public float Delay = 1.0f;

        [SerializeField]
        Inlet inlet = null;

        [SerializeField]
        Outlet outlet = null;

        List<DelayedSignal> m_delayedSignals = new List<DelayedSignal>();

        void OnInlet(Signal signal)
        {
            DelayedSignal delayedSignal = new DelayedSignal();
            delayedSignal.Signal = signal;

            m_delayedSignals.Add(delayedSignal);
        }

        protected override void Inited()
        {
            inlet.SlotReceivedSignal += OnInlet;
        }

        public override void Construct()
        {
            Name = "DelayNode";

            inlet = (Inlet) MakeLet(LetTypes.INLET);

            outlet = (Outlet) MakeLet(LetTypes.OUTLET);
            outlet.yOffset = 25;
        }

        public override void WindowCallback(int id)
        {
            GUI.BeginGroup(new Rect(5, 50, 100, 50));
            EditorGUIUtility.LookLikeControls(50, 50);
            Delay = EditorGUILayout.FloatField("Delay", Delay, GUILayout.MaxWidth(80));
            GUI.EndGroup();

            base.WindowCallback(id);
        }

        void Update()
        {
            if( m_delayedSignals.Count > 0 )
            {
                List<DelayedSignal> SignalsToRemove = null;

                foreach (DelayedSignal delayedSignal in m_delayedSignals)
                {
                    delayedSignal.RunningTime += Time.deltaTime;
                    if (delayedSignal.RunningTime > Delay)
                    {
                        outlet.Emit(delayedSignal.Signal);

                        if (SignalsToRemove == null)
                            SignalsToRemove = new List<DelayedSignal>();

                        SignalsToRemove.Add(delayedSignal);
                    }
                }

                if( SignalsToRemove != null )
                {
                    foreach(DelayedSignal delayedSignal in SignalsToRemove)
                    {
                        m_delayedSignals.Remove(delayedSignal);
                    }
                }
            }
        }
    }
}