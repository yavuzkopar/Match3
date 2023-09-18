using UnityEngine;

[System.Serializable]
public class TileSwapper
{
    [SerializeField, Range(0.1f, 10f)]
    float duration = 0.25f;

    [SerializeField, Range(0f, 1f)]
    float maxDepthOffset = 0.5f;

    Tile tileA, tileB;
    Vector3 positionA, positionB;
    float progress;
    bool pingPong;

    
    public float Swap(Tile a,Tile b, bool pingPong)
    {
        tileA= a;
        tileB= b;
        positionA = tileA.transform.localPosition;
        positionB = tileB.transform.localPosition;
        this.pingPong = pingPong;
        progress = 0;
        
        return pingPong ? 2f * duration: duration;
    }
    
    public void Update()
    {
        if (progress < 0) return;
        progress += Time.deltaTime;
        if (progress>= duration)
        {
            if (pingPong)
            {
                progress -= duration;
                pingPong= false;
                (tileA,tileB) = (tileB,tileA);
            }
            else
            {
                progress = -1;
                tileA.transform.localPosition = positionB;
                tileB.transform.localPosition = positionA;
                return;
            }
        }
        float t = progress / duration;
        float z = Mathf.Sin(Mathf.PI * t) * maxDepthOffset;
        Vector3 p = Vector3.Lerp(positionA, positionB, t);
        p.z = -z;
        tileA.transform.localPosition = p;
        p = Vector3.Lerp(positionA, positionB, 1f - t);
        p.z = z;
        tileB.transform.localPosition = p;
    }
}