using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// InvestigatorBoardScreen.cs
/// The Investigator's interactive board screen.
/// The investigator taps D-pad style buttons to move their token one cell at a time.
/// Movement is blocked by wall cells (grid value == 1).
///
/// SCENE NAME: "InvestigatorBoard"
///
/// INSPECTOR SETUP:
/// - gridContainer   → RectTransform holding the 8x8 cell Images
/// - cellPrefab      → Prefab with an Image component
/// - btnUp/Down/Left/Right → Four Button components (your D-pad UI)
/// - endTurnButton   → Button to end the investigator's turn
/// - nextSceneName   → "GhostStart"
/// </summary>
public class InvestigatorBoardScreen : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform gridContainer;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private Button btnUp;
    [SerializeField] private Button btnDown;
    [SerializeField] private Button btnLeft;
    [SerializeField] private Button btnRight;
    [SerializeField] private Button endTurnButton;

    [Header("Scene Navigation")]
    [SerializeField] private string nextSceneName = "GhostStart";

    [Header("Visual Settings")]
    [SerializeField] private Color wallColor = new Color(0.22f, 0.25f, 0.32f);
    [SerializeField] private Color passageColor = new Color(0.10f, 0.11f, 0.14f);
    [SerializeField] private Color investigatorColor = new Color(0.27f, 0.60f, 0.85f);

    private int gridSize;
    private Image[,] cellImages;

    // ─────────────────────────────────────────────
    void Start()
    {
        gridSize = GameManager.Instance.gridSize;
        cellImages = new Image[gridSize, gridSize];

        BuildGrid();
        RefreshGrid();

        btnUp.onClick.AddListener(() => TryMove(-1, 0));
        btnDown.onClick.AddListener(() => TryMove(1, 0));
        btnLeft.onClick.AddListener(() => TryMove(0, -1));
        btnRight.onClick.AddListener(() => TryMove(0, 1));
        endTurnButton.onClick.AddListener(OnEndTurn);
    }

    // ── Build visual grid ─────────────────────────────────────────────────────
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

    // ── Movement ──────────────────────────────────────────────────────────────
    void TryMove(int dRow, int dCol)
    {
        int newRow = GameManager.Instance.investigatorRow + dRow;
        int newCol = GameManager.Instance.investigatorCol + dCol;

        // Bounds check
        if (newRow < 0 || newRow >= gridSize || newCol < 0 || newCol >= gridSize)
            return;

        // Wall check — blocked if target cell is a wall
        if (GameManager.Instance.grid[newRow, newCol] == 1)
            return;

        GameManager.Instance.investigatorRow = newRow;
        GameManager.Instance.investigatorCol = newCol;
        RefreshGrid();
    }

    // ── Redraw cells ──────────────────────────────────────────────────────────
    void RefreshGrid()
    {
        int[,] grid = GameManager.Instance.grid;
        int invRow = GameManager.Instance.investigatorRow;
        int invCol = GameManager.Instance.investigatorCol;

        for (int r = 0; r < gridSize; r++)
            for (int c = 0; c < gridSize; c++)
            {
                if (r == invRow && c == invCol)
                    cellImages[r, c].color = investigatorColor;
                else
                    cellImages[r, c].color = grid[r, c] == 1 ? wallColor : passageColor;
            }
    }

    // ── End turn ──────────────────────────────────────────────────────────────
    void OnEndTurn()
    {
        endTurnButton.interactable = false;
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        endTurnButton.onClick.RemoveAllListeners();
        btnUp.onClick.RemoveAllListeners();
        btnDown.onClick.RemoveAllListeners();
        btnLeft.onClick.RemoveAllListeners();
        btnRight.onClick.RemoveAllListeners();
    }
}
