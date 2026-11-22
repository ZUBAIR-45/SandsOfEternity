using UnityEngine;

// Saari awaazein: wind + drums (music) aur sound effects.
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public AudioClip wind, drumsSlow, drumsFast;
    public AudioClip gem, hit, door, heart, victory, gameOver, slam;

    AudioSource windSrc, drumSrc, sfxSrc;

    void Awake()
    {
        Instance = this;
        windSrc = MakeSource(true, 0.35f);
        drumSrc = MakeSource(true, 0.5f);
        sfxSrc = MakeSource(false, 1f);
    }

    AudioSource MakeSource(bool loop, float volume)
    {
        AudioSource s = gameObject.AddComponent<AudioSource>();
        s.loop = loop;
        s.volume = volume;
        s.playOnAwake = false;
        return s;
    }

    public void StartMusic()
    {
        windSrc.clip = wind; windSrc.Play();
        drumSrc.clip = drumsSlow; drumSrc.Play();
    }

    public void FastDrums()
    {
        drumSrc.clip = drumsFast; drumSrc.Play();
    }

    public void StopMusic()
    {
        windSrc.Stop();
        drumSrc.Stop();
    }

    void Play(AudioClip c) { if (c != null) sfxSrc.PlayOneShot(c); }

    public void PlayGem() { Play(gem); }
    public void PlayHit() { Play(hit); }
    public void PlayDoor() { Play(door); }
    public void PlayHeart() { Play(heart); }
    public void PlayVictory() { Play(victory); }
    public void PlayGameOver() { Play(gameOver); }
    public void PlaySlam() { Play(slam); }
}
