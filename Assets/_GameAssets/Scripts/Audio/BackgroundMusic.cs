using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic Instance { get; private set; }

    [Header("Loop Settings")]
    [SerializeField] private bool _useSeamlessLoop = true; // Daha stabil çalışma için varsayılan true
    [SerializeField] private float _startTimeOffset = 0f;
    [SerializeField] private float _loopEndPoint = 1.5f; 
    [SerializeField] private float _fadeDuration = 1.2f;

    private AudioSource _audioSource1;
    private AudioSource _audioSource2;
    private bool _isUsingSource1 = true;
    
    private float _defaultPitch = 1f;
    private float _initialMasterVolume; 
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
        
        // İlk açılıştaki ses seviyesini kalıcı olarak kaydet
        _initialMasterVolume = _audioSource1.volume > 0 ? _audioSource1.volume : 0.5f;
        _defaultVolume = _initialMasterVolume;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Sahne değişince tüm yarım kalan DOTween'leri temizle
        _audioSource1.DOKill();
        _audioSource2.DOKill();
        
        _defaultVolume = _initialMasterVolume;
        
        AudioSource activeSource = _isUsingSource1 ? _audioSource1 : _audioSource2;
        activeSource.volume = _defaultVolume;
        activeSource.pitch = _defaultPitch;

        AudioSource inactiveSource = _isUsingSource1 ? _audioSource2 : _audioSource1;
        inactiveSource.volume = 0f;

        if (_currentClip != null && !activeSource.isPlaying)
        {
            activeSource.Play();
        }
    }

    private void Update()
    {
        // Geçiş (fade) devam ediyorsa veya klibi yoksa bekle
        if (!_useSeamlessLoop || !_isCustomLooping || _currentClip == null) return;

        AudioSource activeSource = _isUsingSource1 ? _audioSource1 : _audioSource2;

        // Loop noktasına gelindiğinde Crossfade başlat
        if (activeSource.isPlaying && activeSource.time >= _currentClip.length - _loopEndPoint)
        {
            CrossfadeToNext();
        }
    }

    public void ChangeMusic(AudioClip newMusic, bool loop = true)
    {
        if (_currentClip == newMusic) return;

        _currentClip = newMusic;
        
        _audioSource1.DOKill();
        _audioSource2.DOKill();

        _audioSource1.clip = newMusic;
        _audioSource2.clip = newMusic;

        _isUsingSource1 = true;
        _audioSource1.volume = _defaultVolume;
        _audioSource2.volume = 0f;
        
        _audioSource1.pitch = _defaultPitch;
        _audioSource2.pitch = _defaultPitch;

        if (_useSeamlessLoop)
        {
            _audioSource1.loop = false;
            _audioSource2.loop = false;
            _isCustomLooping = true;
        }
        else
        {
            _audioSource1.loop = loop;
            _audioSource2.loop = loop;
            _isCustomLooping = false;
        }

        _audioSource1.Play();
        _audioSource2.Stop();
    }

    private void CrossfadeToNext()
    {
        // KİLİT: Geçiş bitene kadar Update'in tekrar burayı tetiklemesini engelle
        _isCustomLooping = false; 

        AudioSource fadingOutSource = _isUsingSource1 ? _audioSource1 : _audioSource2;
        AudioSource fadingInSource = _isUsingSource1 ? _audioSource2 : _audioSource1;

        fadingInSource.time = _startTimeOffset;
        fadingInSource.Play();
        fadingInSource.volume = 0f;

        fadingOutSource.DOFade(0f, _fadeDuration).SetEase(Ease.Linear).SetLink(gameObject);
        fadingInSource.DOFade(_defaultVolume, _fadeDuration).SetEase(Ease.Linear).SetLink(gameObject).OnComplete(() => {
            fadingOutSource.Stop();
            _isUsingSource1 = !_isUsingSource1; 
            _isCustomLooping = true; // Geçiş bitti, artık bir sonraki loop beklenebilir
        });
    }

    public void SetPitch(float targetPitch, float duration = 0.5f)
    {
        _audioSource1.DOPitch(targetPitch, duration).SetLink(gameObject);
        _audioSource2.DOPitch(targetPitch, duration).SetLink(gameObject);
    }

    public void SetVolume(float targetVolume, float duration = 0.5f)
    {
        AudioSource activeSource = _isUsingSource1 ? _audioSource1 : _audioSource2;
        activeSource.DOKill();
        activeSource.DOFade(targetVolume, duration).SetLink(gameObject);
    }

    // SettingsUI.cs içindeki hatayı çözen fonksiyon:
    public void SetMusicMute(bool isMuted)
    {
        if (_audioSource1 != null) _audioSource1.mute = isMuted;
        if (_audioSource2 != null) _audioSource2.mute = isMuted;
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

    private void OnDestroy()
{
    // Obje yok edildiğinde bu obje üzerindeki tüm DOTween animasyonlarını durdur
    DOTween.Kill(gameObject);
}
}