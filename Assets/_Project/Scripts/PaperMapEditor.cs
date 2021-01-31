using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using System.Linq;
using Rewired;
using UnityEngine.UI;

public class PaperMapEditor : MonoBehaviour
{
    public bool watching_map = false;
    public bool show_player = true;

    public GameObject canvasObject = null;
    public Camera playerCamera;
    public Camera mapCamera;
    public RenderTexture rt = null;
    public RenderTexture final_rt = null;
    //public RenderTexture bgmap_rt = null;
    public Texture atlas = null; //atlas
    public Texture map_bg = null; //bg
    public Vector2 last_pos;
    public List<Vector3> path;
    public float world_scale = 4.0f;

    public Rect visible_rect; //from 0,0 to map_bg.width,map_bg.height

    //public List<Texture> circuit_textures;
    //public List<Sprite> blueprint_textures;

    public Vector3 hitpos;
    public Vector3 localpos;
    public Vector3 cursorpos;

    //private int levelID = 0;
    //public RectTransform panelEdit;
    //public RectTransform panelControl;
    private bool pencil_drawing = false;

    Plane plane;

    void OnEnable()
    {
        RenderPipelineManager.endCameraRendering += RenderPipelineManager_endCameraRendering;
    }
    void OnDisable()
    {
        RenderPipelineManager.endCameraRendering -= RenderPipelineManager_endCameraRendering;
    }
    private void RenderPipelineManager_endCameraRendering(ScriptableRenderContext context, Camera camera)
    {
        OnPostRender();
    }

    private void Start()
    {
        RenderTexture.active = rt;
        GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        RenderTexture.active = null;
        visible_rect.Set(0, 0, map_bg.width, map_bg.height);
        //visible_rect.Set(0, 0, 256f, 256f);
        //visible_rect.Set(0, 64f, 256f, 256f);
        initMap();
        //FocusOnWorldPos(new Vector3(100, 0, 100),128f);
    }

    void initMap()
    {
        GameManager.Instance.paperMapEditor = this;
        //levelID = GameManager.Instance.level;
        //mapMR.material.SetTexture( "_BaseMap", circuit_textures[ levelID ] );
        //blueprintImg.sprite = blueprint_textures[ levelID ];
    }

    void OnDrawGizmos()
    {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        //Vector3 hitpos = canvasObject.transform.InverseTransformPoint(cursorpos);
        Gizmos.DrawSphere(hitpos, 0.05f);
        for (int i = 0; i < path.Count; ++i)
        {
            Gizmos.DrawSphere(path[i], 1.0f);
           // Debug.Log($"worldops _x:{path[i].x}_z:{path[i].z}");
        }
    }

