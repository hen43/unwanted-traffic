using UnityEngine;
using TMPro;

public class Distance : MonoBehaviour
{
    [SerializeField] private CurrencyHandler currencyHandler;

    public TMP_Text distanceText;

    private Player player;
    private Horde horde;
    private float startingX;
    private bool initializedBaseline = false;

    public float dist { get; private set; }
    public float dist_horde { get; private set; }

    private Navigator navigator;

    [SerializeField] RectTransform coneIcon;
    [SerializeField] RectTransform hordeIcon;

    private float progressBarLength = 400f - (-435f);
    private float progressBarTotalDist = 2000f;

    public static event System.Action OnWin;

    void Start()
    {
        TryFindObjects();
        
        if(navigator == null)
        {
            navigator = FindAnyObjectByType<Navigator>();
        }
    }

    void OnEnable()
    {
        GameManager.OnStateChanged += hide;
    }

    void OnDisable()
    {
        GameManager.OnStateChanged -= hide;
    }

    void hide(GameManager.GameState gameState)
    {
        if(gameState == GameManager.GameState.Shop)
        {
            distanceText.enabled = false;
        } else {
            distanceText.enabled = true;
        }
    }

    void Update()
    {
        if (player == null)
        {
            TryFindObjects();
            return;
        }

        if (distanceText != null)
        {
            dist = Mathf.FloorToInt(player.transform.position.x - startingX);
            dist_horde = Mathf.FloorToInt(horde.transform.position.x - startingX);
            // distanceText.text = $"Distance: {dist}m";
            // abandoned after progress bar was added
        }

        ProgressBar();
        WinCheck();
    }

    private void ProgressBar(){
        // start x = -435
        // end x = 400
        float x = -435 + (dist / progressBarTotalDist) * progressBarLength;
        float horde_x = -435 + (dist_horde / progressBarTotalDist) * progressBarLength;

        coneIcon.anchoredPosition = new Vector2(x,  coneIcon.anchoredPosition.y);
        hordeIcon.anchoredPosition = new Vector2(horde_x,  coneIcon.anchoredPosition.y);
    }

    private void TryFindObjects()
    {
        player = FindAnyObjectByType<Player>();
        if (player != null && !initializedBaseline)
        {
            startingX = player.transform.position.x;
        }

        horde = FindAnyObjectByType<Horde>();
        if (horde != null && !initializedBaseline)
        {
            startingX = horde.transform.position.x;
        }

        if(player != null && horde != null)
        {
            initializedBaseline = true;
        }
    }

    private void WinCheck()
    {
        if(dist > 2000f)
        {
            OnWin?.Invoke();
            navigator.ChangeScene("Ending");
        }
    }
}