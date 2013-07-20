#pragma strict

var audioClips : AudioClip[];

private var prcReverb : PRCReverbFilter;
private var nReverb : NReverbFilter;

private var reverbType = 0;
private var decayTime = 2.0;
private var sendLevel = 0.2;

private function SelectClip(index : int) {
    audio.clip = audioClips[index];
    audio.Play();
}

function Awake() {
    prcReverb = FindObjectOfType(PRCReverbFilter);
    nReverb = FindObjectOfType(NReverbFilter);
}

function Update() {
    prcReverb.enabled = (reverbType == 0);
    nReverb.enabled = (reverbType == 1);
    prcReverb.decayTime = nReverb.decayTime = decayTime;
    prcReverb.sendLevel = nReverb.sendLevel = sendLevel;
}

function OnGUI() {
    GUILayout.BeginArea(Rect(16, 16, Screen.width - 32, Screen.height - 32));
    GUILayout.FlexibleSpace();

    GUILayout.Label("Audio sources");
    GUILayout.BeginHorizontal();
    if (GUILayout.Button("Click tone")) SelectClip(0);
    if (GUILayout.Button("Acoustic drums")) SelectClip(1);
    if (GUILayout.Button("Synth drum loop")) SelectClip(2);
    if (GUILayout.Button("Electric Piano")) SelectClip(3);
    if (GUILayout.Button("TTS voice")) SelectClip(4);
    GUILayout.EndHorizontal();

    GUILayout.FlexibleSpace();

    GUILayout.Label("Reverb type (current: " + (reverbType == 0 ? "PRCReverb" : "NReverb") + ")");
    GUILayout.BeginHorizontal();
    if (GUILayout.Button("PRCReverb")) reverbType = 0;
    if (GUILayout.Button("NReverb")) reverbType = 1;
    GUILayout.EndHorizontal();

    GUILayout.FlexibleSpace();

    GUILayout.Label("Decay time = " + decayTime.ToString("0.00") + " sec");
    decayTime = GUILayout.HorizontalSlider(decayTime, 0.0, 10.0);

    GUILayout.FlexibleSpace();

    GUILayout.Label("Send level = " + (sendLevel * 100).ToString("0") + " %");
    sendLevel = GUILayout.HorizontalSlider(sendLevel, 0.0, 1.0);

    GUILayout.FlexibleSpace();
    GUILayout.EndArea();
}