    public void FocusOnWorldPos(Vector3 worldpos, float area = 128f)
    {
        //return;
        Vector2 center = convertWorldToMap(worldpos, true);
        visible_rect.x = center.x - area * 0.5f;
        visible_rect.y = center.y - area * 0.5f;
        visible_rect.width = area;
        visible_rect.height = area;
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    //converts from worldcoords to papermap coords
    public Vector2 convertWorldToMap(Vector3 worldpos, bool skip_rect = false)
    {
        float world_width = 128.0f * world_scale;
        float world_height = 128.0f * world_scale;
        Vector2 pos2D = new Vector2( Remap(worldpos.x, 0, world_width, 0, map_bg.width ),
                                     Remap(worldpos.z, 0, world_height, map_bg.height, 0));

        if (!skip_rect)
        {
            pos2D = new Vector2(Remap(pos2D.x, visible_rect.x, visible_rect.x + visible_rect.width, 0, final_rt.width),
                Remap(pos2D.y, visible_rect.y, visible_rect.y + visible_rect.height, 0, final_rt.height));
        }

        //pos2D = new Vector2(Remap(pos2D.x, visible_rect.x, visible_rect.x + visible_rect.width, 0.0f, rt.width),
        //            Remap(pos2D.y, visible_rect.y, visible_rect.y + visible_rect.height, 0.0f, rt.height));

        return pos2D;
    }

    //converts from papermap coords to worldcoords
    public Vector3 convertMapToWorld(Vector2 pos2D, bool skip_rect = false)
    {
        if (!skip_rect)
        {
            pos2D = new Vector2(Remap(pos2D.x, 0.0f, final_rt.width, visible_rect.x, visible_rect.x + visible_rect.width),
                                Remap(pos2D.y, 0.0f, final_rt.height, visible_rect.y, visible_rect.y + visible_rect.height));
            //            pos2D = new Vector2(Remap(pos2D.x, 0.0f, rt.width, visible_rect.x, visible_rect.x + visible_rect.width),
            //                            Remap(pos2D.y, 0.0f, rt.height, visible_rect.y, visible_rect.y + visible_rect.height));
        }

        float world_width = 128.0f * world_scale;
        float world_height = 128.0f * world_scale;
        Vector3 world_pos = new Vector3( Remap(pos2D.x, 0, map_bg.width, 0, world_width), 0.0f, Remap(pos2D.y, map_bg.height, 0, 0, world_height) );
        return world_pos;
    }

    public void OpenMap(Vector3 position)
    {
        if (watching_map) return;
        watching_map = true;
        FocusOnWorldPos(position);
    }
    
    public void CloseMap()
    {
        if (watching_map == false) return;
        watching_map = false;

        if (pencil_drawing)
            onLastPoint();
    }

    public void onLastPoint()
    {
        if (!pencil_drawing)
            return;

        Debug.Log("stop drawing... ");
        pencil_drawing = false;
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
        RenderTexture.active = rt;
        DrawSprite(0, 3, cursorpos, new Vector2(32, 32));
        RenderTexture.active = null;
        GL.PopMatrix();
        finishDrawing();
    }

    void LateUpdate()
    {
        if (!mapCamera || !rt || !final_rt)
            return;

        if (ReInput.players.GetPlayer(0).GetButtonDown("Map"))
            watching_map = !watching_map;

        mapCamera.enabled = watching_map;
        playerCamera.enabled = !watching_map;

        if (!watching_map) return;

        //UpdateBoard();

        plane = new Plane(canvasObject.transform.up, canvasObject.transform.position);
        Ray ray = mapCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        //Debug.DrawRay(canvasObject.transform.position, canvasObject.transform.up, Color.red);
        float enter = 0.0f;
        if (watching_map && plane.Raycast(ray, out enter) )
        {
            hitpos = ray.GetPoint(enter);
            localpos = canvasObject.transform.InverseTransformPoint(hitpos);
            cursorpos.x = (-localpos.x / 10.0f + 0.5f) * rt.width;
            cursorpos.y = (localpos.z / 10.0f + 0.5f) * rt.height;

            if (Input.GetMouseButtonDown(0) && 
                cursorpos.x >= 0.0 && cursorpos.x <= rt.width && //is inside map
                cursorpos.y >= 0.0 && cursorpos.y <= rt.height )
            {
                ClearPath();
                Debug.Log("start drawing... ");
                GameManager mng = GameManager.Instance;
                //last_pos = cursorpos;
                last_pos = convertWorldToMap(mng.start_pos);
                path.Add(this.convertMapToWorld(last_pos));
                pencil_drawing = true;
                GL.PushMatrix();
                GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
                RenderTexture.active = rt;
                DrawSprite(0, 1, last_pos, new Vector2(32, 32));
                RenderTexture.active = null;
                GL.PopMatrix();
            }
        }

        if (Input.GetMouseButtonUp(0) && pencil_drawing)
        {
            onLastPoint();
        }

        DrawPaperMapTexture();
    }

    void finishDrawing()
    {
        GameManager mng = GameManager.Instance;
        mng.SetFlockWaypoints(path);
    }

    void ClearPath()
    {
        RenderTexture.active = rt;
        GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        RenderTexture.active = null;
        path.Clear();
    }

    void DrawPaperMapTexture()
    {
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, rt.width, rt.height, 0);
        //GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 0.3f));
        //Graphics.DrawTexture(new Rect(cursorpos.x,cursorpos.y,50,50), t);
        //DrawSprite(0, 1, new Vector2(rt.width*0.5f, rt.height * 0.5f), new Vector2(128, 128));
        if (pencil_drawing)
        {
            float min_dist_between_points = 12.0f;
            float dist = Vector2.Distance(last_pos, cursorpos);
            if(dist > min_dist_between_points)
            {
                RenderTexture.active = rt;
                Vector2 start_pos = last_pos;
                Vector2 end_pos = new Vector2(cursorpos.x, cursorpos.y);
                Vector2 delta = end_pos - start_pos;
                delta.Normalize();
                delta *= min_dist_between_points;

                while (dist > min_dist_between_points)
                {
                    Vector2 point_pos = last_pos + delta;
                    last_pos = point_pos;
                    path.Add(this.convertMapToWorld(point_pos));
                    DrawSprite(0, 1, point_pos, new Vector2(16, 16));
                    dist = Vector2.Distance(last_pos, cursorpos);
                }
                RenderTexture.active = null;
            }

        }

        //compose final image
        RenderTexture.active = final_rt;
        GL.Clear(true, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));

