using UnityEngine;
using System.Collections;

public class EventMicExample : MonoBehaviour {

    public GameObject AudioBeat;
    public string selectedDevice { get; private set; }  //Mic selected
    private bool micSelected = false;                   //Mic flag
    private bool started = false;                       //Flaf to see if detection has started		
    private int minFreq, maxFreq; 						//Max and min frequencies window
    private ParticleControll particles;

    public void MyCallbackEventHandler(BeatDetection.EventInfo eventInfo)
    {
        switch (eventInfo.messageInfo)
        {
            case BeatDetection.EventType.Energy:
                StartCoroutine(ShakeParticles());
                Debug.Log("Energy");
                break;
            case BeatDetection.EventType.HitHat:
                Debug.Log("HitHat");
                break;
            case BeatDetection.EventType.Kick:
                Debug.Log("Kick");
                break;
            case BeatDetection.EventType.Snare:
                Debug.Log("Snare");
                particles.EmitNote();
                break;
        }
    }
    
    IEnumerator ShakeParticles()
    {
        particles.shakeParticlesOn = true;
        yield return new WaitForSeconds(0.1f);
        particles.shakeParticlesOn = false;
    }

    // Use this for initialization
    void Start()
    {
        particles = GetComponent<ParticleControll>();
        //Register the beat callback function
        AudioBeat.GetComponent<BeatDetection>().CallBackFunction = MyCallbackEventHandler;

        //Set up mic
        started = false;
        selectedDevice = Microphone.devices[0].ToString();
        micSelected = true;
        GetMicCaps();
        setUptMic();
        StartCapture();
        //End setup mic
    }

    //Start Mic
    public void StartCapture()
    {
        if (started)
            return;

        //start capture volume
        StartMicrophone();
        started = true;
    }

    //Stop Mic
    public void StopCapture()
    {
        StopMicrophone();
        started = false;
    }

    //Setup mic as device
    void setUptMic()
    {
        AudioBeat.GetComponent<AudioSource>().clip = null;
        AudioBeat.GetComponent<AudioSource>().loop = true; // Set the AudioClip to loop
        AudioBeat.GetComponent<AudioSource>().mute = false; // Mute the sound, we don't want the player to hear it
    }

    //Get mic capabilities
    public void GetMicCaps()
    {
        Microphone.GetDeviceCaps(selectedDevice, out minFreq, out maxFreq);//Gets the frequency of the device
        if ((minFreq + maxFreq) == 0)
            maxFreq = 44100;
    }

    //True start mic
    public void StartMicrophone()
    {
        AudioBeat.GetComponent<AudioSource>().clip = Microphone.Start(selectedDevice, true, 10, maxFreq); //Starts recording
        while (!(Microphone.GetPosition(selectedDevice) > 0)) { } // Wait until the recording has started
        AudioBeat.GetComponent<AudioSource>().Play(); // Play the audio source!
    }

    //True stop mic
    public void StopMicrophone()
    {
        AudioBeat.GetComponent<AudioSource>().Stop();//Stops the audio
        Microphone.End(selectedDevice);//Stops the recording of the device	
    }
}
