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

    // �Q�[���i�s�󋵂��Ƃ̏���������ϐ�
    private Action[] audioFunc = new Action[Enum.GetValues(typeof(GameController.GameProgress)).Length];

    // ���݂̐i�s�󋵂�1�t���[���O�̐i�s��
    GameController.GameProgress gameProg;
    GameController.GameProgress oldGameProg;

    // Start is called before the first frame update
    void Start()
    {
        // �����ݒ�
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
    /// �Q�[���i�s�󋵂��Ƃ̏�����ݒ�
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
    /// �I�[�f�B�I�Đ����̏������s��
    /// </summary>
    /// <param name="nowGameProgress">���݂̃Q�[���i�s��</param>
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
        // inGameBGM�ɕύX���čĐ�
        audioSource.clip = inGame;
        audioSource.Play();
    }

    private void PlayGameClearAudio()
    {
        // BGM���~�߂ăN���ASE���Đ�
        audioSource.Stop();
        audioSource.PlayOneShot(gameClear);
    }

    private void PlayGameOverAudio()
    {
        // gameOverBGM�ɕύX���čĐ�
        audioSource.clip = gameOver;
        audioSource.Play();
    }

    private void PlayResultAudio()
    {
        return;
    }
}
