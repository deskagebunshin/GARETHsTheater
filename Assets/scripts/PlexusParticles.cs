//using System.Collections;

using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(ParticleSystem))]
public class PlexusParticles : MonoBehaviour {

    public float maxDistance = 1.0f;
    public LineRenderer lineRendererTemplate;
    List<LineRenderer> lineRenderers= new List<LineRenderer>();
    new ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;
    ParticleSystem.MainModule particleSystemMainModule;
    Transform _Transform;
    public int maxParticleConections;
    public int maxConectionsTotal;
    // Use this for initialization
    void Start () {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;
	}
	
	// Update is called once per frame
	void LateUpdate () {
        int maxParticles = particleSystemMainModule.maxParticles;

        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }

        particleSystem.GetParticles(particles);
        int particleCount = particleSystem.particleCount;
        int lineRendererIndex = 0;
        int lineRendererCount = lineRenderers.Count;
        float maxDistanceSqr = maxDistance * maxDistance;

        switch (particleSystemMainModule.simulationSpace)
        {
            case ParticleSystemSimulationSpace.Local:
                {
                    _Transform = transform;
                    lineRendererTemplate.useWorldSpace = false;
                    break;
                }
            case ParticleSystemSimulationSpace.Custom:
                {
                    _Transform = particleSystemMainModule.customSimulationSpace;
                    lineRendererTemplate.useWorldSpace = false;
                    break;
                }
            case ParticleSystemSimulationSpace.World:
                {
                    _Transform = transform;
                    lineRendererTemplate.useWorldSpace = true;
                    break;

                }
            default:
                {
                    Debug.Log("Simulation Space Not Recognized!");
                    break;
                                 
                }
        }
        
        for (int i = 0; i < particleCount; i++)
        {
            if (lineRendererIndex >= maxConectionsTotal)
            {
                break;
            }
            int counter = 0;
            Vector3 p1_position = particles[i].position;
            for (int j = i +1; j < particleCount; j++)
            {
                if (!particles[i].startColor.Equals(particles[j].startColor))
                {
                    break;
                }
                Vector3 p2_position = particles[j].position;
                float distanceSqr = Vector3.SqrMagnitude(p2_position - p1_position);

                if (distanceSqr < maxDistanceSqr)
                {
                    LineRenderer lr;
                    if (lineRendererIndex == lineRendererCount)
                    {
                        lr = Instantiate(lineRendererTemplate, _Transform, false);
                        lineRenderers.Add(lr);
                        lineRendererCount++;
                    }
                    lr = lineRenderers[lineRendererIndex];
                    lr.enabled = true;
                    lr.SetPosition(0, p1_position);
                    lr.SetPosition(1, p2_position);
                    lineRendererIndex++;
                    counter++;

                    if (counter >= maxParticleConections)
                    {
                        break;
                    }
                }
                
            }

        }

        for (int i = lineRendererIndex; i < lineRendererCount; i++)
        {
            lineRenderers[i].enabled = false;
        }

    }
}
