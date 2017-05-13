using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Kino;


public class ParticleControll : MonoBehaviour {
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem.MainModule particleSystemMainModule;
    public Vector2 shakeRange;
    public Vector3 allowedDistance;
    public bool shakeParticlesOn;
    private int particleCountGlobal = 0;
    public float particleSpeedXY;
    private int[] musicNumbers = new int[] {2,3,4,5,6,7,8,7,6,5,4,3,2,3};
    private int totalNotes = 0;
    public Color[] musicBar;
    public bool circle = false;
    private Vector2 center = Vector2.zero;
    public float speed = 1;
    public float diameter = 10;
    public float circleTolerance = 0.2f;
    private PlexusParticles plexus;
    public ColorCorrectionCurves CameraColorCorrection;
    public AnalogGlitch CameraAnalog;   
    public DigitalGlitch CameraDigital;
    public Vector2 particleInitialPos;
    public GameObject warpSpeed;
    private bool warpActive = false;


    // Use this for initialization
    void Start () {
        for (int i = 0; i < musicNumbers.Length ;i++)
        {
            totalNotes += musicNumbers[i];
        }
        
        plexus = GetComponent<PlexusParticles>();
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;
    }
	

	// Update is called once per frame
	void Update () {

        if (Input.GetButtonUp("Jump"))
        {
            EmitNote();
        }
        /*
        if (Input.GetButton("Fire1"))
        {
            shakeParticlesOn = true;        
        } else
        {
            shakeParticlesOn = false;
        }*/

        if (Input.GetKeyDown(KeyCode.Return))
        {
            MakeParticlesSphere();
        }

        if (Input.GetKeyDown(KeyCode.RightShift))
        {
            MultiplyParticles(1);
        }

        if (Input.GetKeyDown(KeyCode.RightControl))
        {
            plexus.triangles = !plexus.triangles;
        }

        if (Input.GetKeyDown(KeyCode.Backslash))
        {
            circle = !circle;
        }

        if (shakeParticlesOn)
        {
            ShakeParticles();
        }
        if (circle)
        {
            GravityAttractor();
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            CameraColorCorrection.enabled = !CameraColorCorrection.enabled;
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            CameraAnalog.enabled = !CameraAnalog.enabled;
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            CameraDigital.enabled = !CameraDigital.enabled;
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            warpActive = !warpActive;
            warpSpeed.SetActive(warpActive);
        }


    }



    Color GetColor()
    {
        int count = (particleCountGlobal % totalNotes) + 1;
        int color = 0;

        for (int i = 0; i < musicNumbers.Length; i++)
        {
            count -= musicNumbers[i];
            if (count <= 0)
            {
                break;
            }
            else
            {
                color++;
            }
        }

/*        Debug.Log("count " + count);
        Debug.Log("particleGlobal " + particleCountGlobal);
        Debug.Log("color " + color ); 
s*/
        return musicBar[color];
    }

    public void EmitNote()
    {
        var emitParams = new ParticleSystem.EmitParams();
        emitParams.startColor = GetColor();
        emitParams.position = new Vector3(Random.Range(-particleInitialPos.x, particleInitialPos.x), Random.Range(-particleInitialPos.y, particleInitialPos.y), -11.1f);
        emitParams.velocity = new Vector3(Random.Range(-particleSpeedXY, particleSpeedXY), Random.Range(-particleSpeedXY, particleSpeedXY), 0.5f);
        particleSystem.Emit(emitParams, 1);
        particleCountGlobal++;
    }

    void MakeParticlesSphere()
    {
        StartCoroutine(EmitNumberOfParticles(100, 5.0f));
    }

    IEnumerator EmitNumberOfParticles (int number, float seconds)
    {
        for (int i = 0; i < number; i++)
        {
            particleSystem.Emit(1);
            yield return new WaitForSeconds(seconds / number);
        }
        

    }

    void MultiplyParticles(int number)
    {
        int particleCount = particleSystem.particleCount;


        for (int i = 0; i < particleCount-1; i++)
        {
            for(int j = 0; j < number; j++)
            {
                var emitParams = new ParticleSystem.EmitParams();
                emitParams.startColor = particles[i].startColor;
                emitParams.velocity = new Vector3(Random.Range(-particleSpeedXY, particleSpeedXY), Random.Range(-particleSpeedXY, particleSpeedXY), particles[i].velocity.z);
                emitParams.position = particles[i].position;
                particleSystem.Emit(emitParams, 1);
            }
        }
        
    }

    void GravityAttractor ()
    {
        int particleCount = particleSystem.particleCount;
        particles = new ParticleSystem.Particle[particleCount];

        particleSystem.GetParticles(particles);

        for (int i = 0; i < particleCount; i++)
        {
            Vector3 velocity = particles[i].velocity;
            Vector3 position = particles[i].position;
            Vector2 particleV2 = new Vector2(position.x, position.y);
            Vector2 move = Vector2.MoveTowards(center, particleV2, speed);
            if (Vector3.Distance(particleV2, center) > diameter)
            {
                move = -move;
            }
            if (true) //Vector2.SqrMagnitude(move) > circleTolerance / 100
            {
                particles[i].position = new Vector3(position.x + move.x, position.y + move.y, position.z);
            }
            
            

        }        

        particleSystem.SetParticles(particles, particleCount);
    }

    void ShakeParticles()
    {
        int particleCount = particleSystem.particleCount;

        particles = new ParticleSystem.Particle[particleCount];
       

        particleSystem.GetParticles(particles);

        for (int i = 0;  i < particleCount; i++)
        {
            particles[i].position += new Vector3(Random.Range(shakeRange.x, shakeRange.y), Random.Range(shakeRange.x, shakeRange.y), 0);
        }

        particleSystem.SetParticles(particles, particleCount);
    }

    IEnumerator ShakeParticlesTimer(float length)
    {

        shakeParticlesOn = true;
        yield return new WaitForSeconds(length);
        shakeParticlesOn = false;
    }
}