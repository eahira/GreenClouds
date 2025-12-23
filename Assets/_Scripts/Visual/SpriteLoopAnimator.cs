using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteLoopAnimator : MonoBehaviour
{
    [Header("Frames")]
    public Sprite[] frames;

    [Header("Speed")]
    [Min(0.01f)] public float fps = 8f;

    [Header("Options")]
    public bool playOnEnable = true;
    public bool randomStartFrame = true;

    private SpriteRenderer sr;
    private float timer;
    private int index;
    private bool playing;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        if (randomStartFrame && frames != null && frames.Length > 0)
            index = Random.Range(0, frames.Length);
        else
            index = 0;

        timer = 0f;

        if (playOnEnable)
            Play();
        else
            ApplyFrame();
    }

    private void Update()
    {
        if (!playing) return;
        if (frames == null || frames.Length == 0) return;

        timer += Time.deltaTime;
        float frameTime = 1f / fps;

        while (timer >= frameTime)
        {
            timer -= frameTime;
            index = (index + 1) % frames.Length;
            ApplyFrame();
        }
    }

    private void ApplyFrame()
    {
        if (sr != null && frames != null && frames.Length > 0)
            sr.sprite = frames[index];
    }

    public void Play()
    {
        playing = true;
        ApplyFrame();
    }

    public void Stop()
    {
        playing = false;
    }

    public void SetFrames(Sprite[] newFrames, float newFps)
    {
        frames = newFrames;
        fps = Mathf.Max(0.01f, newFps);

        index = 0;
        timer = 0f;
        ApplyFrame();
    }
}
