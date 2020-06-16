using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    Text scoreText;
    [SerializeField]
    Text movesText;
    [SerializeField]
    Text bombCountText;

    public Transform bombHexagonT;
    public static UIManager ins;

    int score = 0;
    int movesCount = 0;

    int bombCount = -1;
    int startBombCount = 5;

    bool isBombCount;
    public bool IsBombCount { 
        get {
            return isBombCount;
        } 
        set {
            if (value == true){
                bombCount = startBombCount;
            } else {
                bombCountText.text = "";
            }
            isBombCount = value;
        }
    }

    public Text infoText;

    private void Awake()
    {
        if (ins == null)
            ins = this;
        else
        {
            Destroy(gameObject);
            print("Destroy Gama Manager");
        }
        isBombCount = false;
    }

    private void Update() {
        if (bombHexagonT != null) {
            bombCountText.transform.position = bombHexagonT.position;
        }

        if (Input.GetKeyDown(KeyCode.G)) {
            score = 970;
            scoreText.text = "Score\n" + score;
        }
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetScore() {
        score += 5;
        scoreText.text = "Score\n" + score;

        if (score % 1000 == 0)
        {
            IsBombCount = true;
        }
    }

    public void SetMovesAndBombCount()
    {
        //Bomba geri sayımı
        if (IsBombCount)
            SetBombCount();

        // //Bütün animasyonlar bittikten sonraki sayaç
        movesCount++;
        movesText.text = "Moves\n" + movesCount;
    }

    void SetBombCount()
    {
        bombCount--;
        bombCountText.text = bombCount.ToString();
        if (bombCount == 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        Time.timeScale = 0;
        print("Game Over");
    }
}