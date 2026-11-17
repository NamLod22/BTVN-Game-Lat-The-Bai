using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Transform cardContainer;
    public GameObject cardPrefabs;
    public Sprite[] cardIcons;
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public GameObject victoryUI;
    public GameObject gameOverUI;
    public float timeRemaining = 300f;

    private CardController firstCard;
    private CardController secondCard;
    private bool isChecking = false;
    private bool isGameEnded = false;
    private int score = 0;
    private int matchedPairs = 0;
    private int totalPairs = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (cardContainer != null)
        {
            for (int i = cardContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(cardContainer.GetChild(i).gameObject);
            }
        }

        if (cardIcons != null && cardPrefabs != null && cardContainer != null)
        {
            totalPairs = cardIcons.Length;
            for (int i = 0; i < cardIcons.Length; i++)
            {
                GameObject card1 = Instantiate(cardPrefabs, cardContainer);
                GameObject card2 = Instantiate(cardPrefabs, cardContainer);

                CardController c1 = card1.GetComponent<CardController>();
                CardController c2 = card2.GetComponent<CardController>();
                if (c1 != null) c1.InitCard(i, cardIcons[i]);
                if (c2 != null) c2.InitCard(i, cardIcons[i]);
            }

            
            int count = cardContainer.childCount;
            for (int i = count - 1; i > 0; i--)
            {
                int rand = Random.Range(0, i + 1);
                cardContainer.GetChild(rand).SetSiblingIndex(i);
            }
        }

        score = 0;
        matchedPairs = 0;
        timeRemaining = 300f;
        isGameEnded = false;
        UpdateScoreUI();
        UpdateTimerUI();

        if (victoryUI != null)
        {
            victoryUI.SetActive(false);
        }

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(false);
        }
    }

    private void Update()
    {
        if (isGameEnded)
        {
            return;
        }

        timeRemaining -= Time.deltaTime;
        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            isGameEnded = true;
            StopAllCoroutines();
            isChecking = false;
            UpdateTimerUI();

            if (gameOverUI != null)
            {
                gameOverUI.SetActive(true);
            }
            return;
        }

        UpdateTimerUI();
    }

    public bool CanFlip()
    {
        return !isGameEnded && !isChecking && secondCard == null;
    }

    public void OnCardFlipped(CardController card)
    {
        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null && card != firstCard)
        {
            secondCard = card;
            StartCoroutine(CheckMatchRoutine());
        }
    }

    private IEnumerator CheckMatchRoutine()
    {
        isChecking = true;
        yield return new WaitForSeconds(0.75f);

        if (firstCard != null && secondCard != null && firstCard.id == secondCard.id)
        {
            firstCard.HideCard();
            secondCard.HideCard();

            score += 1;
            matchedPairs += 1;
            UpdateScoreUI();

            if (matchedPairs >= totalPairs)
            {
                isGameEnded = true;
                if (victoryUI != null)
                {
                    victoryUI.SetActive(true);
                }
            }
        }
        else
        {
            if (firstCard != null) firstCard.Unflip();
            if (secondCard != null) secondCard.Unflip();
            yield return new WaitForSeconds(0.4f);
        }

        firstCard = null;
        secondCard = null;
        isChecking = false;
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }
}
