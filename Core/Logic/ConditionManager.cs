using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Orujin.Framework;

namespace Orujin.Core.Logic
{
    public class GameEventConditionManager
    {
        private List<GameEventCondition> conditions;
        private List<int> remotelyFulfilledConditions;

        public GameEventConditionManager()
        {
            this.conditions = new List<GameEventCondition>();
            this.remotelyFulfilledConditions = new List<int>();
        }

        public void Update(float elapsedTime, List<GameObject> objects)
        {
            foreach (int i in remotelyFulfilledConditions)
            {
                if (!this.conditions[i].fulfilled)
                {
                    this.conditions[i].Fulfill();
                }
            }
            remotelyFulfilledConditions.Clear();

            for(int x = 0; x < objects.Count; x++)
            {
                if (objects[x] is Trigger)
                {
                    Trigger t = (Trigger)objects[x];
                    if (t.triggered && !this.conditions[t.conditionId].fulfilled)
                    {
                        this.conditions[t.conditionId].Fulfill();
                        t.Register();
                    }
                }
            }

            foreach (GameEventCondition ec in this.conditions)
            {
                if (ec != null)
                {
                    ec.Update(elapsedTime); //Pass down objects too?
                }
            }
        }

        public void RemotelyFulfillCondition(int i)
        {
            this.remotelyFulfilledConditions.Add(i);
        }

        public void AddCondition(GameEventCondition newCondition)
        {
            if (newCondition.id >= conditions.Count)
            {
                for (int x = this.conditions.Count; x <= newCondition.id; x++)
                {
                    this.conditions.Add(null);
                }
            }
            this.conditions[newCondition.id] = newCondition;
        }

        public void RemoveCondition(int id)
        {
            this.conditions[id] = null;
        }


    }
}
