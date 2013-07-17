// Perry's simple reverberator class, based on CCRMA STK library.
// https://ccrma.stanford.edu/software/stk/

using UnityEngine;
using System.Collections;

public class PRCReverb : MonoBehaviour
{
    // T60 decay time.
    [Range(0.0f, 4.0f)]
    public float
        decayTime = 1.0f;

    // Wet signal ratio.
    [Range(0.0f, 1.0f)]
    public float
        wetMix = 0.1f;

    // Delay lines.
    Stk.DelayLine allpass1;
    Stk.DelayLine allpass2;
    Stk.DelayLine comb1;
    Stk.DelayLine comb2;

    // Filter coefficients.
    float allpassCoeff = 0.7f;
    float comb1Coeff;
    float comb2Coeff;

    // Used for error handling.
    string error;

    void UpdateParameters ()
    {
        float scaler = -3.0f / (decayTime * AudioSettings.outputSampleRate);
        comb1Coeff = Mathf.Pow (10.0f, scaler * comb1.Delay);
        comb2Coeff = Mathf.Pow (10.0f, scaler * comb2.Delay);
    }

    void Awake ()
    {
        // Delay length for 44100 Hz sample rate.
        int[] delays = {341, 613, 1557, 2137};
        
        // Scale the delay lengths if necessary.
        var sampleRate = AudioSettings.outputSampleRate;
        if (sampleRate != 44100) {
            var scaler = sampleRate / 44100.0f;
            for (var i = 0; i < delays.Length; i++) {
                var delay = Mathf.FloorToInt (scaler * delays [i]);
                if ((delay & 1) == 0) {
                    delay++;
                }
                while (!Stk.Math.IsPrime(delay))
                    delay += 2;
                delays [i] = delay;
            }
        }

        allpass1 = new Stk.DelayLine (delays [0]);
        allpass2 = new Stk.DelayLine (delays [1]);
        comb1 = new Stk.DelayLine (delays [2]);
        comb2 = new Stk.DelayLine (delays [3]);

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
            
            var temp0 = allpassCoeff * allpass1.NextOut;
            temp0 += input;
            temp0 = allpass1.Tick (temp0) - allpassCoeff * temp0;
            
            var temp1 = allpassCoeff * allpass2.NextOut;
            temp1 += temp0;
            temp1 = allpass2.Tick (temp1) - allpassCoeff * temp1;

            var out1 = comb1.Tick (temp1 + comb1Coeff * comb1.NextOut);
            var out2 = comb2.Tick (temp1 + comb2Coeff * comb2.NextOut);

            out1 = wetMix * out1 + (1.0f - wetMix) * data [i];
            out2 = wetMix * out2 + (1.0f - wetMix) * data [i + 1];

            data [i] = out1;
            data [i + 1] = out2;
        }
    }
}
