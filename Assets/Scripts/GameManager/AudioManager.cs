using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource audioSource;

    public AudioClip game_1;
    public AudioClip game_2;
    public AudioClip revenge;
    public AudioClip upgrade;
    public AudioClip dead;
    public AudioClip ending;

    private GameManager.PlayerState currentPlayerState;

    private int gameCounts = 0;

    private void OnEnable()
    {
        GameManager.OnStateChanged += HandleGameStateChanged;
        GameManager.PlayerStateChanged += HandlePlayerStateChanged;
        Distance.OnWin += HandleWin;
    }

    private void OnDisable()
    {
        GameManager.OnStateChanged -= HandleGameStateChanged;
        GameManager.PlayerStateChanged -= HandlePlayerStateChanged;
        Distance.OnWin -= HandleWin;
    }

    private void HandleGameStateChanged(GameManager.GameState state)
    {
        if (currentPlayerState == GameManager.PlayerState.Revenge) return;

        switch(state)
        {
            case GameManager.GameState.Play:
                if(gameCounts % 2 == 0){
                    PlayTrack(game_1);
                } else {
                    PlayTrack(game_2);
                }
                gameCounts++;
                break;
            case GameManager.GameState.Shop:
                PlayTrack(upgrade);
                break;
            case GameManager.GameState.Dead:
                PlayTrack(dead);
                break;
            default:
                break;
        }
    }

    private void HandlePlayerStateChanged(GameManager.PlayerState playerState)
    {
        currentPlayerState = playerState;

        switch(playerState)
        {
            case GameManager.PlayerState.Normal:
                break;
            case GameManager.PlayerState.Revenge:
                PlayTrack(revenge);
                break;
            default:
                break;
        }
    }

    private void HandleWin()
    {  
        audioSource.Stop();
    }

    private bool IsAlreadyPlaying(AudioClip audio)
    {
        return (audioSource.isPlaying && audioSource.clip == audio) ? true : false;
    }

    private void PlayTrack(AudioClip audio)
    {
        if (IsAlreadyPlaying(audio)) return;

        audioSource.Stop();
        audioSource.clip = audio;
        audioSource.loop = true;
        audioSource.Play();
    }
}