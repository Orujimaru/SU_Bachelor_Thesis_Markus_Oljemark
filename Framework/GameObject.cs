using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Orujin.Core.Renderer;

namespace Orujin.Framework
{
    public struct Identity
    {
        public string name;
        public string tag;

        public bool Equals(Identity other)
        {
            if (name.Equals(other.name) && tag.Equals(other.tag))
            {
                return true;
            }
            return false;
        }

        public bool PartlyEquals(Identity other)
        {
            if (name.Equals(other.name) || tag.Equals(other.tag))
            {
                return true;
            }
            return false;
        }
    }

    public class GameObject
    {
        /***Farseer Physics body of the game object, use addBody() to initialize it***/
        private Body physicsBody = null;

        /***Public accessor for the Farseer Physics Body ***/
        public Body body { get { return physicsBody; } private set { return; } }

        //List of all the current collisions
        protected List<Fixture> currentCollisions = new List<Fixture>();
        public Vector2 scrollSpeed { get; private set; }

        /***RendererComponent contains all the attributes the Renderer needs to render the object***/
        public RendererComponents rendererComponents { get; private set; }
        
        private Vector2 previousPosition;
        protected Vector2 position;

        protected Vector2 initialPosition;
        protected bool constant = false;

        public Vector2 distanceMoved
        {
            get
            {
                return position - previousPosition;
            }
            private set { return; }
        }

        public Vector2 origin {
            get
            {
                if (prioritizeFarseerPhysics)
                {
                    return (this.physicsBody.Position * Camera.PixelsPerMeter);
                }
                return this.position - Camera.adjustedPosition * (this.scrollSpeed - new Vector2(1,1));
            }
            private set
            {
                return;
            }
        }
        public Vector2 nextVelocity { get; private set; }
        public float velocityModifier { get; private set; }

        public Identity identity { get; private set; }

        public bool checkForInput = false;
        public bool checkForPixelCollision = false;

        public bool prioritizeFarseerPhysics = false;

        private static int idGen = 0;
        public int id { get; private set; }

        public GameObject(Vector2 position, string name, string tag)
        {
            this.initialPosition = position;
            this.position = position;
            this.previousPosition = position;
            this.nextVelocity = Vector2.Zero;
            this.velocityModifier = 0.2f;
            this.rendererComponents = new RendererComponents(this);
           
            Identity identity = new Identity();
            identity.name = name;
            identity.tag = tag;
            this.identity = identity;
            this.constant = false;
            idGen += 1;
            this.id = idGen;
            this.scrollSpeed = new Vector2(1, 1);
        }

        public void SetScrollSpeed(Vector2 newScrollSpeed)
        {
            this.scrollSpeed = newScrollSpeed;
        }

        public virtual void Update(float elapsedTime)
        {
            if (this.constant)
            {
                if (this.physicsBody != null)
                {
                    this.position = initialPosition + Camera.adjustedPosition;
                    this.physicsBody.SetTransform(this.position / Camera.PixelsPerMeter, this.physicsBody.Rotation);
                }
                else
                {
                    this.position = initialPosition;
                }
            }
            else
            {
                this.previousPosition = position;
                this.HandlePhysicsBody();
                this.HandleMove(elapsedTime);
            }          
            this.rendererComponents.Update(elapsedTime);
            this.CheckContiniousCollisions();
        }

        private void HandleMove(float elapsedTime)
        {
            if (!this.prioritizeFarseerPhysics && this.nextVelocity != Vector2.Zero)
            {
                Vector2 velocity = this.nextVelocity * elapsedTime/10.0f;
                this.nextVelocity = Vector2.Zero;

                this.position += velocity;
                if (this.physicsBody != null)
                {
                    this.physicsBody.SetTransform(new Vector2(this.physicsBody.Position.X + (velocity.X / Camera.PixelsPerMeter), this.physicsBody.Position.Y + (velocity.Y / Camera.PixelsPerMeter)), this.physicsBody.Rotation);
                }
            }
        }

        protected virtual void NoCollision()
        {
        }

