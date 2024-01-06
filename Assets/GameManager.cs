using TwitchIntegration;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : TwitchMonoBehaviour
{
    // Serialized fields for UI elements and game sprites
    // UI elements
    [SerializeField] TextMeshProUGUI rockUILbl;
    [SerializeField] TextMeshProUGUI paperUILbl;
    [SerializeField] TextMeshProUGUI scissorsUILbl;
    [SerializeField] TextMeshProUGUI countDownLbl;
    [SerializeField] TextMeshProUGUI RobotWinsLbl;
    [SerializeField] TextMeshProUGUI ChatWinsLbl;
    [SerializeField] Image ChatImage;
    [SerializeField] Image RobotImage;
    [SerializeField] Sprite[] Sprites;
    [SerializeField] TextMeshProUGUI InfoLabel;
    [SerializeField] TextMeshProUGUI TimerLabel;

    // Game counters and variables
    private int rockCount = 0;
    private int paperCount = 0;
    private int scissorsCount = 0;
    private float countDownCount = 10f;
    private Vector3 chatStrarting;
    private Vector3 robotStarting;
    private bool gameInprogress = false;
    private bool startAnimation;
    private bool isMovingUp = false;
    private bool canVote = true;
    private int optionIndex = 0;
    public float moveDistance = 20f; // Distance to move the hand up and down
    public float animationSpeed = 1.0f; // Speed of the animation
    public string[] options = { "Rock", "Paper", "Scissors", "Shoot", "" };
    private bool startGame;
    private float coolDownCount = 5f;

    // Enum to represent choices
    enum Choice
    {
        Rock,
        Paper,
        Scissors
    }

    // Start is called before the first frame update
    void Start()
    {
        // Initialize UI elements and starting game values
        Application.runInBackground = true;
        rockUILbl.SetText(rockCount.ToString());
        paperUILbl.SetText(paperCount.ToString());
        scissorsUILbl.SetText(scissorsCount.ToString());
        countDownLbl.SetText(countDownCount.ToString());
        RobotWinsLbl.SetText(PlayerPrefs.GetInt("RobotWins", 0).ToString());
        ChatWinsLbl.SetText(PlayerPrefs.GetInt("ChatWins", 0).ToString());
        ChatImage.sprite = Sprites[0];
        RobotImage.sprite = Sprites[0];
        chatStrarting = ChatImage.transform.position;
        robotStarting = RobotImage.transform.position;
        InfoLabel.text = "";
    }

    // Twitch commands for user inputs
    [TwitchCommand("rock", "Rock", "ROCK", "R", "r")]
    public void Rock()
    {
        // Increment 'Rock' count if voting is allowed
        if (canVote)
        {
            rockCount++;
            rockUILbl.SetText(rockCount.ToString());
        }

    }

    // Twitch commands for user inputs
    [TwitchCommand("paper", "Paper", "PAPER", "p", "P")]
    public void Paper()
    {
        // Increment 'Paper' count if voting is allowed
        if (canVote)
        {
            paperCount++;
            paperUILbl.SetText(paperCount.ToString());
        }
    }

    // Twitch commands for user inputs
    [TwitchCommand("scissors", "Scissors", "SCISSORS", "s", "S")]
    public void Scissors()
    {
        // Increment 'Scissors' count if voting is allowed
        if (canVote)
        {
            scissorsCount++;
            scissorsUILbl.SetText(scissorsCount.ToString());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Game flow control based on different game phases
        if (!gameInprogress)
        {
            if (countDownCount > 0)
            {
                countDownCount -= Time.deltaTime;
            }
            else
            {
                gameInprogress = true;
                startAnimation = true;
            }
        }
        else
        {
            if (GamePlayable())
            {
                if (startAnimation)
                {
                    print("ANIMATION");
                    AnimateHand();
                }
                else if (startGame)
                {
                    print("PLAY GAME");
                    PlayGame();
                }
                else
                {
                    if (coolDownCount > 0)
                    {
                        TimerLabel.text = "Next Voting Phase Starts In";
                        coolDownCount -= Time.deltaTime;
                    }
                    else
                    {
                        ResetGame();
                    }
                }
            }
            else
            {
                if (coolDownCount > 0)
                {
                    TimerLabel.text = "Next Voting Phase Starts In";
                    coolDownCount -= Time.deltaTime;
                    InfoLabel.text = "No Votes, Game Canceled";
                }
                else
                {
                    ResetGame();
                }
            }

        }

        float numberToShow = !gameInprogress && !startAnimation && !startGame ? countDownCount : coolDownCount;
        countDownLbl?.SetText(System.Math.Round(numberToShow, 0).ToString());

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    private bool GamePlayable()
    {
        // Checks if no votes are cast
        if (rockCount == 0 && paperCount == 0 && scissorsCount == 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    // Executes the game logic after the voting phase
    void PlayGame()
    {
        /// Determines player and computer choices and initiates the game
        canVote = false;
        Choice chatsChooice = rockCount >= paperCount ? rockCount >= scissorsCount ? Choice.Rock : Choice.Scissors : paperCount >= scissorsCount ? Choice.Paper : Choice.Scissors;
        MakeChoice((int)chatsChooice);
    }

    // Resets the game to its initial state
    void ResetGame()
    {
        // Resets game-related variables and UI elements
        print("RESET GAME");
        rockCount = 0;
        paperCount = 0;
        scissorsCount = 0;
        rockUILbl.SetText(rockCount.ToString());
        paperUILbl.SetText(paperCount.ToString());
        scissorsUILbl.SetText(paperCount.ToString());
        countDownCount = 10f;
        gameInprogress = false;
        RobotImage.transform.position = robotStarting;
        ChatImage.transform.position = chatStrarting;
        gameInprogress = false;
        startAnimation = false;
        startGame = false;
        TimerLabel.text = "Vote Now, Next Game Starts";
        ChatImage.sprite = Sprites[0];
        RobotImage.sprite = Sprites[0];
        InfoLabel.text = "";
        coolDownCount = 5f;
        optionIndex = 0;
        isMovingUp = true;
        canVote = true;

    }

    // Initiates the game based on player's choice
    public void MakeChoice(int playerChoice)
    {
        // Compares player and computer choices to determine the winner
        Choice player = (Choice)playerChoice;
        ChatImage.sprite = Sprites[playerChoice];
        int randomNumber = Random.Range(0, 3);
        Choice computer = (Choice)randomNumber;
        RobotImage.sprite = Sprites[randomNumber];

        string result = DetermineWinner(player, computer);

        InfoLabel.text = result;

        coolDownCount = 5f;
        startAnimation = false;
        startGame = false;

    }

    // Determines the winner of the game
    private string DetermineWinner(Choice player, Choice computer)
    {
        // Checks player and computer choices to determine the winner
        if (player == computer)
        {
            return "It's a tie!";
        }
        else if ((player == Choice.Rock && computer == Choice.Scissors) ||
                 (player == Choice.Paper && computer == Choice.Rock) ||
                 (player == Choice.Scissors && computer == Choice.Paper))
        {
            PlayerPrefs.SetInt("ChatWins", PlayerPrefs.GetInt("ChatWins", 0) + 1);
            ChatWinsLbl.SetText(PlayerPrefs.GetInt("ChatWins").ToString());
            return "Chat wins!";
        }
        else
        {
            PlayerPrefs.SetInt("RobotWins", PlayerPrefs.GetInt("RobotWins", 0) + 1);
            RobotWinsLbl.SetText(PlayerPrefs.GetInt("RobotWins", 0).ToString());
            return "Robot wins!";
        }
    }

    // Animates the hand movement during the game
    void AnimateHand()
    {
        // Controls the movement of hand images based on game phases
        float moveAmount = moveDistance * animationSpeed * Time.deltaTime;
        int curentIndex = optionIndex;

        if (isMovingUp)
        {
            ChatImage.transform.Translate(Vector2.up * moveAmount);
            RobotImage.transform.Translate(Vector2.up * moveAmount);
        }
        else
        {
            ChatImage.transform.Translate(Vector2.down * moveAmount);
            RobotImage.transform.Translate(Vector2.down * moveAmount);
        }

        if (ChatImage.transform.localPosition.y >= moveDistance)
        {
            isMovingUp = false;
            InfoLabel.text = options[curentIndex];
        }
        else if (ChatImage.transform.localPosition.y <= 0)
        {
            isMovingUp = true;
            InfoLabel.text = "";
            curentIndex = (optionIndex++) % options.Length;
            if (optionIndex == 4)
            {
                startAnimation = false;
                startGame = true;
                countDownCount = 15f;
                optionIndex = 0;
            }
        }
    }
}
