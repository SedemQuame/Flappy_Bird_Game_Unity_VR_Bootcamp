using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;

public class GameOverWindow : MonoBehaviour {

    private Text scoreText;
    private Text highscoreText;

    private void Awake() {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        //highscoreText = transform.Find("highscoreText").GetComponent<Text>();
        
        transform.Find("retryBtn").GetComponent<Button_UI>().ClickFunc = () => {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        };
    }

    private void Start() {
        Bird.GetInstance().OnDied += Bird_OnDied;
        Hide();
    }

    private void Update() {

    }

    private void Bird_OnDied(object sender, System.EventArgs e) {
        scoreText.text = (Level.GetInstance().GetPipesPassedBird() / 2).ToString(); 

        Show();
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }

}
