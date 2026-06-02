using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

/// <summary>
/// GhostBoardScreen.cs
/// Ghost's board screen. The ghost can:
///   1. Shift rows/columns using edge arrows (maze manipulation) - once per turn
///   2. Move their token tile by tile using a D-pad - up to 3 steps per turn
///
/// SCENE NAME: "GhostBoard"
///
/// INSPECTOR SETUP:
/// - gridContainer      → RectTransform holding the 6x6 cell images
/// - cellPrefab         → Prefab with an Image component
/// - arrowButtonPrefab  → Prefab with Button + TMP_Text child
/// - endTurnButton      → Button to end the ghost's turn
/// - dpadUp/Down/Left/Right → Four buttons for ghost movement
/// - roundLabel         → TMP_Text top-left, yellow, shows round number
/// - movesLabel         → TMP_Text showing remaining moves
/// - shiftLabel         → TMP_Text showing whether shift has been used
/// - nextSceneName      → "GhostStart"
/// </summary>
public class GhostBoardScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject arrowButtonPrefab;
    [SerializeField] private Button endTurnButton;

    [Header("D-Pad (Ghost Movement)")]
    [SerializeField] private Button dpadUp;
    [SerializeField] private Button dpadDown;
    [SerializeField] private Button dpadLeft;
    [SerializeField] private Button dpadRight;

    [Header("HUD Labels")]
    [SerializeField] private TMP_Text roundLabel;  // top-left yellow round counter
    [SerializeField] private TMP_Text movesLabel;  // remaining move steps
    [SerializeField] private TMP_Text shiftLabel;  // whether board shift is available

    [Header("Scene Navigation")]
    // [SerializeField] private string nextSceneName = "InvestigatorStart";
    // no investigator use of the app
    [SerializeField] private string nextSceneName = "GhostStart";

    [Header("Visual Settings")]
    [SerializeField] private Color wallColor = new Color(0.22f, 0.25f, 0.32f);
    [SerializeField] private Color passageColor = new Color(0.10f, 0.11f, 0.14f);
    [SerializeField] private Color investigatorColor = new Color(0.27f, 0.60f, 0.85f);
    [SerializeField] private Color ghostColor = new Color(0.85f, 0.85f, 1.00f);

    [Header("Arrow Settings")]
    [SerializeField] private float arrowOffset = 50f;
    [SerializeField] private float arrowSize = 80f;
    // arrowSize exposed to inspector so you can tweak without touching code

    private int gridSize;
    private Image[,] cellImages;
    private Canvas rootCanvas;

    // functions
    void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("open from the ghoststart screen, moron");
            return;
        }

        gridSize = GameManager.Instance.gridSize;
        cellImages = new Image[gridSize, gridSize];
        rootCanvas = GetComponentInParent<Canvas>();

        BuildGrid();
        RefreshGrid();
        RefreshHUD();

        //listeners for dpad
        dpadUp.onClick.AddListener(() => TryMoveGhost(-1, 0));
        dpadDown.onClick.AddListener(() => TryMoveGhost(1, 0));
        dpadLeft.onClick.AddListener(() => TryMoveGhost(0, -1));
        dpadRight.onClick.AddListener(() => TryMoveGhost(0, 1));

        endTurnButton.onClick.AddListener(OnEndTurn);

        StartCoroutine(BuildArrowsNextFrame());
    }

    IEnumerator BuildArrowsNextFrame()
    {
        //force the grid layout to fully calculate before we read its corners
        //this is what was causing the slight arrow misalignment previously
        LayoutRebuilder.ForceRebuildLayoutImmediate(gridContainer);
        yield return null;
        yield return null;
        // two frames to be safe - layout rebuild can take a frame to propagate
        BuildArrows();
    }

    //visual 6x6 grid 
    void BuildGrid()
    {
        GridLayoutGroup layout = gridContainer.GetComponent<GridLayoutGroup>();
        if (layout != null)
        {
            layout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            layout.constraintCount = gridSize;
        }

        for (int r = 0; r < gridSize; r++)
            for (int c = 0; c < gridSize; c++)
            {
                GameObject cell = Instantiate(cellPrefab, gridContainer);
                cell.name = $"Cell_{r}_{c}";
                cellImages[r, c] = cell.GetComponent<Image>();
            }
    }

    //ghost movement functions
    void TryMoveGhost(int dRow, int dCol)
    {
        //block movement if no moves left this turn
        if (GameManager.Instance.movesRemaining <= 0)
            return;

        int newRow = GameManager.Instance.ghostRow + dRow;
        int newCol = GameManager.Instance.ghostCol + dCol;

        //checking bounds
        if (newRow < 0 || newRow >= gridSize || newCol < 0 || newCol >= gridSize)
            return;

        //ghost can pass through walls, this is subject to change, at the very least wall movement parameters / illegal moves must 
        //- be created, and the possibility remains of the removal of the moving thru walls feature entirely .

        GameManager.Instance.ghostRow = newRow;
        GameManager.Instance.ghostCol = newCol;
        GameManager.Instance.movesRemaining--;

        RefreshGrid();
        RefreshHUD();
    }

    // ── Spawn edge arrows for row/column shifting
    void BuildArrows()
    {
        Vector3[] corners = new Vector3[4];
        gridContainer.GetWorldCorners(corners);
        // corners: 0=bottom-left, 1=top-left, 2=top-right, 3=bottom-right

        RectTransform canvasRect = rootCanvas.GetComponent<RectTransform>();

        Vector2 bottomLeft = WorldToCanvasLocal(corners[0], canvasRect);
        Vector2 topLeft = WorldToCanvasLocal(corners[1], canvasRect);
        Vector2 topRight = WorldToCanvasLocal(corners[2], canvasRect);

        float gridWidth = topRight.x - topLeft.x;
        float gridHeight = topLeft.y - bottomLeft.y;
        float cellW = gridWidth / gridSize;
        float cellH = gridHeight / gridSize;

        for (int i = 0; i < gridSize; i++)
        {
            //centre of each cell along each axis
            float cellCentreX = topLeft.x + (i * cellW) + (cellW / 2f);
            float cellCentreY = topLeft.y - (i * cellH) - (cellH / 2f);

            SpawnArrow("→", "row", i, 1, new Vector2(bottomLeft.x - arrowOffset, cellCentreY));
            SpawnArrow("←", "row", i, -1, new Vector2(topRight.x + arrowOffset, cellCentreY));
            SpawnArrow("↓", "col", i, 1, new Vector2(cellCentreX, topLeft.y + arrowOffset));
            SpawnArrow("↑", "col", i, -1, new Vector2(cellCentreX, bottomLeft.y - arrowOffset));
        }
    }

    Vector2 WorldToCanvasLocal(Vector3 worldPos, RectTransform canvasRect)
    {
        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect, screenPoint, null, out Vector2 localPoint);
        return localPoint;
    }

    void SpawnArrow(string label, string axis, int index, int direction, Vector2 canvasLocalPos)
    {
        GameObject btn = Instantiate(arrowButtonPrefab, rootCanvas.transform);
        btn.transform.SetAsLastSibling();
        btn.name = $"Arrow_{axis}_{index}_{(direction > 0 ? "pos" : "neg")}";

        RectTransform rt = btn.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = canvasLocalPos;
        rt.sizeDelta = new Vector2(arrowSize, arrowSize);
        // size driven by inspector field now, was hardcoded 65f before

        TMP_Text text = btn.GetComponentInChildren<TMP_Text>();
        if (text != null) text.text = label;

        Button button = btn.GetComponent<Button>();
        string capturedAxis = axis;
        int capturedIndex = index;
        int capturedDir = direction;
        button.onClick.AddListener(() => OnArrowPressed(capturedAxis, capturedIndex, capturedDir));
    }

    void OnArrowPressed(string axis, int index, int direction)
    {
        //block if shift already used this turn
        if (GameManager.Instance.hasShiftedBoard)
            return;

        if (axis == "row")
            GameManager.Instance.ShiftRow(index, direction);
        else
            GameManager.Instance.ShiftColumn(index, direction);

        RefreshGrid();
        RefreshHUD();
    }

    //updates round counter, move count, and shift availability text
    //also greys out dpad when moves run out
    void RefreshHUD()
    {
        if (roundLabel != null)
            roundLabel.text = $"Round {GameManager.Instance.roundNumber}";

        if (movesLabel != null)
            movesLabel.text = $"Moves: {GameManager.Instance.movesRemaining}";

        if (shiftLabel != null)
            shiftLabel.text = GameManager.Instance.hasShiftedBoard ? "Shift: Used" : "Shift: Available";

        //grey out dpad buttons when no moves remain
        bool canMove = GameManager.Instance.movesRemaining > 0;
        dpadUp.interactable = canMove;
        dpadDown.interactable = canMove;
        dpadLeft.interactable = canMove;
        dpadRight.interactable = canMove;
    }

    // ── Redraw all cells ──────────────────────────────────────────────────────
    void RefreshGrid()
    {
        int[,] grid = GameManager.Instance.grid;
        int invRow = GameManager.Instance.investigatorRow;
        int invCol = GameManager.Instance.investigatorCol;
        int ghRow = GameManager.Instance.ghostRow;
        int ghCol = GameManager.Instance.ghostCol;

        for (int r = 0; r < gridSize; r++)
        {
            for (int c = 0; c < gridSize; c++)
            {
                // Ghost takes priority visually if on same cell as investigator
                if (r == ghRow && c == ghCol)
                    cellImages[r, c].color = ghostColor;
                else if (r == invRow && c == invCol)
                    cellImages[r, c].color = investigatorColor;
                else
                    cellImages[r, c].color = grid[r, c] == 1 ? wallColor : passageColor;
            }
        }
    }

    // ── End turn ──────────────────────────────────────────────────────────────
    void OnEndTurn()
    {
        GameManager.Instance.EndGhostTurn();
        endTurnButton.interactable = false;
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        StopAllCoroutines();
        endTurnButton.onClick.RemoveAllListeners();
        dpadUp.onClick.RemoveAllListeners();
        dpadDown.onClick.RemoveAllListeners();
        dpadLeft.onClick.RemoveAllListeners();
        dpadRight.onClick.RemoveAllListeners();
    }
}
