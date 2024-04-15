using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CanvasController : MonoBehaviour
{
    [SerializeField]
    private GameController gameController;

    [SerializeField]
    private Canvas gameClearCanvas;

    [SerializeField]
    private Canvas gameOverCanvas;

    [SerializeField]
    private Animator animeGameClear;

    [SerializeField]
    private Animator animeGameOver;

    [SerializeField]
    private Button nextButton;

    [SerializeField]
    private Button retryButton;

    [SerializeField]
    private Button titleButton;

    private Action[] enableCanvasFunc = new Action[Enum.GetValues(typeof(GameController.GameProgress)).Length];
    // Start is called before the first frame update
    private void Start()
    {
        // ÉLÉÉÉìÉoÉXÇñ≥å¯âª
        gameClearCanvas.enabled = false;
        gameOverCanvas.enabled = false;

        // ä÷êîÇê›íË
        enableCanvasFunc[(int)GameController.GameProgress.start] = DefaultCanvas;
        enableCanvasFunc[(int)GameController.GameProgress.playing] = DefaultCanvas;
        enableCanvasFunc[(int)GameController.GameProgress.goal] = EnableGameClearCanvas;
        enableCanvasFunc[(int)GameController.GameProgress.gameover] = EnableGameOverCanvas;
        enableCanvasFunc[(int)GameController.GameProgress.result] = DefaultCanvas;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        enableCanvasFunc[(int)gameController.gameProgress]();
    }

    private void EnableGameClearCanvas()
    {
        if (!gameClearCanvas.enabled)
        {
            animeGameClear.SetTrigger("GameClear");
            nextButton.Select();
            gameClearCanvas.enabled = true;
        }

        if(SceneManager.GetActiveScene().name == "GameScene")
        {
            nextButton.image.enabled = false;
            titleButton.image.enabled = false;
        }
    }

    private void EnableGameOverCanvas()
    {
        if (!gameOverCanvas.enabled)
        {
            animeGameOver.SetBool("GameOver", true);
            retryButton.Select();
            titleButton.image.enabled = true;
            gameOverCanvas.enabled = true;
        }
    }

    private void DefaultCanvas()
    {
        return;
    }
}
