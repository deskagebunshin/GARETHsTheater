using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControll : MonoBehaviour {
    private ParticleSystem particleSystem;
    private ParticleSystem.Particle[] particles;
    private ParticleSystem.MainModule particleSystemMainModule;
    public Vector2 shakeRange;
    public float gravity;
    public Vector3 allowedDistance;
    private bool shakeParticlesOn;
    private int particleCountGlobal = 0;
    public float particleSpeedXY;
    private int[] musicNumbers = new int[] {1,2,3,4,5,6,7,8,7,6,5,4,3,2,3};
    private int totalNotes = 66;
    public Color[] musicBar;
    // Use this for initialization
    void Start () {
        
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;
        StartCoroutine(Gravity());
    }
	

	// Update is called once per frame
	void Update () {

        if (Input.GetButtonUp("Jump"))
        {
            
            var emitParams = new ParticleSystem.EmitParams();
            emitParams.startColor = GetColor();
            emitParams.velocity = new Vector3(Random.Range(-particleSpeedXY, particleSpeedXY), Random.Range(-particleSpeedXY, particleSpeedXY), 0.5f);
            particleSystem.Emit(emitParams, 1);
            particleCountGlobal++;
        }

        if (Input.GetButton("Fire1"))
        {
            shakeParticlesOn = true;        
        } else
        {
            shakeParticlesOn = false;
        }
        
        if (Input.GetButtonDown("Fire2"))
        {
            MultiplyParticles(1);
        }

        if (shakeParticlesOn)
        {
            ShakeParticles();
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

        Debug.Log("count " + count);
        Debug.Log("particleGlobal " + particleCountGlobal);
        Debug.Log("color " + color );
        return musicBar[color];
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
            if (particles[i].position.z > allowedDistance.z)
            {
                particles[i].velocity = new Vector3(particles[i].velocity.x, particles[i].velocity.y, 0);
                //particles[i].position = new Vector3(position.x, position.y, allowedDistance.z);
            }
            /*
            if (Mathf.Abs(position.x) > allowedDistance.x)
            {
                Debug.Log(position.x + " " +allowedDistance.x + " " + Mathf.Abs(position.x));
                particles[i].velocity = new Vector3(-particles[i].velocity.x, particles[i].velocity.y, particles[i].velocity.z);
            }
            if (Mathf.Abs(position.y) > allowedDistance.y)
            {
                particles[i].velocity = new Vector3(particles[i].velocity.x, -particles[i].velocity.y, particles[i].velocity.z);
            }*/
        }

        

        particleSystem.SetParticles(particles, particleCount);
    }

    IEnumerator Gravity()
    {
        yield return new WaitForSeconds(1);
        GravityAttractor();
        StartCoroutine(Gravity());
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

