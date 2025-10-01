using System.Collections.Generic;
using AdvancedMonoBehaviour.Scripts.Interfaces;
using AdvancedMonoBehaviour.Scripts.Patterns;
using UnityEngine;

namespace AdvancedMonoBehaviour.Scripts.Base
{
    public class MonoGod : AdvancedSingleton<MonoGod>
    {

        #region System Variables

        protected Dictionary<int, List<AdvancedMono>> Registered = new Dictionary<int, List<AdvancedMono>>();
        [SerializeField] protected Dictionary<int, List<OnStart>> OnStart = new Dictionary<int, List<OnStart>>();
        [SerializeField] protected Dictionary<int, List<OnUpdate>> OnUpdate = new Dictionary<int, List<OnUpdate>>();
        [SerializeField] protected List<float> WeightFrameRateToDispose = new List<float>();
        
        public float timer, refresh, avgFramerate;


        #endregion

        #region Register

        public void Register(AdvancedMono script, int weight, OnUpdate onUpdate, OnStart onStart)
        {
            if(!Registered[weight].Contains(script))
                Registered[weight].Add(script);
            else
            {
                return;
            }
            
            if(onStart != null && !OnStart[weight].Contains(onStart))
                OnStart[weight].Add(onStart);
            
            if(onUpdate != null && !OnUpdate[weight].Contains(onUpdate))
                OnUpdate[weight].Add(onUpdate);
        }
        public void DeRegister(AdvancedMono script)
        {

        }

        #endregion
        
        #region Unity Native Bridge Crossing.

        public override void Awake()
        {
            base.Awake();
            Registered.Add(0, new List<AdvancedMono>());
            Registered.Add(1, new List<AdvancedMono>());
            Registered.Add(2, new List<AdvancedMono>());
            Registered.Add(3, new List<AdvancedMono>());
            Registered.Add(4, new List<AdvancedMono>());
            Registered.Add(5, new List<AdvancedMono>());
            
            
            OnStart.Add(0, new List<OnStart>());
            OnStart.Add(1, new List<OnStart>());
            OnStart.Add(2, new List<OnStart>());
            OnStart.Add(3, new List<OnStart>());
            OnStart.Add(4, new List<OnStart>());
            OnStart.Add(5, new List<OnStart>());
            
            OnUpdate.Add(0, new List<OnUpdate>());
            OnUpdate.Add(1, new List<OnUpdate>());
            OnUpdate.Add(2, new List<OnUpdate>());
            OnUpdate.Add(3, new List<OnUpdate>());
            OnUpdate.Add(4, new List<OnUpdate>());
            OnUpdate.Add(5, new List<OnUpdate>());
            
            WeightFrameRateToDispose.Add(0);
            WeightFrameRateToDispose.Add(20);
            WeightFrameRateToDispose.Add(30);
            WeightFrameRateToDispose.Add(40);
            WeightFrameRateToDispose.Add(50);
            WeightFrameRateToDispose.Add(55);

        }
    
        private void Start()
        {
            for (int ii = 1; ii < 6; ii++)
            {
                for (int i = 0; i < OnStart[ii].Count; i++)
                {
                    OnStart[ii][i].OnStart();
                }
            }
        }

        private void LateUpdate()
        {
            float timelapse = Time.smoothDeltaTime;
            timer = timer <= 0 ? refresh : timer -= timelapse;
 
            if(timer <= 0) avgFramerate = (int) (1f / timelapse);
        }

        private void Update()
        {
            for (int i = 0; i < OnUpdate[0].Count; i++)
            {
                OnUpdate[0][i].OnUpdate();
            }
            
            for (int ii = 1; ii < 6; ii++)
            {
                if (avgFramerate < WeightFrameRateToDispose[ii])
                {
                    return;
                }

                for (int i = 0; i < OnUpdate[ii].Count; i++)
                {
                    OnUpdate[ii][i].OnUpdate();
                }
            }
        }
    
        private void FixedUpdate()
        {
        
        }
    
        private void Reset()
        {
        
        }

        #endregion
    }
}