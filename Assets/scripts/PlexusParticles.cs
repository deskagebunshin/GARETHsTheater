﻿//using System.Collections;

using UnityEngine;
using System.Collections.Generic;
[RequireComponent(typeof(ParticleSystem))]
public class PlexusParticles : MonoBehaviour {

    private Vector3[] newVertices;
    private Vector2[] newUV;
    private int[] newTriangles;
    public int maxTriangles = 100;
    public bool triangles = true;
    public float maxDistance = 1.0f;
    public LineRenderer lineRendererTemplate;
    List<LineRenderer> lineRenderers= new List<LineRenderer>();
    new ParticleSystem particleSystem;
    ParticleSystem.Particle[] particles;
    ParticleSystem.MainModule particleSystemMainModule;
    Transform _Transform;
    public int maxParticleConections;
    public int maxConectionsTotal;
    private MeshFilter meshFilter;
    private MeshRenderer meshRender;
    // Use this for initialization
    void Start () {
        meshRender = GetComponent<MeshRenderer>();
        particleSystem = GetComponent<ParticleSystem>();
        particleSystemMainModule = particleSystem.main;
        meshFilter = GetComponent<MeshFilter>();

    }
	
	// Update is called once per frame
	void LateUpdate () {
        int maxParticles = particleSystemMainModule.maxParticles;
        int maxVertices = maxTriangles * 3;
        int vertCount = 0;
        if (particles == null || particles.Length < maxParticles)
        {
            particles = new ParticleSystem.Particle[maxParticles];
        }
        if (triangles)
        {
            if (newVertices == null || newVertices.Length < maxVertices)
            {
                newVertices = new Vector3[maxVertices];
                newUV = new Vector2[maxVertices];
                newTriangles = new int[maxVertices];
            }
        } else
        {

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
        if (triangles)
        {
            meshRender.enabled = true;   
        } else
        {
            meshRender.enabled = false;
        }
        
        for (int i = 0; i < particleCount; i++)
        {
            if (lineRendererIndex >= maxConectionsTotal)
            {
                break;
            }


            int counter = 0;
            Vector3 p1_position = particles[i].position;
            if (triangles && vertCount < maxVertices)
            {
                vertCount = vertCount - vertCount % 3;
                newVertices[vertCount] = p1_position;
                vertCount++;
            } 

            int thisVertCount = 0;
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
                    if (triangles && thisVertCount < 2 && vertCount < maxVertices)
                    {
                        newVertices[vertCount] = p2_position;
                        thisVertCount++;
                        vertCount++;
                    }
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
        if (triangles)
        {
            var nullVector = new Vector3(-100, -100, -100);
            for (int i = vertCount + 1; i < newVertices.Length; i++)
            {
                newUV[i] = nullVector;
            }
            for (int i = 0; i < newUV.Length; i++)
            {
                newUV[i] = new Vector2(newVertices[i].x, newVertices[i].z);
                newTriangles[i] = i;
            }
            Mesh mesh = new Mesh();
            meshFilter.mesh = mesh;
            mesh.vertices = newVertices;
            mesh.uv = newUV;
            mesh.triangles = newTriangles;
        }
        

        for (int i = lineRendererIndex; i < lineRendererCount; i++)
        {
            lineRenderers[i].enabled = false;
        }

    }
}
