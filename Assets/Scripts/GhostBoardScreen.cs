using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
public class GhostBoardScreen : MonoBehaviour
{
    private Dictionary<CellType, Sprite> tileSprites = new Dictionary<CellType, Sprite>();
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GameObject arrowButtonPrefab;
    [SerializeField] private Button endTurnButton;
    [SerializeField] private Button dpadUp;
    [SerializeField] private Button dpadDown;
    [SerializeField] private Button dpadLeft;
    [SerializeField] private Button dpadRight;
    [SerializeField] private TMP_Text roundLabel;
    [SerializeField] private TMP_Text movesLabel;
    [SerializeField] private TMP_Text shiftLabel;
    // [SerializeField] private string nextSceneName = "InvestigatorStart";
    // no investigator use of the app so thats why this line is commented out ^^^
    [SerializeField] private string nextSceneName = "GhostStart";
    [SerializeField] private Color wallColor = new Color(0.22f, 0.25f, 0.32f);
    [SerializeField] private Color passageColor = new Color(0.10f, 0.11f, 0.14f);
    [SerializeField] private Color investigatorColor = new Color(0.27f, 0.60f, 0.85f);
    [SerializeField] private Color ghostColor = new Color(0.85f, 0.85f, 1.00f);
    [SerializeField] private float arrowOffset = 50f;
    [SerializeField] private float arrowSize = 80f;
    // arrowsize is changeable via the inspector anyway

    private int gridSize;
    private Image[,] cellImages;
    private Canvas rootCanvas;
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
    //coroutine for grid gen
    {
        //force the grid layout to fully calculate before we read its corners
        //this is what was causing the slight arrow misalignment previously
        //^this bug was hopeless for me to fix so thank you ClaudeAI for finding a solution

        LayoutRebuilder.ForceRebuildLayoutImmediate(gridContainer);
        yield return null;
        yield return null;
        //2 frames for margin of error purposes
        BuildArrows();
    }

    //visual (6x6 now) grid 
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
        //cant move if no moves left this turn
        if (GameManager.Instance.movesRemaining <= 0)
            return;

        int newRow = GameManager.Instance.ghostRow + dRow;
        int newCol = GameManager.Instance.ghostCol + dCol;

        //both tiles must agree on the connection
        if (!GameManager.Instance.CanMove(
            GameManager.Instance.ghostRow,
            GameManager.Instance.ghostCol,
            newRow, newCol))
            return;
        //bound checker
        //if (newRow < 0 || newRow >= gridSize || newCol < 0 || newCol >= gridSize)
        // return;

        //ghost can pass through walls, this is subject to change, at the very least wall movement parameters / illegal moves must 
        //- be created, and the possibility remains of the removal of the moving thru walls feature entirely .

        GameManager.Instance.ghostRow = newRow;
        GameManager.Instance.ghostCol = newCol;
        GameManager.Instance.movesRemaining--;

        RefreshGrid();
        RefreshHUD();
    }

    //arrow buttons for moving rows and columns 
    void BuildArrows()
    {
        Vector3[] corners = new Vector3[4];
        gridContainer.GetWorldCorners(corners);

        //corners: 0=bottom-left, 1=top-left, 2=top-right, 3=bottom-right

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

            SpawnArrow("→", "row", i, -1, new Vector2(bottomLeft.x - arrowOffset, cellCentreY));
            SpawnArrow("←", "row", i, 1, new Vector2(topRight.x + arrowOffset, cellCentreY));
            SpawnArrow("↓", "col", i, -1, new Vector2(cellCentreX, topLeft.y + arrowOffset));
            SpawnArrow("↑", "col", i, 1, new Vector2(cellCentreX, bottomLeft.y - arrowOffset));
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

        //just hotfix by making it rotate the child and not the canvas itself
        Image arrowImage = btn.GetComponentInChildren<Image>();
        if (arrowImage != null)
        {
            if (label == "←") arrowImage.rectTransform.localEulerAngles = new Vector3(0, 0, 180f);
            if (label == "↑") arrowImage.rectTransform.localEulerAngles = new Vector3(0, 0, 90f);
            if (label == "↓") arrowImage.rectTransform.localEulerAngles = new Vector3(0, 0, 270f);
        }

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

    //loads all tile sprites from Resources/Tiles/ by matching enum names
    void LoadTileSprites()
    {
        foreach (CellType type in System.Enum.GetValues(typeof(CellType)))
        {
            Sprite s = Resources.Load<Sprite>($"Tiles/{type}");
            if (s != null)
                tileSprites[type] = s;
            else
                Debug.LogWarning($"missing sprite for tile: {type}");
        }
    }
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
    void RefreshGrid()
    //this does what it says, simply updates the grid each time it needs to do so
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
                if (r == ghRow && c == ghCol)
                    cellImages[r, c].color = ghostColor;
                else if (r == invRow && c == invCol)
                    cellImages[r, c].color = investigatorColor;
                //ignore investigator stuff. outdated, but not worth removing right now
                else
                {
                    //closed = wall colour, anything else = passage colour
                    //sprite rendering will replace this later when tiles arrive
                    CellType cell = GameManager.Instance.grid[r, c];
                    cellImages[r, c].color = cell == CellType.Closed ? wallColor : passageColor;
                }
                // cellImages[r, c].color = grid[r, c] == 1 ? wallColor : passageColor;
            }
        }
    }
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
