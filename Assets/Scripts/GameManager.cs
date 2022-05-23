using System;
using System.Collections;
using UnityEngine;
public class GameManager : MonoBehaviour
{
    public PauseMenu _pauseMenu;
    public Player _player;
    public LabyrinthCreator _labyrinthCreator;

    [SerializeField] private CanvasGroup loaderPause;

    public void OnLabyrinthGenerated()
    {
        StartCoroutine(_pauseMenu.AlphaCoroutine(1f, 0f, loaderPause, callback: () => _player.OnGameStart()));
    }
    
    public void Finish()
    {
        _player.Finish();
        StartCoroutine(_pauseMenu.AlphaCoroutine(0f, 1f, loaderPause, 2, ResetGame));
    }
    
    public void ContinueGame()
    {
        _player.OnGameContinue();
        _pauseMenu.SetPauseState(false);
    }
    
    public void PauseGame()
    {
        _player.OnGamePause();
        _pauseMenu.SetPauseState(true);
    }

    private void ResetGame()
    {
        _player.ResetPlayer();
        _labyrinthCreator.GenerateLabyrinth();
    }

}
