using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using Orujin.Core.Renderer;

namespace Orujin.Framework
{
    public class Trigger : GameObject
    {
        public bool triggered { get; private set; }
        public Identity triggerer { get; private set; }
        public int conditionId { get; private set; }
        public int currentTriggerCount { get; private set; }
        public int maxTriggerCount { get; private set; }
        public bool consumed { get; private set; }

        public Trigger(Vector2 position, Rectangle rectangle, string name, int conditionId, Identity triggerer, World world, int maxTriggerCount, bool constant) 
            : base(position, name, "Trigger")
        {
            Body body = BodyFactory.CreateRectangle(world, rectangle.Width / Camera.PixelsPerMeter, rectangle.Height / Camera.PixelsPerMeter, 1);
            body.BodyType = BodyType.Static;
            body.IsSensor = true;
            base.AddBody(body);
            this.triggerer = triggerer;
            this.triggered = false;
            this.conditionId = conditionId;
            this.consumed = false;
            this.maxTriggerCount = maxTriggerCount;
            this.currentTriggerCount = 0;
            GameManager.game.AddObject(this);
        }

        public override bool OnCollisionEnter(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            if (!this.triggered && !this.consumed)
            {
                if (fixtureB.Body.parent != null && fixtureB.Body.parent.identity.Equals(this.triggerer))
                {
                     this.triggered = true;
                }
            }
            return false;
        }

        public void Register()
        {
            if (this.maxTriggerCount != 0)
            {
                this.currentTriggerCount++;
                if (this.maxTriggerCount == this.currentTriggerCount)
                {
                    this.consumed = true;
                }
            }
            this.triggered = false;       
        }
    }
}
