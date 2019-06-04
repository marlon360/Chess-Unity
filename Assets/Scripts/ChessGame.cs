using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChessGame : MonoBehaviour {

    [Header ("Piece Objects")]
    public PieceObject PawnObject;
    public PieceObject RookObject;
    public PieceObject KnightObject;
    public PieceObject BishopObject;
    public PieceObject QueenObject;
    public PieceObject KingObject;
    [Header ("Materials")]
    public Material WhiteMaterial;
    public Material BlackMaterial;
    [Header ("Tiles")]
    public Tile WhiteTile;
    public Tile BlackTile;

    [Header ("UI")]
    public GameObject GameOverCanvas;
    private Text gameOverText;

    private Chess chess;
    private Tile[, ] tiles = new Tile[8, 8];
    private PieceObject[, ] pieces = new PieceObject[8, 8];

    private float tileHeight;

    private GameObject PieceParent;

    public Subject<Team> OnTeamChanged = new Subject<Team>();
    public Subject<Piece> OnKilled = new Subject<Piece>();
    public Subject<Team> OnGameOver = new Subject<Team>();

    private void Awake() {
        gameOverText = GameOverCanvas.GetComponentInChildren<Text> ();
        GameOverCanvas.SetActive (false);
        chess = new Chess ();
        chess.StartFormation ();
        SetUpGrid ();
        RenderState ();
    }

    // Start is called before the first frame update
    void Start () {
        OnTeamChanged.Notify(chess.currentTeam);
    }

    public void ShowGameOver (Team winner) {
        GameOverCanvas.SetActive (true);
        gameOverText.text = winner + " wins!";
        OnGameOver.Notify(winner);
        //StartCoroutine(ResetAfterDelay(3f));
    }

    [ContextMenu ("Debug State")]
    public void DebugState () {
        string DebugText = "";
        for (int y = 0; y < 8; y++) {
            for (int x = 0; x < 8; x++) {
                if (GetPiece (x, y) != null) {
                    DebugText += GetPiece (x, y);
                } else {
                    DebugText += "        ";
                }
            }
            DebugText += "\n";
        }
        Debug.Log (DebugText);
    }

    [ContextMenu ("Render")]
    public void RenderState () {
        if (chess.NoMovesPossible()) {
            ShowGameOver (chess.currentTeam == Team.White ? Team.Black : Team.White);
        }
        pieces = new PieceObject[8, 8];
        if (PieceParent != null) {
            Destroy (PieceParent);
        }
        PieceParent = new GameObject();
        PieceParent.transform.parent = transform;
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                Piece piece = chess.GetPiece (x, y);
                if (piece != null) {
                    if (piece is Pawn) {
                        InstantiatePiece (PawnObject, x, y, piece.team);
                    } else if (piece is Rook) {
                        InstantiatePiece (RookObject, x, y, piece.team);
                    } else if (piece is Knight) {
                        InstantiatePiece (KnightObject, x, y, piece.team);
                    } else if (piece is Bishop) {
                        InstantiatePiece (BishopObject, x, y, piece.team);
                    } else if (piece is King) {
                        InstantiatePiece (KingObject, x, y, piece.team);
                    } else if (piece is Queen) {
                        InstantiatePiece (QueenObject, x, y, piece.team);
                    }
                }
            }
        }
    }

    public void SetUpGrid () {
        for (int x = 0; x < 8; x++) {
            for (int y = 0; y < 8; y++) {
                GameObject tileObj;
                if ((y % 2 == 1 && x % 2 == 0) || (y % 2 == 0 && x % 2 == 1)) {
                    tileObj = WhiteTile.gameObject;
                } else {
                    tileObj = BlackTile.gameObject;
                }
                GameObject go = GameObject.Instantiate (tileObj, new Vector3 (x, 0, y), Quaternion.identity, transform);
                Tile tile = go.GetComponent<Tile> ();
                tile.position = new Vector2Int (x, y);
                tiles[x, y] = tile;
                tileHeight = go.GetComponent<BoxCollider> ().bounds.max.y;
            }
        }
    }

    private void InstantiatePiece (PieceObject pieceObject, int x, int y, Team team) {
        GameObject go = GameObject.Instantiate (pieceObject.gameObject, Vector3.zero, Quaternion.identity, PieceParent.transform);
        go.name += " (" + team + ")";
        //float pieceHeight = go.GetComponent<BoxCollider>().bounds.max.y;
        go.transform.position = new Vector3 (x, tileHeight, y);
        go.GetComponent<PieceObject> ().position = new Vector2Int (x, y);
        pieces[x, y] = go.GetComponent<PieceObject> ();
        go.GetComponentInChildren<Renderer> ().material = team == Team.White ? WhiteMaterial : BlackMaterial;
    }

    public void MakeMove (Vector2Int start, Vector2Int end) {
        Piece startPiece = GetPiece (start);
        if (startPiece != null && chess.IsMoveValid (start, end)) {
            AnimateMakeMove (start, end);
        }
    }

    public void MakeMove (Move move) {
        MakeMove (move.start, move.end);
    }

    public void AnimateMakeMove (Vector2Int start, Vector2Int end) {
        DeselectTiles ();
        pieces[start.x, start.y].MoveTo (end, () => {
            PieceObject endPiece = pieces[end.x, end.y];
            if (endPiece) {
                Kill (endPiece);
            }
            pieces[start.x, start.y].DeselectPiece (() => {
                chess.MakeMove (start, end);
                RenderState ();
                OnTeamChanged.Notify(chess.currentTeam);
            });
        });

    }

    private void Kill (PieceObject pieceObject) {
        Piece piece = GetPiece (pieceObject.position);
        OnKilled.Notify(piece);
        if (piece is King) {
            ShowGameOver (chess.currentTeam);
        }
        pieceObject.Kill (() => {
            Destroy (pieceObject);
        });
    }

    [ContextMenu ("Undo")]
    public void Undo () {
        chess.Undo ();
        RenderState ();
    }

    public Piece GetPiece (Vector2Int pos) {
        return chess.GetPiece (pos);
    }
    public Piece GetPiece (int x, int y) {
        return chess.GetPiece (x, y);
    }

    public Chess GetChess () {
        return chess;
    }

    public void SelectPiece (PieceObject pieceObject) {
        pieceObject.SelectPiece ();
        Piece piece = GetPiece (pieceObject.position);
        SelectTiles (piece.GetValidDestinations (chess.state));
    }

    public void SelectPiece (Piece piece) {
        PieceObject pieceObject = pieces[piece.position.x, piece.position.y];
        pieceObject.SelectPiece ();
        SelectTiles (piece.GetValidDestinations (GetChess ().state));
    }

    public void DeselectPiece (PieceObject pieceObject) {
        pieceObject.DeselectPiece ();
        DeselectTiles ();
    }

    public void SelectTiles (Vector2Int[] positions) {
        foreach (Vector2Int pos in positions) {
            if (chess.GetPiece (pos) != null) {
                tiles[pos.x, pos.y].SelectAttackAtTile ();
            } else {
                tiles[pos.x, pos.y].SelectMoveToTile ();
            }
        }
    }
    public void DeselectTiles () {
        foreach (Tile tile in tiles) {
            tile.DeselectTile ();
        }
    }

    private IEnumerator ResetAfterDelay(float delay) {
        yield return new WaitForSeconds(delay);
        Reset();
    }

    public void Reset () {
        GameOverCanvas.SetActive (false);
        chess = new Chess ();
        chess.StartFormation ();
        RenderState ();
        OnTeamChanged.Notify(chess.currentTeam);
    }

}