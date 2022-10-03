using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class Flymo : MonoBehaviour
{
    private static float NORMAL_SPEED = 0.5f;
    private static float TURBO_SPEED = 0.125f;

    public Tilemapper tilemapper;
    private Tilemap _tm;
    private Vector3 _nextForward = new Vector3(1, 0, 0);
    private Quaternion _nextRotation = Quaternion.identity;
    private Vector3 forward = new Vector3(1, 0, 0);
    private float _speed;
    private float _countdown;
    private bool _turboMode;
    public Transform probe;
    public TMPro.TextMeshProUGUI _debug;
    public TMPro.TextMeshProUGUI _turboCountdown;
    public GameObject levelComplete;

    public bool IsRunning { get; set; }

    void Awake()
    {
        _tm = tilemapper.GetComponent<Tilemap>();
    }

    IEnumerator Start()
    {
        // Wait for the tile mapper
        while (!tilemapper.IsReady)
            yield return null;

        gameObject.transform.position = tilemapper.StartPosition;
        IsRunning = true;
        _speed = NORMAL_SPEED;
        _countdown = 10f;
        tilemapper.MowLawn(transform.position);

        StartCoroutine(TurboCountdown());

        while (IsRunning)
        {
            float time = 0f;
            var startPos = transform.position;
            var endPos = transform.position + forward;

            if (tilemapper.IsBlocker(endPos) || IsAtScreenBounds(endPos))
            {
                yield return null;
            }
            else
            {
                while (time < 1f)
                {
                    transform.position = Vector3.Lerp(startPos, endPos, time);
                    time += Time.deltaTime / _speed;
                    yield return null;
                }

                transform.position = endPos;
                tilemapper.MowLawn(transform.position);
            }

            forward = _nextForward;
            transform.rotation = _nextRotation;

            if (tilemapper.LevelComplete)
            {
                levelComplete.SetActive(true);
                IsRunning = false;
                float tOut = 0f;
                while(tOut < 1f)
                {
                    tOut += Time.deltaTime;
                    yield return null;
                }

                tilemapper.MoveToNextScreen();
                SceneManager.LoadScene("Interstitial");
            }
        }
    }

    IEnumerator TurboCountdown()
    {
        while (IsRunning)
        {
            _countdown -= Time.deltaTime;
            if (_countdown < 0)
            {
                _turboMode = !_turboMode; // turbo mode on/off toggle
                _countdown = _turboMode ? 3f : 10f;
                _speed = _turboMode ? TURBO_SPEED : NORMAL_SPEED;
            }

            _turboCountdown.text = _turboMode ? $"TURBO!!! For {Mathf.CeilToInt(_countdown)}" : $"Turbo in {Mathf.CeilToInt(_countdown)}";
            yield return null;
        }
    }

    void Update()
    {
        var right = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        var left = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        var up = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        var down = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        if (right)
        {
            _nextForward = Vector3.right;
            _nextRotation = Quaternion.identity;
        }
        else if (left)
        {
            _nextForward = Vector3.left;
            _nextRotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }

        if (up)
        {
            _nextForward = Vector3.up;
            _nextRotation = Quaternion.Euler(new Vector3(0, 0, 90));
        }
        else if (down)
        {
            _nextForward = Vector3.down;
            _nextRotation = Quaternion.Euler(new Vector3(0, 0, 270));
        }

        _debug.text = $"Score: {tilemapper.Score:000000}"; //   tilemapper.Score.ToString();
    }

    private bool IsAtScreenBounds(Vector3 pos)
    {
        return (pos.x == 40) || (pos.x == -1) || (pos.y == -1) || (pos.y == 21);
    }
}

