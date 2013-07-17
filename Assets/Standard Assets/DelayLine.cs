namespace Stk
{
    public class DelayLine
    {
        float[] buffer;
        float lastOut;
        int inPoint;
        int outPoint;
        int delay;

        public int MaxDelay {
            get { return buffer.Length - 1; }
        }

        public int Delay {
            get { return delay; }
            set {
                if (value <= inPoint) {
                    outPoint = inPoint - value;
                } else {
                    outPoint = inPoint + buffer.Length - value;
                }
                delay = value;
            }
        }
    
        public float LastOut {
            get { return lastOut; }
        }

        public float NextOut {
            get { return buffer [outPoint]; }
        }
    
        public DelayLine (int delay, int maxDelay = 4095)
        {
            buffer = new float[maxDelay + 1];
            Delay = delay;
        }
    
        public float Tick (float input)
        {
            buffer [inPoint++] = input;
            if (inPoint == buffer.Length) {
                inPoint = 0;
            }
            
            lastOut = buffer [outPoint++];
            if (outPoint == buffer.Length) {
                outPoint = 0;
            }
            
            return lastOut;
        }
    }
}
