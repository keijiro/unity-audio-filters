namespace Stk
{
    public class SineWave
    {
        float phase;
        float delta;
        float lastOut;

        public float LastOut {
            get { return lastOut;}
        }

        public float Frequency {
            set { delta = value / Config.SampleRate; }
        }

        public float Tick ()
        {
            lastOut = UnityEngine.Mathf.Sin (phase * 6.28318530718f);
            phase += delta;
            phase -= (int)phase;
            return lastOut;
        }
    }
}
