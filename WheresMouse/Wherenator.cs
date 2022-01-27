using System;
using System.Numerics;

namespace WheresMouse
{

    public class Wherenator
    {

        public bool first = true;
        public Vector2 prevPos;

        public double distAccum = 0.0;
        public double distDecayFactorInactive = 0.95;
        public double distDecayFactorActive = 0.9;
        public double distHystMin = 100.0;
        public double distHystMax = 1500.0;

        public bool active = false;
        public double activePower;

        public void Update(Vector2 pos)
        {
            if (first == true)
            {
                prevPos = pos;
                first = false;
                return;
            }
            if (active == true)
            {
                if (distAccum < distHystMin)
                {
                    active = false;
                }
            }
            else
            {
                distAccum += Math.Sqrt(Math.Pow(pos.X - prevPos.X, 2.0) + Math.Pow(pos.Y - prevPos.Y, 2.0));
                if (distAccum >= distHystMax)
                {
                    active = true;
                }
            }
            if (active == true)
            {
                double distcap = distAccum > distHystMax ? distHystMax : distAccum;
                activePower = (distcap - distHystMin) / (distHystMax - distHystMin) * 100.0;
                distAccum *= distDecayFactorActive;
            }
            else
            {
                distAccum *= distDecayFactorInactive;
            }
            prevPos = pos;
        }

    }

}
