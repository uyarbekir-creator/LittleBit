using UnityEngine;
using DG.Tweening;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance { get; private set; }

    [Header("Loop Settings")]
    [SerializeField] private bool _useSeamlessLoop = false;
    [SerializeField] private float _startTimeOffset = 1.5f;
    [SerializeField] private float _loopEndPoint = 2.0f;
    [SerializeField] private float _fadeDuration = 1.0f;

    private AudioSource _audioSource1;
    private AudioSource _audioSource2;
    private bool _isUsingSource1 = true;
    
    private float _defaultPitch = 1f;
    private float _defaultVolume = 1f;
    private bool _isCustomLooping = false;
    private AudioClip _currentClip;

    private void Awake() 
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        } 
        
        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        AudioSource[] sources = GetComponents<AudioSource>();
        _audioSource1 = sources.Length > 0 ? sources[0] : gameObject.AddComponent<AudioSource>();
        _audioSource2 = sources.Length > 1 ? sources[1] : gameObject.AddComponent<AudioSource>();

        _audioSource2.playOnAwake = false;
        _audioSource2.spatialBlend = _audioSource1.spatialBlend;
        _audioSource2.priority = _audioSource1.priority;

        // Baslangic ses seviyesini kaydet
        _defaultVolume = _audioSource1.volume;
    }

    private void Update()
    {
        if (!_useSeamlessLoop || !_isCustomLooping || _currentClip == null) return;

        AudioSource activeSource = _isUsingSource1 ? _audioSource1 : _audioSource2;

        if (activeSource.isPlaying && activeSource.time >= _currentClip.length - _loopEndPoint)
        {
            CrossfadeToNext();
        }
    }

    public void ChangeMusic(AudioClip newMusic, bool loop = true)
    {
        // Muzik zaten caliyorsa ve durmussa tekrar oynat
        if (_currentClip == newMusic)
        {
            AudioSource activeSource = _isUsingSource1 ? _audioSource1 : _audioSource2;
            if (!activeSource.isPlaying) activeSource.Play();
            return;
        }

        _currentClip = newMusic;
        
        // DOTween animasyonlarini temizle ve sesleri durdur
        _audioSource1.DOKill();
        _audioSource2.DOKill();
        _audioSource1.Stop();
        _audioSource2.Stop();

        _audioSource1.clip = newMusic;
        _audioSource2.clip = newMusic;

        _audioSource1.pitch = _defaultPitch;
        _audioSource2.pitch = _defaultPitch;
        
        _isUsingSource1 = true;

        if (_useSeamlessLoop)
        {
            _audioSource1.loop = false;
            _audioSource2.loop = false;
            _audioSource1.time = _startTimeOffset;
            _audioSource1.volume = _defaultVolume; // Sesi garantiye al
            _audioSource2.volume = 0f;
            _isCustomLooping = true;
        }
        else
        {
            _audioSource1.loop = loop;
            _audioSource1.time = 0f;
            _audioSource1.volume = _defaultVolume;
            _isCustomLooping = false;
        }

        _audioSource1.Play();
    }

    private void CrossfadeToNext()
    {
        _isCustomLooping = false; 

        AudioSource fadingOutSource = _isUsingSource1 ? _audioSource1 : _audioSource2;
        AudioSource fadingInSource = _isUsingSource1 ? _audioSource2 : _audioSource1;

        fadingInSource.time = _startTimeOffset;
        fadingInSource.volume = 0f;
        fadingInSource.Play();

        // Safe Mode hatalarini onlemek icin SetLink ekliyoruz
        fadingOutSource.DOFade(0f, _fadeDuration).SetLink(gameObject).OnComplete(() => {
            fadingOutSource.Stop();
        });

        fadingInSource.DOFade(_defaultVolume, _fadeDuration).SetLink(gameObject).OnComplete(() => {
            _isUsingSource1 = !_isUsingSource1;
            _isCustomLooping = true;
        });
    }

    public void SetPitch(float targetPitch, float duration = 0.5f)
    {
        _audioSource1.DOKill();
        _audioSource2.DOKill();
        _audioSource1.DOPitch(targetPitch, duration).SetLink(gameObject);
        _audioSource2.DOPitch(targetPitch, duration).SetLink(gameObject);
    }

    public void SetVolume(float targetVolume, float duration = 0.5f)
    {
        _defaultVolume = targetVolume;
        AudioSource activeSource = _isUsingSource1 ? _audioSource1 : _audioSource2;
        activeSource.DOKill();
        activeSource.DOFade(targetVolume, duration).SetLink(gameObject);
    }

    public void SetMusicMute(bool isMuted)
    {
        _audioSource1.mute = isMuted;
        _audioSource2.mute = isMuted;
    }

    public bool ToggleMusic()
    {
        _audioSource1.mute = !_audioSource1.mute;
        _audioSource2.mute = _audioSource1.mute;
        return !_audioSource1.mute;
    }

    public void PlayBackgroundMusic(bool isMusicPlaying)
    {
        AudioSource activeSource = _isUsingSource1 ? _audioSource1 : _audioSource2;
        if (isMusicPlaying && !activeSource.isPlaying) activeSource.Play();
        else if (!isMusicPlaying) activeSource.Stop();
    }
}