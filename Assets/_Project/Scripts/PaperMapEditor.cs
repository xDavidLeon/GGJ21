using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using UnityEngine.UI;

public class PaperMapEditor : MonoBehaviour
{
    public GameObject canvasObject = null;
    public Camera cam;
    public RenderTexture rt = null;
    public Texture t = null; //atlas

    public MeshRenderer mapMR = null;
    public Image mapBgImg = null;
    //public List<Texture> circuit_textures;
    //public List<Sprite> blueprint_textures;

    public Vector2 cursorpos;

    public Transform pointOfInterest;

    //private int levelID = 0;
    public RectTransform panelEdit;
    public RectTransform panelControl;
    private bool pencil_drawing = false;

    Plane plane;

    void Awake()
    {
    }

    private void Start()
    {
        initMap();
    }

    void initMap()
    {
        //levelID = GameManager.Instance.level;
        //mapMR.material.SetTexture( "_BaseMap", circuit_textures[ levelID ] );
        //blueprintImg.sprite = blueprint_textures[ levelID ];
    }

    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(cursorpos, 0.01f);
    }

    void Update()
    {
        if (!cam)
            return;

        //UpdateBoard();

        plane = new Plane(canvasObject.transform.forward, canvasObject.transform.position);
        Ray ray = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        float enter = 0.0f;
        if (  plane.Raycast(ray, out enter) )
        {
            Vector2 hitpos = ray.GetPoint(enter);
            cursorpos = canvasObject.transform.InverseTransformPoint(hitpos);
            cursorpos.x = cursorpos.x * 512 + 256; //offset
            cursorpos.y = cursorpos.y * -256 + 128;

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("start drawing... ");
                pencil_drawing = true;
            }

            if (Input.GetMouseButtonUp(0) && pencil_drawing )
            {
                Debug.Log("stop drawing... ");
                pencil_drawing = false;
            }
        }

        DrawTexture();
    }

    //draws icon from texture atlas
    void DrawIcon(int numx, int numy, Vector2 pos, bool center = true)
    {
        //Rect frame = new Rect( (512.0f - numx*50.0f) / 512.0f, numy*50.0f / 512.0f, 50.0f / 512.0f, 50.0f / 512.0f);
        float s = 50.0f / 512.0f;
        float x = (numx * 50.0f) / 512.0f;
        float y = (512.0f - numy * 50.0f) / 512.0f - s;
        Rect frame = new Rect(x, y, s, s);
        Graphics.DrawTexture(new Rect(pos.x - (center ? 25 : 0), pos.y - (center ? 25 : 0), 50, 50), t, frame, 0, 0, 0, 0);
    }

    float sgn(float v) { return v < 0.0f ? -1.0f : 1.0f; }

    void DrawLine(int x1, int y1, int x2, int y2, int color = 0)
    {
        float d, x, y;
        float dx = (x2 - x1);
        float dy = (y2 - y1);
        if (Mathf.Abs(dx) >= Mathf.Abs(dy))
            d = Mathf.Abs(dx);
        else
            d = Mathf.Abs(dy);
        float vx = dx / d;
        float vy = dy / d;
        x = x1 + sgn(x1) * 0.5f;
        y = y1 + sgn(y1) * 0.5f;
        for (int i = 0; i <= d; i++)
        {
            DrawIcon(5 + color, 1, new Vector2(Mathf.Floor(x), Mathf.Floor(y)));
            x = x + vx;
            y = y + vy;
        }
    }

    void DrawTexture()
    {
        RenderTexture.active = rt;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, 512, 256, 0);
        GL.Clear(true, true, new Color(1.0f, 1.0f, 1.0f, 0.0f));
        //Graphics.DrawTexture(new Rect(cursorpos.x,cursorpos.y,50,50), t);
        //DrawIcon(0,0,cursorpos);

        DrawIcon(4, 1, new Vector2(512, 256), true);

        //draw stuff...

        GL.PopMatrix();
        RenderTexture.active = null;
        //Graphics.DrawTexture(rect, m_renderTexture, new Rect(rect.x / Screen.width, rect.y / Screen.height, (rect.x + rect.width) / Screen.width, (rect.y + rect.height) / Screen.height), 0, 0, 0, 0);
    }

    public void OpenCover()
    {
        //circuitCover.transform.DOLocalMoveX(-0.3f, 1.0f).SetUpdate(UpdateType.Normal, true);
    }

    public void CloseCover()
    {
        //circuitCover.transform.DOLocalMoveX(0.0f, 1.0f).SetUpdate(UpdateType.Normal, true);
    }

    public void ExitEditMode()
    {
        //GameManager.Instance.SetGameStateControl();
    }

    public void EnterEditMode()
    {
        //GameManager.Instance.SetGameStateEdit();
    }

    public void Retry()
    {
        //GameManager.Instance.Restart();
    }
}
