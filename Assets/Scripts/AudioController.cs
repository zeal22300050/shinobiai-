using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioController : MonoBehaviour
{
    [SerializeField]
    private AudioClip gameOver;
    [SerializeField]
    private AudioClip gameClear;
    [SerializeField]
    private AudioClip inGame;

    [SerializeField]
    private GameController gameController;

    private AudioSource audioSource;

    // ゲーム進行状況ごとの処理を入れる変数
    private Action[] audioFunc = new Action[Enum.GetValues(typeof(GameController.GameProgress)).Length];

    // 現在の進行状況と1フレーム前の進行状況
    GameController.GameProgress gameProg;
    GameController.GameProgress oldGameProg;

    // Start is called before the first frame update
    void Start()
    {
        // 初期設定
        audioSource = GetComponent<AudioSource>();
        oldGameProg = gameController.gameProgress;
        SetAudioFunction();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameProg = gameController.gameProgress;

        if (gameProg != oldGameProg)
        {
            PlayAudioProcess(gameProg);
        }

        oldGameProg = gameController.gameProgress;
    }

    /// <summary>
    /// ゲーム進行状況ごとの処理を設定
    /// </summary>
    private void SetAudioFunction()
    {
        audioFunc[(int)GameController.GameProgress.start] = PlayAppearAudio;
        audioFunc[(int)GameController.GameProgress.playing] = PlayInGameAudio;
        audioFunc[(int)GameController.GameProgress.goal] = PlayGameClearAudio;
        audioFunc[(int)GameController.GameProgress.gameover] = PlayGameOverAudio;
        audioFunc[(int)GameController.GameProgress.result] = PlayResultAudio;
    }

    /// <summary>
    /// オーディオ再生時の処理を行う
    /// </summary>
    /// <param name="nowGameProgress">現在のゲーム進行状況</param>
    private void PlayAudioProcess(GameController.GameProgress nowGameProgress)
    {
        audioFunc[(int)nowGameProgress]();
    }

    private void PlayAppearAudio()
    {
        return;
    }

    private void PlayInGameAudio()
    {
        // inGameBGMに変更して再生
        audioSource.clip = inGame;
        audioSource.Play();
    }

    private void PlayGameClearAudio()
    {
        // BGMを止めてクリアSEを再生
        audioSource.Stop();
        audioSource.PlayOneShot(gameClear);
    }

    private void PlayGameOverAudio()
    {
        // gameOverBGMに変更して再生
        audioSource.clip = gameOver;
        audioSource.Play();
    }

    private void PlayResultAudio()
    {
        return;
    }
}
