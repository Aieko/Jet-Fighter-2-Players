using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private int _whiteScore;
    private int _blackScore;

    private Canvas canvas;
    private AudioSource audioSource;

    public float canvasWidth { get; private set; }
    public float canvasHeight { get; private set; }

    [SerializeField] private GameObject white;
    [SerializeField] private GameObject black;
    [SerializeField] private AudioClip hit;


    private void Awake()
    {
        MakeInstance();
        canvas = GameObject.FindObjectOfType<Canvas>();
        var rectTransform = canvas.gameObject.GetComponentInChildren<Image>().GetComponent<RectTransform>();
        canvasWidth = rectTransform.rect.width + 10f;
        canvasHeight = rectTransform.rect.height + 7f;

        audioSource = GetComponent<AudioSource>();
    }

    private void MakeInstance()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != null)
        {
            Destroy(gameObject);
        }
    }

    public void SetScore(int player)
    {
        if (player == 0)
        {
            _blackScore++;
            black.GetComponent<Text>().text = _blackScore.ToString();
            black.GetComponent<AudioSource>().Play();
        }
        else
        {
            _whiteScore++;
            white.GetComponent<Text>().text = _whiteScore.ToString();
            white.GetComponent<AudioSource>().Play();
        }
    }

    public void ConstrainToMap(GameObject obj)
    {

        if (obj.transform.position.x < -canvasWidth)
        {
            obj.transform.position = new Vector2(canvasWidth, obj.transform.position.y);
        }
        else if (obj.transform.position.x > canvasWidth)
        {
            obj.transform.position = new Vector2(-canvasWidth, obj.transform.position.y);
        }

        if (obj.transform.position.y > canvasHeight)
        {
            obj.transform.position = new Vector2(obj.transform.position.x, -canvasHeight);
        }
        else if (obj.transform.position.y < -canvasHeight)
        {
            obj.transform.position = new Vector2(obj.transform.position.x, canvasHeight);
        }
    }

    public void PlayFireSound(AudioClip sound)
    {
        audioSource.clip = sound;
        audioSource.Play();
    }

    public void PlayHitSound()
    {
        audioSource.clip = hit;
        audioSource.Play();
    }

}
