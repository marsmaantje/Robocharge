using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GXPEngine;

namespace Animation
{
    class Tween : Pivot
    {
        float from;
        float to;
        float duration;
        float startTime;
        public delegate float EaseMethod(float a, float b, float t);
        EaseMethod easing;


        //Not (yet) implemented, thought to myself: "Do I really need this?" and the answer was no...
        public Tween(EaseMethod method, ref float targetValue)
        {
            easing = method;
        }

        public void Update()
        {

        }
    }
}