        private void CheckContiniousCollisions()
        {
            if (this.body != null)
            {
                ContactEdge contactEdge = null;
                contactEdge = this.body.ContactList;

                int numberOfAllowedCollisions = 0;

                while (contactEdge != null)
                {
                    if (contactEdge.Contact.IsTouching())
                    {
                        Fixture a = contactEdge.Contact.FixtureA;
                        Fixture b = contactEdge.Contact.FixtureB;
                        if (!b.IsSensor && a.Body != b.Body)
                        {
                            numberOfAllowedCollisions++;
                        }
                        this.OnContiniousCollision(a, b, contactEdge.Contact);
                    }
                    contactEdge = contactEdge.Next;
                }

                if (numberOfAllowedCollisions == 0)
                {
                    this.NoCollision();
                }
            }
        }

        public int HasCollisions()
        {
            if (this.body != null)
            {
                ContactEdge contactEdge = null;
                contactEdge = this.body.ContactList;

                int numberOfAllowedCollisions = 0;

                while (contactEdge != null)
                {
                    if (contactEdge.Contact.IsTouching())
                    {
                        Fixture a = contactEdge.Contact.FixtureA;
                        Fixture b = contactEdge.Contact.FixtureB;
                        if (!b.IsSensor && a.Body != b.Body)
                        {
                            numberOfAllowedCollisions++;
                        }
                    }
                    contactEdge = contactEdge.Next;
                }

                return numberOfAllowedCollisions;
            }
            return 0;
        }

        private void HandlePhysicsBody()
        {
            if (this.physicsBody != null)
            {
                Transform transform;
                this.physicsBody.GetTransform(out transform);
                this.position = this.physicsBody.Position * Camera.PixelsPerMeter;
            }
        }

        public void Move(Vector2 direction, float magnitude)
        {
            this.nextVelocity += direction * magnitude;
        }

        public void SetPosition(Vector2 newPosition)
        {
            this.position = newPosition;
            if (this.physicsBody != null)
            {
                this.physicsBody.Position = newPosition / Camera.PixelsPerMeter;
            }
        }

        public void AddForce(Vector2 direction, float magnitude)
        {
            if (this.physicsBody != null)
            {
                this.physicsBody.ApplyForce((direction * magnitude));
                this.HandlePhysicsBody();
            }
        }

        public void SetVelocity(Vector2 direction, float magnitude)
        {
            if (this.physicsBody != null)
            {
                this.physicsBody.LinearVelocity = (direction * magnitude);
                this.HandlePhysicsBody();
            }
        }

        public void ApplyLinearImpulse(Vector2 direction, float magnitude)
        {
            if (this.physicsBody != null)
            {
                this.physicsBody.ApplyLinearImpulse(direction * magnitude);
            }
        }

        public RendererComponents GetRendererComponents()
        {
            return this.rendererComponents;
        }

        public void AddBody(Body body)
        {
            this.physicsBody = body;
            this.prioritizeFarseerPhysics = true;
            this.body.OnCollision += OnCollisionEnter;
            this.body.OnSeparation += OnCollisionExit;
            this.physicsBody.SetParent(this);
            this.physicsBody.Position = this.position / Camera.PixelsPerMeter;
        }

        public void Destroy()
        {
            this.DestroyBody();
        }

        public void DestroyBody()
        {
            if (physicsBody != null)
            {
                physicsBody.Dispose();
                physicsBody = null;
                this.prioritizeFarseerPhysics = false;
            }
        }
        public void ShowOrigin(Texture2D originTexture)
        {
            this.rendererComponents.Debug(originTexture);
        }

        public void HideOrigin()
        {
            this.rendererComponents.StopDebugging();
        }

        public virtual void OnContiniousCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
        }

        public virtual bool OnCollisionEnter(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            return true;
        }

        public virtual void OnCollisionExit(Fixture fixtureA, Fixture fixtureB)
        {
        }

        public Rectangle GetBounds()
        {
            Rectangle bounds = this.rendererComponents.bounds;
            bounds.X = (int) (this.origin.X - bounds.Width / 2);
            bounds.Y = (int) (this.origin.Y - bounds.Height / 2);
            return bounds;
        }

        public bool IsInActiveArea()
        {
            return this.GetBounds().Intersects(Camera.activeBounds);
        }

        public bool IsInFrame()
        {
            return this.GetBounds().Intersects(Camera.matrixBounds);
        }

        protected void Initialize()
        {
            this.rendererComponents.SetBounds();
        }
    }
}
