using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class PopUpHandler : MonoBehaviour
{
    private GameObject darkOverlay;
    private PopUp[] popUps;
    public bool popupActive;

    public GameObject manaVisionColorIndexPrefab;
    public GameObject manaVisionTextParent;
    private RectTransform manaVisionTextTransform;
    private RectTransform anykeyexitTextTransform;
    private TextMeshProUGUI manaVisionText;
    private TextMeshProUGUI anyKeyExitText;
    private Coroutine textAnimationRoutine;
    private bool Vpressed = false;
    private Vector2 startPos_Mana;
    private Vector2 startPos_AnyKey;
    private bool manavisionavailable = false;

    private int _activePopUps;
    public int activePopUps //Dark layer reacts to this variable
    {
        get
        {
            return _activePopUps;
        }
        set
        {
            if (value < 0)
            {
                Debug.Log("negative number of popups are on screen");
            }
            else if (value == 0)
            {
                popupActive = false;
                DarkenDown();
            }
            else
            {
                popupActive = true;
                DarkenUp();
            }
            _activePopUps = value;
        }
    }

    void Start()
    {
        darkOverlay = transform.Find("DarkLayer").gameObject; //find returns the this.transform's child's transform with a specific name
        popUps = new PopUp[]
        {
            new PopUp.AddNewMap(),
            new PopUp.PauseScreen(),
            new PopUp.ManaVision()
        };
        if (!(SceneManager.GetActiveScene().name == "LevelGroupTest" || SceneManager.GetActiveScene().name == "TestTempMap") ||
            GetComponentsInChildren<RectTransform>(true).FirstOrDefault(t => t.name == "ManaVisionText") == null ||
            GetComponentsInChildren<RectTransform>(true).FirstOrDefault(t => t.name == "AnyKeyExitText") == null ||
            GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "ManaVisionText") == null ||
            GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "AnyKeyExitText") == null) Debug.Log("manavision unavailable");
        else
        {
            manaVisionTextTransform = GetComponentsInChildren<RectTransform>(true).FirstOrDefault(t => t.name == "ManaVisionText");
            anykeyexitTextTransform = GetComponentsInChildren<RectTransform>(true).FirstOrDefault(t => t.name == "AnyKeyExitText");
            manaVisionText = GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "ManaVisionText");
            anyKeyExitText = GetComponentsInChildren<TextMeshProUGUI>(true).FirstOrDefault(t => t.name == "AnyKeyExitText");
            startPos_Mana = manaVisionTextTransform.anchoredPosition;
            startPos_AnyKey = anykeyexitTextTransform.anchoredPosition;
            manavisionavailable = true;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (popupActive) Down(-1);
            else if (SceneManager.GetActiveScene().name != "MapEditor") popUps[1].Up(); //raise menu
        }

        if (manavisionavailable)
        {
            if (!Vpressed && popUps[2].active && Input.anyKey)
            {
                popUps[2].Down();
            }
            else if (!Vpressed && Input.GetKeyDown(KeyCode.V))
            {
                popUps[2].Up();
            }
            if (Input.GetKeyDown(KeyCode.V)) Vpressed = true;
            else if (Input.GetKeyUp(KeyCode.V)) Vpressed = false;
        }
    }

    public void DarkenUp()
    {
        darkOverlay.SetActive(true);
    }

    public void DarkenDown()
    {
        darkOverlay.SetActive(false);
    }

    public void Down(int id = -1) //-1 Disables all popups
    {
        if (id == -1)
        {
            for (int i = 0; i < popUps.Length; i++)
            {
                popUps[i].Down();
            }
        }
        else popUps[id].Down();
    }


    public void SetNewMapNameByInputField(string text) => popUps[0].addNewMap.SetNewMapName(text);
    public void NewMapButtonPress() => popUps[0].addNewMap.ButtonPress();

    public void BackToMainMenu() => popUps[1].pauseScreen.BackToMainMenu();

    //helper functions using monobehaviour for animating manavision texts:
    public void StartTextAnimation()
    {
        // Stop any previous animation so we always restart from the beginning
        if (textAnimationRoutine != null)
        {
            StopCoroutine(textAnimationRoutine);
        }

        textAnimationRoutine = StartCoroutine(SlideAndFadeTexts());
    }

    public void StopTextAnimation()
    {
        // If the overlay is turned off while the text is still animating, stop the coroutine
        if (textAnimationRoutine != null)
        {
            StopCoroutine(textAnimationRoutine);
            textAnimationRoutine = null;
        }

        HideTextImmediately();
    }

    private IEnumerator SlideAndFadeTexts()
    {
        // Decide how many seconds you want the animation to last
        float duration = 1.5f;
        float elapsed = 0f;

        // How far (and in what direction) you want them to slide by the end
        Vector2 offset = new Vector2(130f, 0f); // slides 150px to the right, for example

        manaVisionTextTransform.anchoredPosition = startPos_Mana;
        anykeyexitTextTransform.anchoredPosition = startPos_AnyKey;
        SetTextAlpha(1f);

        // Now animate over 'duration' seconds
        while (elapsed < duration)
        {
            // Use unscaledDeltaTime so it ignores timeScale (works even when paused)
            float dt = Time.unscaledDeltaTime;
            elapsed += dt;

            float t = Mathf.Clamp01(elapsed / duration);

            // Slide to the right
            manaVisionTextTransform.anchoredPosition = Vector2.Lerp(
                startPos_Mana,
                startPos_Mana + offset,
                t
            );

            anykeyexitTextTransform.anchoredPosition = Vector2.Lerp(
                startPos_AnyKey,
                startPos_AnyKey + offset,
                t
            );

            // Fade from alpha=1 to alpha=0
            float alpha = Mathf.Lerp(1f, 0f, t);
            SetTextAlpha(alpha);

            yield return null;
        }

        // Once time is up, hide completely
        SetTextAlpha(0f);
        textAnimationRoutine = null;
    }
    private void HideTextImmediately()
    {
        SetTextAlpha(0f);
    }

    private void SetTextAlpha(float alpha)
    {
        if (manaVisionText != null)
        {
            var c = manaVisionText.color;
            c.a = alpha;
            manaVisionText.color = c;
        }
        if (anyKeyExitText != null)
        {
            var c = anyKeyExitText.color;
            c.a = alpha;
            anyKeyExitText.color = c;
        }
    }

    private void PlaceTextAt(int i, int j, int colorToWrite = -1, int interactiveToWrite = -1)
    {
        GameObject instance = Instantiate(manaVisionColorIndexPrefab, manaVisionTextParent.transform);

        var textComponent = instance.GetComponentInChildren<TextMeshProUGUI>();
        textComponent.text = 
            (colorToWrite != -1 ? colorToWrite.ToString() : "") +  "\n" +
            (interactiveToWrite != -1 ? "->" + interactiveToWrite.ToString() : "");

        RectTransform rt = instance.GetComponent<RectTransform>();

        float cellWidth = Screen.width / 32f;  // Divide into 32 columns
        float cellHeight = Screen.height / 18f; // Divide into 18 rows
        float x = (j + 1.5f) * cellWidth;
        float y = (i + 1) * cellHeight;

        rt.anchoredPosition = new Vector2(y + 5, x);
    }

    public void CreateManaVisionTexts()
    {
        WallManager wm;
        if (FindFirstObjectByType<WallManager>() == null) { Debug.Log("wm is null"); return; }
        else wm = FindFirstObjectByType<WallManager>();
        Map map;
        if (FindFirstObjectByType<Map>() == null) { Debug.Log("map is null"); return; }
        else map = FindFirstObjectByType<Map>();

        for (int i = 0; i < wm.wallPositions.Length; i++)
        {
            for (int j = 0; j < wm.wallPositions[i].Length; j++)
            {
                int val = wm.wallPositions[i][j];
                if (val > 0 && wm.colors.atVisible(val))
                {
                    PlaceTextAt(i, j, val);
                }
            }
        }
        GameObject InteractiveParent = GameObject.Find("ThingParent");
        foreach (Transform interactiveObject in InteractiveParent.transform)
        {
            int colorId = -1, interactiveColor = -1;
            if (interactiveObject.GetComponent<Buttons>() != null) {
                colorId = interactiveObject.GetComponent<Buttons>().colorIndex;
                interactiveColor = interactiveObject.GetComponent<Buttons>().interactWithColor;
            }
            if (interactiveObject.GetComponent<ButtonsForCube>() != null) {
                colorId = interactiveObject.GetComponent<ButtonsForCube>().colorIndex;
                interactiveColor = interactiveObject.GetComponent<ButtonsForCube>().cubeColor;
            }
            if (interactiveObject.GetComponent<ButtonTimerCube>() != null) {
                colorId = interactiveObject.GetComponent<ButtonTimerCube>().colorIndex;
                interactiveColor = interactiveObject.GetComponent<ButtonTimerCube>().cubeColor;
            }
            if (interactiveObject.GetComponent<Lever>() != null) {
                colorId = interactiveObject.GetComponent<Lever>().colorIndex;
                interactiveColor = interactiveObject.GetComponent<Lever>().interactWithColor;
            }
            if (interactiveObject.GetComponent<Portal>() != null) {
                colorId = interactiveObject.GetComponent<Portal>().colorIndex;
                interactiveColor = interactiveObject.GetComponent<Portal>().portalIndex;
            }
            if (interactiveObject.GetComponent<GateLight>() != null) {
                colorId = interactiveObject.GetComponent<GateLight>().colorIndex;
            }
            PlaceTextAt(Mathf.RoundToInt(interactiveObject.transform.position.x + 14.5f), Mathf.RoundToInt(interactiveObject.position.y + 7.5f),
                colorId, interactiveColor);
        }
    }

    public void DeleteManaVisionTexts()
    {
        foreach (Transform child in manaVisionTextParent.transform)
        {
            if (child.name == "ManaVisionText" || child.name == "AnyKeyExitText") continue;
            Destroy(child.gameObject);
        }
    }
}
