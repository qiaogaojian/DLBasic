using UnityEngine;
using DLBasic.Common;

// layermask参数设置的一些总结：
//  1 << 10 打开第10的层。
//  ~(1 << 10) 打开除了第10之外的层。
//  ~(1 << 0) 打开所有的层。
//  (1 << 10) | (1 << 8) 打开第10和第8的层。

public class RayTool : MonoSingletion<RayTool>
{

    /// <summary> 
    /// 相机发射射线照射某层
    /// </summary>
    /// <param name="startPos">起点</param>
    /// <param name="endPos">终点</param>
    /// <param name="layermask">打开或者关闭的层</param>
    /// <param name="maxDistance">射线距离</param>
    /// <returns></returns>
    public RaycastHit CameraRayHasLayerMask(Camera mainCmaera, Vector3 endPos, int layermask = 0, float maxDistance = 100)
    {
        Ray ray = mainCmaera.ScreenPointToRay(endPos);
        if (layermask == 0) layermask = ~(1 << 0);
        Physics.Raycast(ray, out RaycastHit hit, maxDistance, layermask);
#if UNITY_EDITOR
        if (hit.collider != null) Debug.DrawLine(mainCmaera.transform.position, hit.collider.transform.position, Color.black, 0.5f);
        else
        {
            Debug.DrawLine(mainCmaera.transform.position, endPos, Color.yellow, 0.5f);
            Debug.Log("CameraRayHasLayerMask  Collider = Null");
        }
#endif
        return hit;
    }

    /// <summary>
    /// 从某物体发射射线
    /// </summary>
    /// <param name="startPos">射线起始点</param>
    /// <param name="dir">射线方向（若是本地坐标，记得修改Unity（Local）属性）</param>
    /// <param name="layermask">打开或者关闭的层</param>
    /// <param name="maxDistance">射线距离</param>
    /// <returns></returns>
    public RaycastHit RayFormGoToDir(Vector3 startPos, Vector3 dir, int layermask = 0, float maxDistance = 100)
    {
        Ray ray = new Ray(startPos, dir);
        if (layermask == 0) layermask = ~(1 << 0);
        Physics.Raycast(ray, out RaycastHit hit, maxDistance, layermask);
#if UNITY_EDITOR
        if (hit.collider != null) Debug.DrawLine(startPos, hit.collider.transform.position, Color.yellow, 0.5f);
        else
        {
            Debug.DrawRay(startPos, dir, Color.black, 0.5f);
            Debug.Log("RayFormGoToDir  Collider = Null");
        }
#endif
        return hit;
    }

    public bool RayFormGoToDirRB(Vector3 startPos, Vector3 dir, int layermask = 0, float maxDistance = 100)
    {
        Ray ray = new Ray(startPos, dir);
        if (layermask == 0) layermask = ~(1 << 0);
        Physics.Raycast(ray, out RaycastHit hit, maxDistance, layermask);

        if (hit.collider != null)
        {
            //XDebug.Log(string.Format("射线检测目标{0}", hit.collider.name));
            //Debug.DrawLine(startPos, hit.collider.transform.position, Color.yellow, 0.5f);
            return true;
        }
        else
        {
            Debug.DrawRay(startPos, dir, Color.black, 0.5f);
            //XDebug.Log("RayFormGoToDir  Collider = Null");
        }

        return false;
    }

    public bool CameraRayHasLayerMask(Camera mainCmaera, Vector3 endPos, int layermask = 0)
    {
        Ray ray = Camera.main.ScreenPointToRay(endPos);
        if (layermask == 0) layermask = ~(1 << 0);
        Physics.Raycast(ray, out RaycastHit hit, 100, layermask);

        if (hit.collider != null)
        {
            //Debug.DrawLine(mainCmaera.transform.position, hit.collider.transform.position, Color.black, 0.5f);
            return true;
        }
        else
        {
            //Debug.Log("CameraRayHasLayerMask  Collider = Null");
            return false;
        }
    }
}
