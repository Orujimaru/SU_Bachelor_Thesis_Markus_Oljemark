using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Orujin.Core.GameHelp
{
        public class GameFloat
        {
        #region CONSTANT VALUES

        public const int Maximum = -1;
        public const int Minimum = 1;

        /***Linear interpolation***/
        public const int Linear = 0;

        /***Accelerating interpolation***/
        public const int Accelerating = 1;

        /***Decelerating interpolation***/
        public const int Decelerating = 2;

        /***Starts and ends slow, accelerates towards the middle***/
        public const int Hill = 3;

        /***Starts and ends fast, decelerates towards the middle***/
        public const int Pit = 4;

        /***Jumps to the target when the duration has been reached, does not move until that***/
        public const int Flicker = 5;

        /***Jumps back to start when the interpolation reaches the target***/
        public const int Jump = 6;

        /***Alternates between maximum and minimum***/
        public const int Alternate = 7;

        /***The interpolation will not loop.***/
        public const int Single = 8;

        /***If the duration for the interpolation doesn't matter, together with loop the value will bounce on the constrains
         * This must be used with constrained velocity and can give unexpected results if not used properly***/
        public const int NoDuration = 0;
        
        #endregion CONSTANT VALUE

        #region ATTRIBUTES

        /***The f holds the actual float and cannot be accessed by any other class***/
        private float f = float.NaN;

        /***The value returns the proper value of the f and can be accessed by any class***/
        public float value
        {     
            get
            {
                if (this.randomGenerator != null)
                {
                    //Returns a random value within the bounds of minimum and maximum
                    return (this.getDistance() * (float)this.randomGenerator.NextDouble() + this.minimum);
                }
                else
                {
                    //Make sure f has been assigned a value
                    if (!float.IsNaN(this.f))
                    {
                        return this.f;
                    }
                    else
                    {
                        return 0;
                    }
                }
            }
            private set { }
        }

        /***The value will be constrained to the minimum if it is used***/
        public float minimum { get; private set; }

        /***The value will be constrained to the maximum if it is used***/
        public float maximum { get; private set;}

        /***If the value is interpolating towards the minimum or the maximum***/
        public int target { get; private set; }

        /***How long it will take the value to reach the target***/
        public float duration { get; private set; }

        /***How long the current interpolation direction has elapsed***/
        public float elapsed { get; private set; }

        /***Specified acceleration type for the interpolation***/
        public int interpolationType { get; private set; }
        
        /***Current value of the acceleration modifier, the average acceleration value will always be 1***/
        public float currentAcceleration { get; private set; }

        /***How the interpolation is looping or if it is not looping***/
        public int loopType { get; private set; }

        /***The magnitude of the interplation***/
        public float magnitude { get; private set; }

        /***Previous velocity of the GameFloat***/
        public GameFloat velocity { get; private set; }

        /***Random to generate random numbers***/
        private Random randomGenerator = null;

        /***Static seed for Random that is incremented by every new random GameFloat***/
        private static int randomSeed = 0;

        #endregion ATTRIBUTES

        #region CONSTRUCTORS

        /***Constructor for a GameFloat that takes a single float as parameter and ignores the constrains***/
        public GameFloat(float value)
        {
            this.f = value;
            this.minimum = CONST.DoNotUseFloat;
            this.maximum = CONST.DoNotUseFloat;
            this.duration = CONST.DoNotUseInt;
            this.elapsed = CONST.DoNotUseInt;
            this.target = CONST.DoNotUseInt;
            this.interpolationType = CONST.DoNotUseInt;
            this.currentAcceleration = CONST.DoNotUseFloat;
            this.loopType = CONST.DoNotUseInt;
            this.velocity = null;
            
            this.magnitude = CONST.DoNotUseFloat;
        }

        /***Constructor for a constrained GameFloat that takes a value, a minimum, a maximum and a bool for random***/
        public GameFloat(float value, float minimum, float maximum, bool random)
        {
            this.f = value;
            this.minimum = minimum;
            this.maximum = maximum;
            this.duration = CONST.DoNotUseInt;
            this.elapsed = CONST.DoNotUseInt;
            this.target = CONST.DoNotUseInt;
            this.interpolationType = CONST.DoNotUseInt;
            this.currentAcceleration = CONST.DoNotUseFloat;
            this.loopType = CONST.DoNotUseInt;
            this.velocity = new GameFloat(0);

            if (random)
            {
                randomSeed++;
                this.randomGenerator = new Random(randomSeed);
            }

            this.magnitude = CONST.DoNotUseFloat;
        }

        /***Constructor for an interpolating GameFloat that takes a value, a target, an int for if the interpolationTyp and an int for loopingTYpe***/
        public GameFloat(float value, float target, float duration, int interpolationType, int loopType)
        {
            this.f = value;
            this.loopType = loopType;
            this.duration = duration;
            this.interpolationType = interpolationType;
            this.elapsed = 0;

            this.minimum = Math.Min(value, target);
            this.maximum = Math.Max(value, target);

            //Find in which direction the GameFloat will interpolate
            if (this.f.Equals(this.minimum))
            {
                this.target = GameFloat.Maximum;
            }
            else
            {
                this.target = GameFloat.Minimum;
            }

            //calculate the magnitude
            this.magnitude = this.getDistance() / this.duration;

            this.velocity = new GameFloat(0);
        }

        /***Constructor for interpolating GameFloat with constrains for the velocity***/
        public GameFloat(float value, float target, float duration, int interpolationType, int loopType, GameFloat velocityConstrains)
        {
            this.f = value;
            this.loopType = loopType;
            this.duration = duration;
            this.interpolationType = interpolationType;
            this.elapsed = 0;

            this.minimum = Math.Min(value, target);
            this.maximum = Math.Max(value, target);

            //Find in which direction the GameFloat will interpolate
            if (this.f.Equals(this.minimum))
            {
                this.target = GameFloat.Maximum;
            }
            else
            {
                this.target = GameFloat.Minimum;
            }

            //If there is no duration, set the magnitude to the value of the velocityConstrains.
            if (this.duration.Equals(GameFloat.NoDuration))
            {
                this.magnitude = velocityConstrains.value;
            }
            else
            {
                //calculate the magnitude
                this.magnitude = this.getDistance() / this.duration;
            }

            this.velocity = velocityConstrains;
        }

        #endregion CONSTRUCTORS

        /***Calculates the current acceleration offset***/
        private float getAcceleration()
        {
            //If elapsed has exceeded the duration, null the acceleration and force the value to the target
            //This must be done because XNA's GameTime is not precise enough when measuring time
            if (this.elapsed >= this.duration && !this.duration.Equals(GameFloat.NoDuration))
            {
                if (this.target.Equals(GameFloat.Maximum))
                {
                    this.f = this.maximum;
                }
                else if (this.target.Equals(GameFloat.Minimum))
                {
                    this.f = this.minimum;
                }
                return 0;
            }

            float acceleration = 1;

            //If elapsed is less than the duration
            switch (this.interpolationType)
            {
                case GameFloat.Accelerating:
                    {
                        //Linear acceleration starts at 0 and finishes at 2.
                        acceleration = this.elapsed / (this.duration / 2.0f);
                        break;
                    }
                case GameFloat.Decelerating:
                    {
                        //Linear deceleration starts at 2 and finishes at 0.
                        acceleration = 2.0f - this.elapsed / (this.duration / 2.0f);
                        break;
                    }
                case GameFloat.Hill:
                    {
                        //Find if the value is moving towards the middle or away from the middle
                        if (this.target.Equals(GameFloat.Maximum))
                        {
                            //The value is moving towards the middle coming from minimum
                            if ((this.f - this.minimum) <= this.getDistance() / 2.0f)
                            {
                                acceleration = 2 * (this.elapsed / (this.duration / 2.0f));
                            }

                            //The value is moving towards the maximum
                            else
                            {
                                acceleration = 4.0f - (this.elapsed / (this.duration / 2.0f) * 2);
                            }
                        }
                        else if (this.target.Equals(GameFloat.Minimum))
                        {
                            //The value is moving towards the middle coming from maximum
                            if((this.f - this.minimum) >= this.getDistance() / 2.0f)
                            {
                                acceleration = 2 * (this.elapsed / (this.duration / 2.0f));
                            }

                            //The value is moving towards the minimum
                            else
                            {
                                acceleration = 4.0f - (this.elapsed / (this.duration / 2.0f) * 2);
                            }
                        }
                        
                        break;
                    }
                case GameFloat.Pit:
                    {
                        //Find if the value is moving towards the middle or away from the middle
                        if (this.target.Equals(GameFloat.Maximum))
                        {
                            //The value is moving towards the middle coming from minimum
                            if ((this.f - this.minimum) <= this.getDistance() / 2.0f)
                            {
                                acceleration = 2.0f - (2.0f * (this.elapsed / (this.duration / 2.0f)));

                                //Don't let the acceleration reach 0
                                if (acceleration < 0.1f)
                                {
                                    acceleration = 0.1f;
                                }
                            }

                            //The value is moving towards the maximum
                            else
                            {
                                acceleration = 2.0f - (4.0f - (this.elapsed / (this.duration / 2.0f) * 2));
                            }
                        }

                        else if (this.target.Equals(GameFloat.Minimum))
                        {
                            //The value is moving towards the middle coming from maximum
                            if ((this.f - this.minimum) >= this.getDistance() / 2.0f)
                            {
                                acceleration = 2.0f - (2.0f * (this.elapsed / (this.duration / 2.0f)));

                                //Don't let the acceleration reach 0
                                if (acceleration < 0.1f)
                                {
                                    acceleration = 0.1f;
                                }
                            }

                            //The value is moving towards the minimum
                            else
                            {
                                acceleration = 2.0f - (4.0f - (this.elapsed / (this.duration / 2.0f) * 2));
                            }
                        }

                        break;
                    }
                case GameFloat.Flicker:
                    {
                        acceleration = 0;
                        break;
                    }
            }
            return acceleration;
        }

        /***Handles the interpolation of the GameFloat, returns -1 and 1 if the GameFloat is not looping and has reached the minimum/maximum value***/
        public int update(float elapsedTime)
        {
            if (this.target.Equals(CONST.DoNotUseInt))
            {
                //Do nothing
            }

            //If the GameFloat is interpolating
            else
            {
                //Increase the elapsed for the next run
                this.elapsed += elapsedTime;

                //Get the current acceleration
                this.currentAcceleration = this.getAcceleration();
                
                this.velocity.set(this.magnitude * this.currentAcceleration * elapsedTime);

                //If the GameFloat is interpolating towards the maximum
                if (this.target.Equals(GameFloat.Maximum))
                {        
                    //Increase the value until the value reaches the maximum
                    if (this.add(this.velocity.value) == GameFloat.Maximum)
                    {
                        //If the interpolation is alternating, set the target to minimum
                        if (this.loopType.Equals(GameFloat.Alternate))
                        {
                            this.target = GameFloat.Minimum;
                            this.elapsed -= this.duration;
                        }

                        /***If the interpolation should jump back to the start***/
                        else if (this.loopType.Equals(GameFloat.Jump))
                        {
                            this.f -= (this.maximum - this.minimum);
                            this.elapsed -= this.duration;
                        }

                        //If the interpolation is not looping, stop interpolating
                        else
                        {
                            this.target = CONST.DoNotUseInt;

                            //The GameFloat has reached the maximum and stopped there, report this
                            return GameFloat.Maximum;
                        }
                    }
                }

                //If the GameFloat is interpolating towards the minimum
                else if (this.target.Equals(GameFloat.Minimum))
                {
                    //Decrease the value until the value reaches the minimum
                    if (this.subtract(this.velocity.value) == GameFloat.Minimum)
                    {
                        //If the interpolation is alternating, set the target to maximum
                        if (this.loopType.Equals(GameFloat.Alternate))
                        {
                            this.target = GameFloat.Maximum;
                            this.elapsed -= this.duration;
                        }

                        /***If the interpolation should jump back to the start***/
                        else if (this.loopType.Equals(GameFloat.Jump))
                        {
                            this.f += (this.maximum - this.minimum);
                            this.elapsed -= this.duration;
                        }

                        //If the interpolation is not looping, stop interpolating
                        else
                        {
                            this.target = CONST.DoNotUseInt;

                            //The GameFloat has reached the minimum and stopped there, report this
                            return GameFloat.Minimum;
                        }
                    }
                }
            }

            //Nothing to report
            return 0;
        }

        #region VALUE_MODIFIERS

        /***Adds a float to the value and constrains it, returns what action has been made***/
        public int add(float f)
        {
           return this.constrainValue(this.f + f);
        }

        /***Subtracts a float from the value and constrains it, returns what action has been made***/
        public int subtract(float f)
        {
           return this.constrainValue(this.f - f);
        }

        /***Divides the value by a float and constrains it, returns what action has been made***/
        public int divide(float f)
        {
            return this.constrainValue(this.f / f);
        }

        /***Multiplies the value by a float and constrains it, returns what action has been made***/
        public int multiply(float f)
        {
            return this.constrainValue(this.f / f);
        }

        /***Constrains the value and reports what action has been made***/
        private int constrainValue(float newValue)
        {
            if (this.minimum != CONST.DoNotUseFloat)
            {
                //If the new value is less than the minimum, constrain it to the minimum value and return CONST.MINIMUM
                if (newValue <= this.minimum)
                {
                    this.f = this.minimum;
                    return GameFloat.Minimum;
                }
            }

            if (this.maximum != CONST.DoNotUseFloat)
            {
            //If the new value is greater than the maximum, constrain it to the maximum value and return CONST.MAXIMUM
                if (newValue >= this.maximum)
                {

                    this.f = this.maximum;
                    return GameFloat.Maximum;
                }
            }

            //The new value is within the constrains, there is nothing to report
            this.f = newValue;

            //Nothing to report
            return 0;
        }

        /***Set the GameFloat to a new value.***/
        public int set(float newValue)
        {
            return this.constrainValue(newValue);
        }

        #endregion VALUE_MODIFIERS

        /***Returns a float with the distance between minimum and maximum***/
        public float getDistance()
        {
            return this.maximum - this.minimum;
        }

        /***Returns a float with the size of this GameFloat in relation to a second GameFloat, the value will be between 0.0 and 1.0***/
        public float getOffset(GameFloat second)
        {
            float total = this.getDistance() + second.getDistance();
            float offset = this.getDistance() / total;
            return offset;
        }
    }
}