        //draw map
        //Graphics.DrawTexture(new Rect(0,0, final_rt.width, final_rt.height), map_bg );
        //Rect norm_rect = new Rect(visible_rect.x / map_bg.width, 1.0f - (visible_rect.y - visible_rect.height) / map_bg.height, visible_rect.width / map_bg.width, visible_rect.height / map_bg.height);
        Rect norm_rect = new Rect(visible_rect.x / map_bg.width, visible_rect.y / map_bg.height, visible_rect.width / map_bg.width, visible_rect.height / map_bg.height);
        //Debug.Log($"_x:{norm_rect.x}_z:{norm_rect.y}_w:{norm_rect.width}_h:{norm_rect.height}");
        //Rect norm_rect = new Rect(0, 0.0f, 0.5f, 0.5f);
        Graphics.DrawTexture(new Rect(0, 0, final_rt.width, final_rt.height), map_bg, norm_rect, 0,0,0,0);

        //DrawSprite(0, 1, convertWorldToMap(new Vector2(visible_rect.x,visible_rect.y) ), new Vector2(32, 32));

        if (show_player)
        {
            GameManager mng = GameManager.Instance;
            DrawSprite(3, 2, convertWorldToMap(mng.start_pos), new Vector2(32, 32));
            DrawSprite(0, 2, convertWorldToMap(canvasObject.transform.position), new Vector2(32, 32));
        }
        Graphics.DrawTexture(new Rect(0, 0, final_rt.width, final_rt.height), rt);
        RenderTexture.active = null;

        GL.PopMatrix();
    }
    
    //draws icon from texture atlas
    void DrawSprite(int numx, int numy, Vector2 pos, Vector2 size, bool center = true)
    {
        
        if (atlas == null)
            return;
        float w = 512.0f;
        float h = 512.0f;
        float framew = 128.0f;
        float frameh = 128.0f;
        float rows = w / frameh;
        //Rect frame = new Rect( (512.0f - numx*50.0f) / 512.0f, numy*50.0f / 512.0f, 50.0f / 512.0f, 50.0f / 512.0f);
        float x = (numx * framew) / atlas.width;
        //float y = (h - numy * frameh) / h - frameh;
        float y = (rows - numy - 1) * (frameh / atlas.height);
        Rect frame = new Rect(x, y, framew / atlas.width, frameh / atlas.height);
        //Rect frame = new Rect(0.0f,0.0f,1.0f,1.0f);
        Rect screenrect = new Rect(pos.x - (center ? size.x * 0.5f : 0), pos.y - (center ? size.y*0.5f : 0), size.x, size.y);
        Graphics.DrawTexture(screenrect, atlas, frame, 0, 0, 0, 0);
        
        //Graphics.DrawTexture(new Rect(0, 0, 512.0f, 512.0f), t);
        //Debug.Log("draw sprite ");
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
            DrawSprite(5 + color, 1, new Vector2(Mathf.Floor(x), Mathf.Floor(y)),new Vector2(50,50) );
            x = x + vx;
            y = y + vy;
        }
    }

    void OnPostRender()
    {
        return;
        //Debug.Log("draw RT... ");
        GL.PushMatrix();
        GL.LoadOrtho();

        Graphics.DrawTexture(new Rect(0, 0, 1,1), rt);
        //new Rect(rect.x / Screen.width, rect.y / Screen.height, (rect.x + rect.width) / Screen.width, (rect.y + rect.height) / Screen.height)
        //Rect rect = new Rect(0, 0, Screen.width, Screen.height);
        //Graphics.DrawTexture(rect, rt, rect, 0, 0, 0, 0);
        GL.PopMatrix();
    }
}
