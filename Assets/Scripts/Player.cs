using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] GameManager _gameManager;
    [SerializeField] LabyrinthCreator _labyrinthCreator;

    [SerializeField] private float speed;
    [SerializeField] private GameObject mesh;
    [SerializeField] private GameObject deathPlayer;
    [SerializeField] private ParticleSystem winEffect;

    private bool _move;
    private bool _isInvincible;

    private int _nextPathIndex = -1;
    private float _timerTime = 2f;

    private Vector3[] _path;
    private Material _material;
    private Coroutine _startTimer;

    private readonly Color _defaultColor = new Color(1f, 1f, 0f);

    private readonly Color _invincibleColor = new Color(0.67843f, 1f, 0.18431f);

    private static readonly int Explode = Animator.StringToHash("Explode");

    private void Start()
    {
        _labyrinthCreator.OnPathChanged += OnPathChanged;
    }


    public void OnGameStart()
    {
        _startTimer = StartCoroutine(StartTimer());
    }

    public void OnGamePause()
    {
        if (_startTimer != null)
        { StopCoroutine(_startTimer); }

        _move = false;
    }

    public void OnGameContinue()
    {
        if (_timerTime > 0) _startTimer = StartCoroutine(StartTimer());
        else _move = true;
    }

    private void FixedUpdate()
    {
        if (_move && _path != null && _nextPathIndex >= 0 && _nextPathIndex < _path.Length)
        {
            transform.position = Vector3.MoveTowards(transform.position, _path[_nextPathIndex], speed);

            if (Vector3.Distance(transform.position, _path[_nextPathIndex]) < 0.1f) _nextPathIndex--;
        }
    }

    private void OnPathChanged(Vector3[] path)
    {
        _path = path;
        _nextPathIndex = _path.Length - 1;
    }

    public void SetInvincible(bool value)
    {
        _isInvincible = value;

        _material.color = _isInvincible ? _invincibleColor : _defaultColor;
    }

    public void TryToKill()
    {
        if (_isInvincible) return;

        mesh.SetActive(false);
        var player = Instantiate(deathPlayer, transform.position, Quaternion.identity);
        

        StartCoroutine(_gameManager._pauseMenu.AlphaCoroutine(0, 0, null, 0.1f, () =>
        {
            mesh.SetActive(true);
            Destroy(player.gameObject);
            RestartPlayer();
        }));
    }

    public void Finish()
    {
        winEffect.Play();
    }

    public void ResetPlayer()
    {
        _move = false;
        _path = null;
        _nextPathIndex = -1;
        transform.position = Vector3.zero;
    }

    private void RestartPlayer()
    {
        _nextPathIndex = _path.Length - 1;
        transform.position = Vector3.zero;
    }

    private IEnumerator StartTimer()
    {
        while (_timerTime > 0)
        {
            yield return new WaitForSeconds(0.1f);
            _timerTime -= 0.1f;
        }
        _move = true;
    }
    private void Awake()
    {
        _material = mesh.GetComponent<MeshRenderer>().material;
    }

    private void OnDestroy()
    {
        _labyrinthCreator.OnPathChanged -= OnPathChanged;
    }
}