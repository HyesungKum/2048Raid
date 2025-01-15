
using UnityEngine;
using UnityEngine.Audio;

public class SoundMgr : MonoBehaviourSingleton<SoundMgr>
{
    //==========================================================================================
    //
    // 
    //
    [Header("Reference Data")]
    [SerializeField] private AudioClip[] bgms;
    [SerializeField] private AudioClip[] sfxs;
    [SerializeField] private AudioClip ckickSfx;

    //========================================audio mixer=======================================
    [Header("Master Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    //=========================================audio source=====================================
    [Header("Audio Out")]
    [SerializeField] private AudioSource BGMAudio;
    
    [SerializeField] private AudioSource[] SFXAudio = new AudioSource[3];

	[SerializeField] private AudioSource UIAudio;

	//======================================inner variables=====================================
	private int audioCursor = 0;
    private int? lockCursor = null;

    //========================BGM Controll================================
    /// <summary>
    /// setting bgm using audio clip index and play loop
    /// </summary>
    /// <param name="idx">target audio clip index</param>
    public void SetBGM(int idx)
    {
        if (bgms.Length < idx || bgms == null || idx < 0)
        {
#if UNITY_EDITOR
            Debug.LogError($"## SoundMgr Error : wrong bgm idx required idx<{idx}> ");
#endif
            return;
        }

        //apply audio clip
        BGMAudio.clip = bgms[idx];

        //set and play
        BGMAudio.pitch = 1f;
        BGMAudio.loop = true;
        BGMAudio.Play();
    }
    public void SetBGMLoop()
    {
        if(BGMAudio != null) BGMAudio.loop = true;
    }
    public void SetBGMUnLoop()
    {
        if (BGMAudio != null) BGMAudio.loop = false;
    }
    public void SetBGMSpeed(float speed)
    {
        if (BGMAudio != null) BGMAudio.pitch = speed;
    }
    public void PlayBGM()
    {
        if (BGMAudio != null) BGMAudio.Play();
    }
    public void StopBGM()
    {
        if (BGMAudio != null) BGMAudio.Stop();
    }

    //=======================SFX Controll=================================
    /// <summary>
    /// Call predefined Sfx chaneling
    /// </summary>
    /// <param name="idx">predefined Sfx index</param>
    public void CallSfx(int idx)
    {
        if (sfxs.Length < idx || sfxs == null || idx < 0)
        {
#if UNITY_EDITOR
            Debug.LogError($"## SoundMgr Error : wrong sfx idx required idx<{idx}> ");
#endif
            return;
        }

        //dodge lock cursor
        if (audioCursor == lockCursor) audioCursor = (audioCursor + 1) % SFXAudio.Length;

        SFXAudio[audioCursor].clip = sfxs[idx]; 
        SFXAudio[audioCursor].Play();

        audioCursor = (audioCursor + 1) % SFXAudio.Length;
    }
    /// <summary>
    /// call sfx using clip
    /// </summary>
    /// <param name="clip">target play sfx clip</param>
    public void CallSfx(AudioClip clip)
    {
        if (clip == null) return;

        SFXAudio[audioCursor].clip = clip;
        SFXAudio[audioCursor].Play();

        audioCursor = (audioCursor + 1) % SFXAudio.Length;
    }
    public void LoopSfx(int idx)
    {
        if (sfxs.Length < idx || sfxs == null || idx < 0)
        {
#if UNITY_EDITOR
            Debug.LogError($"## SoundMgr Error : wrong sfx idx required idx<{idx}> ");
#endif
            return;
        }

        SFXAudio[audioCursor].clip = sfxs[idx];
        SFXAudio[audioCursor].loop = true;
        SFXAudio[audioCursor].Play();

        lockCursor = audioCursor;

        audioCursor = (audioCursor + 1) % SFXAudio.Length;
    }
    public void StopLoopSfx()
    {
        if (lockCursor == null) return;

        SFXAudio[(int)lockCursor].loop = false;
        SFXAudio[(int)lockCursor].Stop();

        lockCursor = null;
    }
	//=======================UI Controll==============================
	public void CallUI()
	{
		UIAudio.clip = ckickSfx;
		UIAudio.Play();
	}

	public void CallUI(AudioClip clip)
	{
		if (clip == null) return;
		UIAudio.clip = clip;
		UIAudio.Play();
	}

	//=======================Volume Controll==============================
	public void VolumeControll(string target ,float volume)
    {
        if (volume == -40) audioMixer.SetFloat(target, -80);
        else audioMixer.SetFloat(target, volume);
    }
    public float GetVolume(string target)
    {
        audioMixer.GetFloat(target, out float value);
        if (value == -80) return -40;
        else return value;
    }
    /// <summary>
    /// toggle sound (on - off)
    /// </summary>
    /// <param name="target">target audio mixer parameter</param>
    /// <param name="volume">setting value -40 ~ 0 </param>
    public void ToggleControll(string target, float value)
    {
        audioMixer.GetFloat(target, out float volume);

        volume = volume == -80? value : -80;

        audioMixer.SetFloat(target, volume);   
    }
}