// STK chorus effect class, based on CCRMA STK library.
// https://ccrma.stanford.edu/software/stk/
using UnityEngine;
using System.Collections;

public class StkChorus : MonoBehaviour
{
    // Base delay time.
    [Range(0.0f, 0.5f)]
    public float
        baseDelay = 0.1f;

    // Modulation depth.
    [Range(0.0f, 1.0f)]
    public float
        depth = 0.1f;

    // Modulation frequency.
    [Range(0.0f, 10.0f)]
    public float
        freq = 0.2f;

    // Wet signal ratio.
    [Range(0.0f, 1.0f)]
    public float
        wetMix = 0.5f;

    // Delay lines.
    Stk.DelayLine delay1;
    Stk.DelayLine delay2;
    int baseDelayInSamples;

    // Modulations.
    Stk.SineWave mod1;
    Stk.SineWave mod2;

    // Used for error handling.
    string error;

    void UpdateParameters ()
    {
        mod1.Frequency = freq;
        mod2.Frequency = freq * 1.1111f;
    }

    void Awake ()
    {
        baseDelayInSamples = (int)(AudioSettings.outputSampleRate * baseDelay);
        int maxDelay = (int)(baseDelayInSamples * 1.414f) + 2;
        delay1 = new Stk.DelayLine (baseDelayInSamples, maxDelay);
        delay2 = new Stk.DelayLine (baseDelayInSamples, maxDelay);
        mod1 = new Stk.SineWave ();
        mod2 = new Stk.SineWave ();
        UpdateParameters ();
    }

    void Update ()
    {
        if (error == null) {
            UpdateParameters ();
        } else {
            Debug.LogError (error);
            Destroy (this);
        }
    }

    void OnAudioFilterRead (float[] data, int channels)
    {
        if (channels != 2) {
            error = "This filter only supports stereo audio (given:" + channels + ")";
            return;
        }
        
        for (var i = 0; i < data.Length; i += 2) {
            var input = 0.5f * (data [i] + data [i + 1]);

            delay1.Delay = (int)(baseDelayInSamples * (1.0f + depth * mod1.Tick ()) * 0.707f);
            delay2.Delay = (int)(baseDelayInSamples * (1.0f + depth * mod2.Tick ()) * 0.5f);

            var dryMix = 1.0f - wetMix;
            var out1 = wetMix * delay1.Tick (input) + dryMix * data [i];
            var out2 = wetMix * delay2.Tick (input) + dryMix * data [i + 1];

            data [i] = out1;
            data [i + 1] = out2;
        }
    }
}
