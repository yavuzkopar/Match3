using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    PrefabInstancePool<Tile> pool;
    [SerializeField,Range(0,1)]
    float disepearDuration = 0.25f;
    float disappearProgress;
    ParticleSystem particle;
    [System.Serializable]
    struct FallingState
    {
        public float fromY, toY, duration, progress;
    }
    FallingState fallingState;
    public Tile Spawn(Vector3 position)
    {
        Tile instance = pool.GetInstance(this);
        instance.pool = pool;
        instance.transform.localPosition= position;
        instance.transform.localScale = Vector3.one;
        instance.disappearProgress = -1;
        instance.fallingState.progress = -1;
        instance.enabled= false;
        Debug.Log("Spawn");
        return instance;
    }
    public float Fall(float toY, float speed)
    {
        fallingState.fromY = transform.localPosition.y;
        fallingState.toY = toY;
        
        fallingState.duration = (fallingState.fromY - toY) / speed;
        fallingState.progress = 0;
        enabled = true;
        return fallingState.duration;
    }
    public float Disappear()
    {
        disappearProgress = 0;
        enabled = true;
        return disepearDuration;
    }
    private void Awake()
    {
        particle = transform.GetChild(2).GetComponent<ParticleSystem>();
    }
    bool isPlayed;
    private void Update()
    {
        if (disappearProgress >= 0)
        {
            disappearProgress += Time.deltaTime;
            if(!isPlayed)
            {
                particle.Play();
                isPlayed = true;
            }
            
            if (disappearProgress>=disepearDuration)
            {
                DeSpawn();
                return;
            }
            transform.localScale = Vector3.one * (1 - disappearProgress / disepearDuration);
        }
        if (fallingState.progress>=0)
        {
            fallingState.progress += Time.deltaTime;
            Vector3 position = transform.position;
            if (fallingState.progress >= fallingState.duration)
            {
                fallingState.progress = -1;
                position.y = fallingState.toY;
                enabled = disappearProgress>= 0;
            }
            else
            {
                position.y = Mathf.Lerp(fallingState.fromY, fallingState.toY, fallingState.progress / fallingState.duration);
            }
            transform.localPosition = position;
        }
    }
    public void DeSpawn()
    {
        
        pool.Recycle(this);
    }
}
