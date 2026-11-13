using UnityEngine;
using UnityEngine.UI;

public class CardController : MonoBehaviour
{
    public Image icon;
    public int id;
    public bool isFlipped = false;
    public bool isMatched = false;

    private Button currentBtn;
    private Animator animator;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        currentBtn = GetComponent<Button>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        if (currentBtn != null)
        {
            currentBtn.onClick.AddListener(OnCardClicked);
        }
    }

    private void Start()
    {
        // Random vị trí sibling mỗi khi ấn Play
        if (transform.parent != null && transform.parent.childCount > 1)
        {
            transform.SetSiblingIndex(Random.Range(0, transform.parent.childCount));
        }
    }

    public void InitCard(int _id, Sprite iconSprite)
    {
        id = _id;
        if (icon != null)
        {
            icon.sprite = iconSprite;
        }
        isFlipped = false;
        isMatched = false;
    }

    public void OnCardClicked()
    {
        if (isFlipped || isMatched || GameManager.Instance == null || !GameManager.Instance.CanFlip())
        {
            return;
        }

        isFlipped = true;
        if (animator != null)
        {
            animator.SetBool("isFlip", true);
        }
        GameManager.Instance.OnCardFlipped(this);
    }

    public void Unflip()
    {
        isFlipped = false;
        if (animator != null)
        {
            animator.SetBool("isFlip", false);
        }
    }

    public void HideCard()
    {
        isMatched = true;
        if (currentBtn != null)
        {
            currentBtn.interactable = false;
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }

        Image rootImage = GetComponent<Image>();
        if (rootImage != null)
        {
            rootImage.enabled = false;
        }

        foreach (Image childImg in GetComponentsInChildren<Image>(true))
        {
            childImg.enabled = false;
        }
    }
}
