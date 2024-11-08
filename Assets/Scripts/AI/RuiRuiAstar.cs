using RuiRuiAstar;
using RuiRuiMathTool;
using RuiRuiSTL;
using RuiRuiDebug;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


namespace RuiRuiAstar
{
    public static class Astar
    {
        private static IValueOperate valueOperate;
        public static void Initialize(Range2Int xRange, Range2Int yRange, int obstacleLayer, IValueOperate valueOperate = null)
        {
            xRange = new(xRange);
            yRange = new(yRange);
            Astar.xRange = xRange;
            Astar.yRange = yRange;
            if (valueOperate == null) Astar.valueOperate = new DefaultAstarValueOperate();
            else Astar.valueOperate = valueOperate;
            Astar.obstacleLayer = obstacleLayer;
            OperateObstacles(xRange, yRange);
        }
        public static bool MoveTo(GameObject mover, Vector2 pos, float speed)
        {

            return true;
        }


        public static SerializabledCenter2D<AstarNode> grids;
        private static SerializabledCenter2D<bool> openBools;
        private static SerializabledCenter2D<bool> closeBools;


        static List<AstarNode> open = new();
        static List<AstarNode> close = new();
        static Vector2Int start, target;
        static Range2Int xRange, yRange;
        static int obstacleLayer;
        public static List<Vector2Int> OperationPath(Vector2Int start, Vector2Int target)
        {

            open = new();
            close = new();
            openBools = new SerializabledCenter2D<bool>(xRange, yRange);
            closeBools = new SerializabledCenter2D<bool>(xRange, yRange);
            Astar.start = start;
            Astar.target = target;
            List<Vector2Int> result = new List<Vector2Int>();

            if (!grids.ContainsPos(start.x, start.y) || !grids.ContainsPos(target.x, target.y))
            {
                Debug.LogError($"OperationPath 传入参数越界");
                return null;
            }
            AstarNode nowNode = grids[start.x, start.y];
            AddOtherPos(nowNode);
            nowNode.gCost = 0;




            if (start.Equals(target))
            {

                result.Add(start);
                return result;
            }
            while (open.Count > 0)
            {
                nowNode = GetBestNode();
                if (nowNode.Equals(grids[target.x, target.y]))
                {
                    return RetracePath(grids[start.x, start.y], nowNode);
                }
                if (open.Count == 0) break;
                close.Add(nowNode);
                AddOtherPos(nowNode);


                //if (Physics2D.Raycast(nowNode.Pos, nowNode.Pos.Direction(target), Vector2Int.Distance(nowNode.Pos, target),obstacleLayer).collider == null)
                if (Physics2D.BoxCast(nowNode.Pos, Vector2.one, 0, nowNode.Pos.Direction(target), Vector2Int.Distance(nowNode.Pos, target), obstacleLayer).collider == null)
                {
                    result = RetracePath(grids[start.x, start.y], nowNode);
                    result.Add(target);
                    return result;
                }

            }
            Debug.LogError($"OperationPath 计算结果无可行路径");
            return result;


        }
        private static List<Vector2Int> RetracePath(AstarNode startNode, AstarNode endNode)
        {
            List<Vector2Int> path = new List<Vector2Int>();
            AstarNode currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode.Pos);
                currentNode = currentNode.Father;
            }
            path.Reverse();
            return path;
        }
        private static void AddOtherPos(AstarNode now)
        {

            //单方向
            List<Vector2Int> list = now.Pos.GetSignalVector2Int();

            int ax, ay;
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (!closeBools[ax, ay] && !openBools[ax, ay] && (!grids[ax, ay].isObstacle || grids[ax, ay].isDestructible))
                    {
                        open.Add(grids[ax, ay]);
                        openBools[ax, ay] = true;
                        grids[ax, ay].Father = now;
                        grids[ax, ay].gCost = now.gCost + 10;
                        grids[ax, ay].hCost = valueOperate.H(grids[ax, ay].Pos, target, obstacleLayer) * 10;
                    }
                }
            }
            //对角方向
            list = now.Pos.GetDiagonalVector2Int();
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (!closeBools[ax, ay] && !openBools[ax, ay] && (!grids[ax, ay].isObstacle || grids[ax, ay].isDestructible))
                    {
                        open.Add(grids[ax, ay]);
                        openBools[ax, ay] = true;
                        grids[ax, ay].Father = now;
                        grids[ax, ay].gCost = now.gCost + 14 + grids[ax, ay].destroyCost;
                        grids[ax, ay].hCost = valueOperate.H(grids[ax, ay].Pos, target, obstacleLayer) * 10;

                    }
                }
            }
            closeBools[now.Pos.x, now.Pos.y] = true;

        }

        private static AstarNode GetBestNode()
        {
            open.Sort((x, y) =>
            {
                return x.Value.CompareTo(y.Value);
            });
            AstarNode result = open[0];
            open.Remove(result);
            return result;

        }

        /// <summary>
        /// 计算障碍物
        /// </summary>
        /// <param name="xRange">检测范围x</param>
        /// <param name="yRange">检测范围y</param>
        /// <param name="obstacleLayer">障碍物图层</param>
        private static void OperateObstacles(Range2Int xRange, Range2Int yRange)
        {
            grids = new SerializabledCenter2D<AstarNode>(xRange, yRange);
            for (int i = xRange.InFactValue().x; i <= xRange.InFactValue().y; i++)
            {
                for (int j = yRange.InFactValue().x; j < yRange.InFactValue().y; j++)
                {
                    grids[i, j].Pos = new Vector2Int(i, j);
                }
            }
            RangeBox2D mid = new RangeBox2D(xRange, yRange);
            CheckObstacles(mid);



        }

        public static void CheckObstacles(RangeBox2D rangeBox2D)
        {
            Collider2D collider2D = Physics2D.OverlapBox(rangeBox2D.centerPos, rangeBox2D.size, 0, obstacleLayer);
            if (collider2D != null)
            {
                List<RangeBox2D> rangeBox2Ds = rangeBox2D.Quartile();
                if (rangeBox2Ds.Count == 0)
                {
                    AstarNode item = grids[rangeBox2D.xMin, rangeBox2D.yMin];
#if UNITY_EDITOR
                    DrawBox((Vector3Int)item.Pos);
#endif
                    item.isObstacle = true;
                    if (collider2D.TryGetComponent(out IDestoey destoey))
                    {
                        item.isDestructible = true;
                        item.destroyCost = destoey.DestoryCost();
                    }

                }
                else
                {
                    foreach (var a in rangeBox2Ds)
                    {
                        CheckObstacles(a);
                    }
                }
            }
            else
            {
                AstarNode item;
                for (int i = rangeBox2D.xMin; i <= rangeBox2D.xMax; i++)
                {
                    for (int j = rangeBox2D.yMin; j <= rangeBox2D.yMax; j++)
                    {
                        item = grids[i, j];
                        item.isObstacle = false;
                        item.isDestructible = false;
                        item.destroyCost = 0;
                        // item.Value = 100000000;
                    }
                }

            }

        }
        //画障碍物位置
        public static void DrawBox(Vector3 pos)
        {
            Debug.DrawLine(pos, pos + Vector3.up * 0.5f, Color.red, 300f);
            Debug.DrawLine(pos, pos + Vector3.down * 0.5f, Color.red, 300f);
            Debug.DrawLine(pos, pos + Vector3.left * 0.5f, Color.red, 300f);
            Debug.DrawLine(pos, pos + Vector3.right * 0.5f, Color.red, 300f);
        }

    }

    public interface IValueOperate
    {
        /// <summary>
        /// 必须实现，启发式函数,表示任意顶点n到目标顶点的估算距离
        /// </summary>
        /// <returns></returns>
        int H(Vector2Int now, Vector2Int target, int obstacleLayer);

    }

    public interface IDestoey
    {
        /// <summary>
        /// 默认一单位的移动代价为10
        /// </summary>
        /// <returns></returns>
        int DestoryCost();
    }

    public interface IAstarMove
    {
        public bool NeedAstarMove { get; set; }
        public bool IsArrival { get; set; }

        public float Timer { get; set; }
        public int UpdateTime { get; set; }

        public Vector2 NowPos => Transform.position;
        public Vector2 TargetPos { get; }

        public List<Vector2Int> path { get; set; }

        public Transform Transform { get; }

        public int Speed { get; set; }

        public bool Move()
        {


            Vector2Int midTarget = path[0];
            float maxDistance = Speed * Time.fixedDeltaTime;
            Transform.position = Vector2.MoveTowards(Transform.position, midTarget, maxDistance);
            if (Vector2.Distance(Transform.position, midTarget) < 0.001f)
            {
                path.RemoveAt(0);
            }
            if (path.Count == 0)
            {
                Debug.Log(Transform.name + "  arrival!");
                IsArrival = true;
            }
            return IsArrival;
        }

        public void UpdatePath()
        {
            Timer = Time.time;
            path = Astar.OperationPath(NowPos.RoundToVector2Int(), TargetPos.RoundToVector2Int());
            if (path.Count < 2)
            {
                Debug.Log("路劲疑似出错");
                return;
            }
#if UNITY_EDITOR
            for (int i = 0; i < path.Count - 1; i++)
            {
                Debug.DrawLine((Vector2)path[i], (Vector2)path[i + 1], Color.green, 5);
            }
#endif

        }

        public void OnEnableNew(int updateTime, int speed)
        {
            IsArrival = false;
            NeedAstarMove = true;
            Timer = Time.time;
            UpdateTime = updateTime;
            Speed = speed;
            UpdatePath();
        }

        public void FixedUpdateNew()
        {
            if (NeedAstarMove && !IsArrival) Move();
            if (!IsArrival && Timer + UpdateTime < Time.time) UpdatePath();

        }


        /// <summary>
        /// 关闭寻路
        /// </summary>
        public void CloseAstarMove()
        => NeedAstarMove = false;
        /// <summary>
        /// 开启寻路
        /// </summary>
        public void ResumeAstarMove()
        => NeedAstarMove = true;



    }


    public class DefaultAstarValueOperate : IValueOperate
    {
        public int H(Vector2Int now, Vector2Int target, int obstacleLayer)
        {

            int value = Mathf.Abs(target.x - now.x) + Mathf.Abs(target.y - now.y);
            //RaycastHit2D hit = Physics2D.Raycast(now, new Vector2Int(target.x - now.x, 0), Mathf.Abs(target.x - now.x), obstacleLayer);
            //if (hit.collider != null)
            //{
            //    if (hit.collider.TryGetComponent(out IDestoey destoey))
            //    {
            //        value += destoey.DestoryCost();
            //    }
            //    else value += 200;
            //}
            //hit = Physics2D.Raycast(new Vector2Int(target.x, now.y), new Vector2Int(0, target.y - now.y), Mathf.Abs(target.y - now.y), obstacleLayer);
            //if (hit.collider != null)
            //{
            //    if (hit.collider.TryGetComponent(out IDestoey destoey))
            //    {
            //        value += destoey.DestoryCost();
            //    }
            //    else value += 200;
            //}
            return value;
        }


    }

    public class AstarNode
    {
        public Vector2Int Pos;
        public Vector2 targetVector;
        public AstarNode Father;
        public int Value => gCost + hCost;
        public int gCost, hCost;
        public bool isObstacle = false;
        public bool isDestructible = false;
        /// <summary>
        /// 默认一单位的移动代价为10
        /// </summary>
        public int destroyCost = 0;
    }



}


namespace RuiRuiMathTool
{

    public static class MathTool
    {
        public static Vector2 Direction(this Vector2Int from, Vector2Int target)
        {
            return (target - from);
        }
        public static Vector2Int RoundToVector2Int(this Vector2 value)
        {
            return new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
        }
        public static Vector2Int RoundToVector2Int(this Vector3 value)
        {
            return new Vector2Int(Mathf.RoundToInt(value.x), Mathf.RoundToInt(value.y));
        }

        public static List<Vector2Int> GetSroundVector2Int(this Vector2Int center)
        {
            return new List<Vector2Int>()
            {
                center + Vector2Int.up,
                center + Vector2Int.down,
                center + Vector2Int.left,
                center + Vector2Int.right,
                center + Vector2Int.up + Vector2Int.left,
                center + Vector2Int.up + Vector2Int.right,
                center + Vector2Int.down + Vector2Int.left,
                center + Vector2Int.down + Vector2Int.right
            };
        }

        public static List<Vector2Int> GetSignalVector2Int(this Vector2Int center)
        {
            return new List<Vector2Int>()
            {
                center + Vector2Int.up,
                center + Vector2Int.down,
                center + Vector2Int.left,
                center + Vector2Int.right,
            };
        }

        public static List<Vector2Int> GetDiagonalVector2Int(this Vector2Int center)
        {
            return new List<Vector2Int>()
            {
                center + Vector2Int.up + Vector2Int.left,
                center + Vector2Int.up + Vector2Int.right,
                center + Vector2Int.down + Vector2Int.left,
                center + Vector2Int.down + Vector2Int.right
            };
        }
    }



}


namespace RuiRuiSTL
{
    [System.Serializable]
    public struct Range2Int
    {
        [HideInInspector]
        public int X;
        [HideInInspector]
        public int Y;
        /// <summary>
        /// int
        /// </summary>
        public int center => (x_ + y_) / 2;

        public int length => X + Y;


        [SerializeField] private int x_;
        [SerializeField] private int y_;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="small">负数或0</param>
        /// <param name="big">正数</param>
        public Range2Int(int small, int big)
        {
            if (big <= 0 || small > 0) Debug.LogError("Range2Int Init Value Error !");
            x_ = small;
            y_ = big;
            X = -x_;
            Y = y_ + 1;
        }

        public Range2Int(Range2Int range2Int)
        {
            if (range2Int.y_ <= 0 || range2Int.x_ > 0) Debug.LogError("Range2Int Init Value Error !");
            x_ = range2Int.x_;
            y_ = range2Int.y_;
            X = -x_;
            Y = y_ + 1;
        }
        public Vector2Int InFactValue()
        {
            return new Vector2Int(x_, y_);
        }
        public bool IsSmallerThan(Range2Int compare)
        {
            if ((compare.x_ == x_ && compare.y_ == y_) || compare.x_ < x_ || compare.y_ > y_) return true;
            return false;
        }
        public override string ToString()
        {
            return "x=" + x_ + "  y=" + y_;

        }
        public bool ValueInRange(int value)
        {
            return x_ <= value && value <= y_;
        }

    }

    public struct RangeBox2D
    {
        public int xMin, xMax;
        public int yMin, yMax;
        public Vector2 centerPos;
        public Vector2Int size;

        public RangeBox2D(Range2Int xRange, Range2Int yRange)
        {
            xMin = xRange.InFactValue().x;
            xMax = xRange.InFactValue().y;
            yMin = yRange.InFactValue().x;
            yMax = yRange.InFactValue().y;
            centerPos = new Vector2((xMin + xMax) / 2f, (yMin + yMax) / 2f);
            size = new Vector2Int(xMax - xMin + 1, yMax - yMin + 1);
        }
        public RangeBox2D(int xMin, int xMax, int yMin, int yMax)
        {
            this.xMin = xMin;
            this.xMax = xMax;
            this.yMin = yMin;
            this.yMax = yMax;

            centerPos = new Vector2((xMin + xMax) / 2f, (yMin + yMax) / 2f);
            size = new Vector2Int(xMax - xMin + 1, yMax - yMin + 1);
        }

        public List<RangeBox2D> Quartile()
        {
            List<RangeBox2D> result = new List<RangeBox2D>();
            int xLength, yLength;
            xLength = size.x;
            yLength = size.y;
            int xCenter, yCenter;
            xCenter = (xMin + xMax) / 2;
            yCenter = (yMin + yMax) / 2;
            if (xLength == 2) xCenter = xMin;
            if (yLength == 2) yCenter = yMin;

            if (xLength > 1 && yLength > 1)
            {
                result.Add(new RangeBox2D(xMin, xCenter, yMin, yCenter));
                result.Add(new RangeBox2D(xCenter + 1, xMax, yCenter + 1, yMax));
                result.Add(new RangeBox2D(xMin, xCenter, yCenter + 1, yMax));
                result.Add(new RangeBox2D(xCenter + 1, xMax, yMin, yCenter));
            }
            else if (xLength > 1 && yLength == 1)
            {
                result.Add(new RangeBox2D(xMin, xCenter, yMin, yMax));
                result.Add(new RangeBox2D(xCenter + 1, xMax, yMin, yMax));
            }
            else if (yLength > 1 && xLength == 1)
            {
                result.Add(new RangeBox2D(xMin, xMax, yMin, yCenter));
                result.Add(new RangeBox2D(xMin, xMax, yCenter + 1, yMax));

            }
            return result;

        }

        public bool Contains(Vector2Int pos)
        {
            return pos.x >= xMin && pos.x <= xMax && pos.y >= yMin && pos.y <= yMax;
        }
    }



    [Serializable]
    public class Serializable2D<T>
    {
        public T[] datas;
        //[JsonProperty]
        private int xSize;
        //[JsonProperty]
        private int ySize;
        public Serializable2D(int x, int y)
        {
            xSize = x;
            ySize = y;
            datas = new T[xSize * ySize];
            for (int i = 0; i < x * y; i++)
            {
                datas[i] = Activator.CreateInstance<T>();
            }
        }
        //[JsonIgnore]
        public T this[int i, int j]
        {

            get
            {
                if (i >= xSize) throw new IndexOutOfRangeException($"x越界 xSize:{xSize} i:{i}");
                if (j >= ySize) throw new IndexOutOfRangeException($"y越界 ySize:{ySize} j:{j}");
                return datas[i + j * xSize];
            }
            set
            {
                if (i >= xSize) throw new IndexOutOfRangeException($"x越界 xSize:{xSize} i:{i}");
                if (j >= ySize) throw new IndexOutOfRangeException($"y越界 ySize:{ySize} j:{j}");
                datas[i + j * xSize] = value;
            }
        }
        public int GetLength(int dimension)
        {
            if (dimension == 0) return xSize;
            else if (dimension == 1) return ySize;
            else throw new ArgumentException("无效的维度");
        }

        /// <summary>
        /// 只能变大
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void ReSize(int x, int y)
        {
            if ((x == xSize && y == ySize) || x < xSize || y < ySize) return;
            T[] midData = new T[x * y];
            for (int i = 0; i < midData.Length; i++)
            {
                midData[i] = default(T);
            }

            for (int i = 0; i < xSize; i++)
            {
                for (int j = 0; j < ySize; j++)
                {
                    midData[i + j * x] = datas[i + j * xSize];
                }
            }
            xSize = x;
            ySize = y;
            datas = midData;

        }

    }

    /// <summary>
    /// 象限
    /// </summary>
    public enum Quadrant
    {
        First = 0, Second, Third, Fourth
    }

    /// <summary>
    /// 这个三位数组会存在原点进行坐标运算扩展,x,y为轴,z可以填其他东西
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [System.Serializable]
    public class SerializabledCenter2D<T>
    {
        //[JsonProperty] 
        private Serializable2D<T> firstData;
        //[JsonProperty] 
        private Serializable2D<T> secondData;
        //[JsonProperty] 
        private Serializable2D<T> thirdData;
        //[JsonProperty] 
        private Serializable2D<T> fourthData;
        //[JsonProperty] 
        private Range2Int xRange;
        //[JsonProperty] 
        private Range2Int yRange;

        private int XSize => xRange.Y + xRange.X;
        private int YSize => yRange.Y + yRange.X;
        public SerializabledCenter2D(Range2Int xRange, Range2Int yRange)
        {
            xRange = new(xRange);
            yRange = new(yRange);
            firstData = new Serializable2D<T>(xRange.Y, yRange.Y);
            secondData = new Serializable2D<T>(xRange.X, yRange.Y);
            thirdData = new Serializable2D<T>(xRange.X, yRange.X);
            fourthData = new Serializable2D<T>(xRange.Y, yRange.X);

            this.xRange = xRange;
            this.yRange = yRange;
        }

        /// <summary>
        /// 返回是否在范围内
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool ContainsPos(int x, int y)
        {
            if ((x >= xRange.Y && x > 0) || (-x > xRange.X && x < 0)) return false;
            if ((y >= yRange.Y && y > 0) || (-y > yRange.X && y < 0)) return false;
            return true;
        }

        public T this[int x, int y]
        {
            get
            {
                if ((x > xRange.Y && x > 0) || (-x > xRange.X && x < 0)) Debug.LogError("X 越界!  XRange:" + xRange + "   X:" + x);
                if ((y > yRange.Y && y > 0) || (-y > yRange.X && y < 0)) Debug.LogError("Y 越界!  YRange:" + yRange + "   Y:" + y);
                bool xLessZero = false;
                bool yLessZero = false;
                if (x < 0)
                {
                    xLessZero = true;
                    x = -x - 1;
                }
                if (y < 0)
                {
                    yLessZero = true;
                    y = -y - 1;
                }
                if (!xLessZero && !yLessZero) return firstData[x, y];//x>=0,y>=0
                if (xLessZero && !yLessZero) return secondData[x, y];//x<0,y>=0
                if (xLessZero && yLessZero) return thirdData[x, y];//x<0,y<0
                return fourthData[x, y];//x>=0,y<0

            }
            set
            {
                if ((x > xRange.Y && x > 0) || (-x > xRange.X && x < 0)) Debug.LogError("X 越界!  XRange:" + xRange + "   X:" + x);
                if ((y > yRange.Y && y > 0) || (-y > yRange.X && y < 0)) Debug.LogError("Y 越界!  YRange:" + yRange + "   Y:" + y);
                bool xLessZero = false;
                bool yLessZero = false;
                if (x < 0)
                {
                    xLessZero = true;
                    x = -x - 1;
                }
                if (y < 0)
                {

                    yLessZero = true;
                    y = -y - 1;
                }

                if (!xLessZero && !yLessZero)//x>=0,y>=0
                {
                    firstData[x, y] = value;
                    return;
                }
                if (xLessZero && !yLessZero)//x<0,y>=0
                {
                    secondData[x, y] = value;
                    return;
                }
                if (xLessZero && yLessZero)//x<0,y<0
                {
                    thirdData[x, y] = value;
                    return;
                }
                //x>=0,y<0
                fourthData[x, y] = value;
            }
        }
        public Vector2Int GetXRange()
        {
            return xRange.InFactValue();
        }
        public Vector2Int GetYRange()
        {
            return yRange.InFactValue();
        }
        public int GetXSize()
        {
            return xRange.Y + xRange.X;
        }
        public int GetYSize()
        {
            return yRange.Y + yRange.X;
        }
        public Vector2 GetSize()
        {
            return new Vector2(XSize, YSize);
        }
        public int GetLength(int dimension)
        {
            if (dimension == 0) return XSize;
            else if (dimension == 1) return YSize;
            else throw new ArgumentException("无效的维度");
        }
        public Vector2 GetCenter()
        {
            return new Vector2((GetXRange().x + GetXRange().y) / 2, (GetYRange().x + GetYRange().y) / 2);
        }
        public void ResizeByQuadrant(Quadrant quadrant, int x, int y)
        {
            Serializable2D<T> target;
            if (quadrant == Quadrant.First) target = firstData;
            else if (quadrant == Quadrant.Second) target = secondData;
            else if (quadrant == Quadrant.Third) target = thirdData;
            else target = fourthData;
            target.ReSize(x, y);
        }
        /// <summary>
        /// 只允许扩大
        /// </summary>
        /// <param name="newXRange"></param>
        public void ResizeX(Range2Int newXRange)
        {
            newXRange = new(newXRange);
            if (newXRange.IsSmallerThan(xRange))
            {
                Debug.LogError("SerializabledCenter3DArry ResizeX Error!   xRange:" + xRange + "  newXRange:" + newXRange);
                return;
            }
            if (newXRange.X > xRange.X)
            {
                ResizeByQuadrant(Quadrant.Second, newXRange.X, yRange.Y);
                ResizeByQuadrant(Quadrant.Third, newXRange.X, yRange.X);
            }
            if (newXRange.Y > xRange.Y)
            {
                ResizeByQuadrant(Quadrant.First, newXRange.Y, yRange.Y);
                ResizeByQuadrant(Quadrant.Fourth, newXRange.Y, yRange.X);
            }
            xRange = newXRange;
        }

        /// <summary>
        /// 只允许扩大
        /// </summary>
        /// <param name="newYRange"></param>
        public void ResizeY(Range2Int newYRange)
        {
            newYRange = new(newYRange);
            if (newYRange.IsSmallerThan(yRange))
            {
                Debug.LogError("SerializabledCenter3DArry ResizeY Error!   yRange:" + yRange + "  newYRange:" + newYRange);
                return;
            }

            if (newYRange.X > yRange.X)
            {
                ResizeByQuadrant(Quadrant.Third, xRange.X, newYRange.X);
                ResizeByQuadrant(Quadrant.Fourth, xRange.Y, newYRange.X);

            }
            if (newYRange.Y > yRange.Y)
            {
                ResizeByQuadrant(Quadrant.First, xRange.Y, newYRange.Y);
                ResizeByQuadrant(Quadrant.Second, xRange.X, newYRange.Y);
            }
            yRange = newYRange;

        }
        /// <summary>
        /// 只允许扩大
        /// </summary>
        /// <param name="newXRange"></param>
        /// <param name="newYRange"></param>
        public void Resize(Range2Int newXRange, Range2Int newYRange)
        {
            newXRange = new(newXRange);
            newYRange = new(newYRange);
            if (!newXRange.Equals(xRange)) ResizeX(newXRange);

            if (!newYRange.Equals(yRange)) ResizeY(newYRange);

        }


        public void ForEach(ForAction predicate)
        {
            for (int i = xRange.InFactValue().x; i <= xRange.InFactValue().y; i++)
            {
                for (int j = yRange.InFactValue().x; j < yRange.InFactValue().y; j++)
                {
                    predicate(this[i, j], i, j);
                }
            }

        }

        public delegate void ForAction(T item, int x, int y);
        public delegate T ForActionValueType(int x, int y);


        public void For(int xMin, int xMax, int yMin, int yMax, ForAction predicate)
        {
            for (int i = xMin; i <= xMax; i++)
            {
                for (int j = yMin; j <= yMax; j++)
                {
                    predicate(this[i, j], i, j);
                }
            }

        }


        public void For(int xMin, int xMax, int yMin, int yMax, ForActionValueType predicate)
        {
            for (int i = xMin; i <= xMax; i++)
            {
                for (int j = yMin; j <= yMax; j++)
                {
                    this[i, j] = predicate(i, j);
                }
            }

        }

    }



}

namespace RuiRuiVectorField
{

    public class VectorNode
    {

        //public int X, Y;

        public Vector2Int Pos;
        public Vector2 TargetVector;

        public float Value = 100000000;

        public bool IsObstacle = false;
        public bool IsDestructible = false;
        /// <summary>
        /// 默认一单位的移动代价为10
        /// </summary>
        public int DestroyCost = 0;
    }
    public static class VectorField
    {

        public static void Initialize(int obstacleLayer, Range2Int xRange, Range2Int yRange)
        {
            xRange = new(xRange);
            yRange = new(yRange);
            VectorField.obstacleLayer = obstacleLayer;
            OperateObstacles(xRange, yRange);
        }


        private static SerializabledCenter2D<VectorNode> grids;
        private static SerializabledCenter2D<bool> openBools;
        private static SerializabledCenter2D<bool> closeBools;

        public static SerializabledCenter2D<VectorNode> GetGrids() => grids;

        public static Queue<VectorNode> Open = new();
        public static List<VectorNode> Close = new();
        static Vector2Int target;
        static int obstacleLayer;

        public static void UpdateVectorFieldTotal(Vector2Int target)
        {
            Open = new();
            Close = new();
            VectorField.target = target;
            if (!grids.ContainsPos(target.x, target.y))
            {
                Debug.LogError($"UpdateVectorField 参数越界!");
            }
            Close.Add(grids[target.x, target.y]);
            grids[target.x, target.y].Value = 1;
            AddOtherPos(grids[target.x, target.y]);

            while (Open.Count > 0)
            {
                VectorNode mid = Open.Dequeue();
                Close.Add(mid);
                AddOtherPos(mid);
            }
            //grids.ForEach((item, x, y) =>
            //{
            //    var list = GetMoveableSroundNode(item);
            //    if (list.Count == 8)
            //    {
            //        foreach (var a in list)
            //        {
            //            if (!a.IsObstacle || a.IsDestructible) item.TargetVector += item.Pos.Direction(a.Pos).normalized * (1 / a.Value);
            //        }
            //    }
            //    else
            //    {
            //        float midMin = 100000000;
            //        foreach (var a in list)
            //        {
            //            if (midMin > a.Value)
            //            {
            //                if (!a.IsObstacle || a.IsDestructible)
            //                {
            //                    item.TargetVector = item.Pos.Direction(a.Pos).normalized * a.Value;
            //                    midMin = a.Value;
            //                }

            //            }

            //        }
            //    }


            //    item.TargetVector = item.TargetVector.normalized;
            //    if (item.TargetVector.Equals(Vector2Int.zero))
            //    {
            //        float midMin = 100000000;
            //        foreach (var a in list)
            //        {
            //            if (midMin > a.Value)
            //            {
            //                if (!a.IsObstacle || a.IsDestructible)
            //                {
            //                    item.TargetVector = item.Pos.Direction(a.Pos).normalized * a.Value;
            //                    midMin = a.Value;
            //                }

            //            }

            //        }
            //        item.TargetVector = item.TargetVector.normalized;
            //    }



            //});


            VectorNode item;
            for (int i = grids.GetXRange().x; i <= grids.GetXRange().y; i++)
            {
                for (int j = grids.GetYRange().x; j < grids.GetYRange().y; j++)
                {
                    item = grids[i, j];

                    var list = GetMoveableSroundNode(item);
                    if (list.Count == 8)
                    {
                        foreach (var a in list)
                        {
                            if (!a.IsObstacle || a.IsDestructible) item.TargetVector += item.Pos.Direction(a.Pos).normalized * (1 / a.Value);
                        }
                    }
                    else
                    {
                        float midMin = 100000000;
                        foreach (var a in list)
                        {
                            if (midMin > a.Value)
                            {
                                if (!a.IsObstacle || a.IsDestructible)
                                {
                                    item.TargetVector = item.Pos.Direction(a.Pos).normalized * a.Value;
                                    midMin = a.Value;
                                }

                            }

                        }
                    }


                    item.TargetVector = item.TargetVector.normalized;
                    if (item.TargetVector.Equals(Vector2Int.zero))
                    {
                        float midMin = 100000000;
                        foreach (var a in list)
                        {
                            if (midMin > a.Value)
                            {
                                if (!a.IsObstacle || a.IsDestructible)
                                {
                                    item.TargetVector = item.Pos.Direction(a.Pos).normalized * a.Value;
                                    midMin = a.Value;
                                }

                            }

                        }
                        item.TargetVector = item.TargetVector.normalized;
                    }


                }
            }


            grids[target.x, target.y].TargetVector = Vector2.zero;
            DarwVectorFieldInScene();

        }

        public static void PartialUpdates(Range2Int xRange, Range2Int yRange)
        {
            xRange = new(xRange);
            yRange = new(yRange);
            RangeBox2D mid = new RangeBox2D(xRange, yRange);

            CheckObstacles(mid);

            Open = new();
            Close = new();
            float minValue = int.MaxValue;
            Vector2Int minPos = grids[mid.xMin, mid.yMin].Pos;
            VectorNode item;
            for (int i = mid.xMin; i <= mid.xMax; i++)
            {
                for (int j = mid.yMin; j <= mid.yMax; j++)
                {
                    item = grids[i, j];
                    if (item.Value < minValue)
                    {
                        minValue = item.Value;
                        minPos = item.Pos;
                    }
                }
            }

            for (int i = mid.xMin; i <= mid.xMax; i++)
            {
                for (int j = mid.yMin; j <= mid.yMax; j++)
                {
                    openBools[i, j] = false;

                }
            }

            for (int i = mid.xMin; i <= mid.xMax; i++)
            {
                for (int j = mid.yMin; j <= mid.yMax; j++)
                {
                    closeBools[i, j] = false;

                }
            }


            Close.Add(grids[minPos.x, minPos.y]);

            AddOtherPos(grids[minPos.x, minPos.y], mid);

            while (Open.Count > 0)
            {
                VectorNode midNode = Open.Dequeue();
                Close.Add(midNode);
                AddOtherPos(midNode);
            }

            for (int i = mid.xMin; i <= mid.xMax; i++)
            {
                for (int j = mid.yMin; j <= mid.yMax; j++)
                {
                    item = grids[i, j];
                    var list = GetMoveableSroundNode(item);
                    if (list.Count == 8)
                    {
                        foreach (var a in list)
                        {
                            if (!a.IsObstacle || a.IsDestructible) item.TargetVector += item.Pos.Direction(a.Pos).normalized * (1 / a.Value);
                        }
                    }
                    else
                    {
                        float midMin = 100000000;
                        foreach (var a in list)
                        {
                            if (midMin > a.Value)
                            {
                                if (!a.IsObstacle || a.IsDestructible)
                                {
                                    item.TargetVector = item.Pos.Direction(a.Pos).normalized * a.Value;
                                    midMin = a.Value;
                                }

                            }

                        }
                    }


                    item.TargetVector = item.TargetVector.normalized;
                    if (item.TargetVector.Equals(Vector2Int.zero))
                    {
                        float midMin = 100000000;
                        foreach (var a in list)
                        {
                            if (midMin > a.Value)
                            {
                                if (!a.IsObstacle || a.IsDestructible)
                                {
                                    item.TargetVector = item.Pos.Direction(a.Pos).normalized * a.Value;
                                    midMin = a.Value;
                                }

                            }

                        }
                        item.TargetVector = item.TargetVector.normalized;
                    }


                }
            }

#if UNITY_EDITOR
            //DarwVectorFieldInScene();
#endif
        }

        public static void DarwVectorFieldInScene()
        {

            VectorNode item;
            for (int i = grids.GetXRange().x; i <= grids.GetXRange().y; i++)
            {
                for (int j = grids.GetYRange().x; j <= grids.GetYRange().y; j++)
                {
                    item = grids[i, j];
                    DebugDraw.DrawArrow((Vector2)item.Pos, item.TargetVector, Color.yellow - new Color(0, item.Value / 50, 0, 0), 10, 0.5f);
                }
            }
        }

        private static void AddOtherPos(VectorNode now)
        {

            //单方向
            List<Vector2Int> list = new List<Vector2Int>();
            list.Add(now.Pos + Vector2Int.up);
            list.Add(now.Pos + Vector2Int.down);
            list.Add(now.Pos + Vector2Int.left);
            list.Add(now.Pos + Vector2Int.right);

            int ax, ay;
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (grids[ax, ay].IsObstacle && !grids[ax, ay].IsDestructible) continue;
                    if (grids[ax, ay].IsDestructible) grids[ax, ay].Value = Mathf.Min(now.Value + 1 + grids[ax, ay].DestroyCost, grids[ax, ay].Value);
                    else grids[ax, ay].Value = Mathf.Min(now.Value + 1, grids[ax, ay].Value);
                    if (!closeBools[ax, ay] && !openBools[ax, ay] && (!grids[ax, ay].IsObstacle || grids[ax, ay].IsDestructible))
                    {
                        Open.Enqueue(grids[ax, ay]);
                        openBools[ax, ay] = true;
                    }
                }


            }
            //对角方向
            list = new List<Vector2Int>();
            list.Add(now.Pos + Vector2Int.up + Vector2Int.left);
            list.Add(now.Pos + Vector2Int.up + Vector2Int.right);
            list.Add(now.Pos + Vector2Int.down + Vector2Int.left);
            list.Add(now.Pos + Vector2Int.down + Vector2Int.right);
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (grids[ax, ay].IsObstacle && !grids[ax, ay].IsDestructible) continue;
                    if (grids[ax, ay].IsDestructible) grids[ax, ay].Value = Mathf.Min(now.Value + 1.4f + grids[ax, ay].DestroyCost, grids[ax, ay].Value);
                    else grids[ax, ay].Value = Mathf.Min(now.Value + 1.4f, grids[ax, ay].Value);

                    if (!closeBools[ax, ay] && !openBools[ax, ay] && (!grids[ax, ay].IsObstacle || grids[ax, ay].IsDestructible))
                    {
                        Open.Enqueue(grids[ax, ay]);
                        openBools[ax, ay] = true;
                    }
                }

            }
            closeBools[now.Pos.x, now.Pos.y] = true;

        }

        private static void AddOtherPos(VectorNode now, RangeBox2D rangeLimit)
        {

            //单方向
            List<Vector2Int> list = new List<Vector2Int>();
            list.Add(now.Pos + Vector2Int.up);
            list.Add(now.Pos + Vector2Int.down);
            list.Add(now.Pos + Vector2Int.left);
            list.Add(now.Pos + Vector2Int.right);
            list.RemoveAll(x => !rangeLimit.Contains(x));
            int ax, ay;
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (grids[ax, ay].IsObstacle && !grids[ax, ay].IsDestructible) continue;
                    if (grids[ax, ay].IsDestructible) grids[ax, ay].Value = Mathf.Min(now.Value + 1 + grids[ax, ay].DestroyCost, grids[ax, ay].Value);
                    else grids[ax, ay].Value = Mathf.Min(now.Value + 1, grids[ax, ay].Value);
                    if (!closeBools[ax, ay] && !openBools[ax, ay] && (!grids[ax, ay].IsObstacle || grids[ax, ay].IsDestructible))
                    {

                        Open.Enqueue(grids[ax, ay]);
                        openBools[ax, ay] = true;
                    }
                }


            }
            //对角方向
            list = new List<Vector2Int>();
            list.Add(now.Pos + Vector2Int.up + Vector2Int.left);
            list.Add(now.Pos + Vector2Int.up + Vector2Int.right);
            list.Add(now.Pos + Vector2Int.down + Vector2Int.left);
            list.Add(now.Pos + Vector2Int.down + Vector2Int.right);
            list.RemoveAll(x => !rangeLimit.Contains(x));
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (grids[ax, ay].IsObstacle && !grids[ax, ay].IsDestructible) continue;
                    if (grids[ax, ay].IsDestructible) grids[ax, ay].Value = Mathf.Min(now.Value + 1.4f + grids[ax, ay].DestroyCost, grids[ax, ay].Value);
                    else grids[ax, ay].Value = Mathf.Min(now.Value + 1.4f, grids[ax, ay].Value);
                    if (!closeBools[ax, ay] && !openBools[ax, ay] && (!grids[ax, ay].IsObstacle || grids[ax, ay].IsDestructible))
                    {

                        Open.Enqueue(grids[ax, ay]);
                        openBools[ax, ay] = true;
                    }
                }

            }
            closeBools[now.Pos.x, now.Pos.y] = true;
        }







        private static List<VectorNode> GetSroundNode(VectorNode now)
        {
            List<VectorNode> result = new List<VectorNode>();

            List<Vector2Int> list = new List<Vector2Int>() {
            now.Pos + Vector2Int.up,
            now.Pos + Vector2Int.down,
            now.Pos + Vector2Int.left,
            now.Pos + Vector2Int.right,
            now.Pos + Vector2Int.up + Vector2Int.left,
            now.Pos + Vector2Int.up + Vector2Int.right,
            now.Pos + Vector2Int.down + Vector2Int.left,
            now.Pos + Vector2Int.down + Vector2Int.right

            };
            int ax, ay;
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (!grids[ax, ay].IsObstacle || grids[ax, ay].IsDestructible) result.Add(grids[ax, ay]);
                }
            }
            return result;
        }

        private static List<VectorNode> GetMoveableSroundNode(VectorNode now)
        {
            List<Vector2Int> list = new List<Vector2Int>() {
            now.Pos + Vector2Int.up,
            now.Pos + Vector2Int.down,
            now.Pos + Vector2Int.left,
            now.Pos + Vector2Int.right,

            };
            List<Vector2Int> list2 = new List<Vector2Int>() {
            now.Pos + Vector2Int.up + Vector2Int.left,
            now.Pos + Vector2Int.up + Vector2Int.right,
            now.Pos + Vector2Int.down + Vector2Int.left,
            now.Pos + Vector2Int.down + Vector2Int.right

            };
            List<VectorNode> result = new List<VectorNode>();


            //List<Vector2Int> listObstacle=list.Where(x=>grids.ContainsPos(x.x,x.y)&& grids[x.x, x.y].IsObstacle && !grids[x.x, x.y].IsDestructible).ToList();

            //list2.RemoveAll(x => listObstacle.Where(y=>(y.x.Equals(x.x) && Mathf.Abs(y.y - x.y) == 1) ||(y.y.Equals(x.y) && Mathf.Abs(y.x - x.x) == 1)).Count()==2);
            int ax, ay;
            foreach (var a in list)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (!grids[ax, ay].IsObstacle || grids[ax, ay].IsDestructible) result.Add(grids[ax, ay]);
                    else
                    {
                        list2.RemoveAll(x => x.x == ax + 1 || x.x == ax - 1 || x.y == ay + 1 || x.y == ay - 1);
                    }
                }
            }



            foreach (var a in list2)
            {
                ax = a.x;
                ay = a.y;
                if (grids.ContainsPos(ax, ay))
                {
                    if (!grids[ax, ay].IsObstacle || grids[ax, ay].IsDestructible) result.Add(grids[ax, ay]);
                }
            }


            return result;
        }

        /// <summary>
        /// 计算障碍物
        /// </summary>
        /// <param name="xRange">检测范围x</param>
        /// <param name="yRange">检测范围y</param>
        /// <param name="obstacleLayer">障碍物图层</param>
        public static void OperateObstacles(Range2Int xRange, Range2Int yRange)
        {
            xRange = new(xRange);
            yRange = new(yRange);
            grids = new SerializabledCenter2D<VectorNode>(xRange, yRange);
            openBools = new SerializabledCenter2D<bool>(xRange, yRange);
            closeBools = new SerializabledCenter2D<bool>(xRange, yRange);
            grids.ForEach((item, x, y) =>
            {
                item.Pos = new Vector2Int(x, y);
                item.Value = 100000000;
            });
            RangeBox2D mid = new RangeBox2D(xRange, yRange);
            CheckObstacles(mid);

        }

        public static void CheckObstacles(RangeBox2D rangeBox2D)
        {

            Collider2D collider2D = Physics2D.OverlapBox(rangeBox2D.centerPos, rangeBox2D.size, 0, obstacleLayer);
            if (collider2D != null)
            {

                List<RangeBox2D> rangeBox2Ds = rangeBox2D.Quartile();
                if (rangeBox2Ds.Count == 0)
                {
                    VectorNode item = grids[rangeBox2D.xMin, rangeBox2D.yMin];
#if UNITY_EDITOR
                    DebugDraw.DrawBox((Vector3Int)item.Pos);
#endif
                    item.IsObstacle = true;
                    item.Value = 100000000;
                    if (collider2D.TryGetComponent(out IDestoey destoey))
                    {
                        item.IsDestructible = true;
                        item.DestroyCost = destoey.DestoryCost();
                    }

                }
                else
                {
                    foreach (var a in rangeBox2Ds)
                    {
                        CheckObstacles(a);
                    }
                }
            }
            else
            {

                VectorNode item;
                for (int i = rangeBox2D.xMin; i <= rangeBox2D.xMax; i++)
                {
                    for (int j = rangeBox2D.yMin; j <= rangeBox2D.yMax; j++)
                    {
                        item = grids[i, j];
                        item.IsObstacle = false;
                        item.IsDestructible = false;
                        // item.Value = 100000000;
                    }
                }

            }

        }


        public static Vector2 GetMoveDirection(Vector2Int pos)
        {
            if (grids.ContainsPos(pos.x, pos.y))
            {
                return grids[pos.x, pos.y].TargetVector;
            }
            else
            {
                Debug.Log("传入位置越界，暂未实现解决方案，将返回Vector2.zero向量");

                return Vector2.zero;
            }

        }


    }


    public interface IVectorFieldMove
    {
        public bool NeedVectorFieldMove { get; set; }
        public bool IsArrival { get; set; }
        public Vector2 NowPos => Transform.position;
        public Transform Transform { get; }
        public int Speed { get; set; }

        public bool Move()
        {

            float maxDistance = Speed * Time.fixedDeltaTime;
            Vector2 moveDir = VectorField.GetMoveDirection(Transform.position.RoundToVector2Int());
            Transform.position = Vector2.MoveTowards(Transform.position, (Vector2)Transform.position + moveDir * Speed, maxDistance);
            if (moveDir.Equals(Vector2.zero))
            {
                IsArrival = true;
            }
            return IsArrival;
        }


        public void OnEnableNew(int speed)
        {
            IsArrival = false;
            NeedVectorFieldMove = true;
            Speed = speed;

        }

        public void FixedUpdateNew()
        {
            if (NeedVectorFieldMove && !IsArrival) Move();

        }


        /// <summary>
        /// 关闭寻路
        /// </summary>
        public void CloseVectorFieldMove()
        => NeedVectorFieldMove = false;
        /// <summary>
        /// 开启寻路
        /// </summary>
        public void ResumeVectorFieldMove()
        => NeedVectorFieldMove = true;



    }


}

namespace RuiRuiDebug
{
    public static class DebugDraw
    {
        public static void DrawArrow(Vector2 start, Vector2 direction, Color color, float duration, float distance)
        {
            Vector2 end = start + direction * distance;
            Vector2 arrowStart1;
            Vector2 arrowStart2;
            float degree = 30;

            arrowStart1 = (Vector3)end - Quaternion.Euler(0, 0, degree) * direction * distance * 0.2f;
            arrowStart2 = (Vector3)end - Quaternion.Euler(0, 0, -degree) * direction * distance * 0.2f;

            Debug.DrawLine(start, end, color, duration);
            Debug.DrawLine(arrowStart1, end, color, duration);
            Debug.DrawLine(arrowStart2, end, color, duration);
        }
        public static void DrawBox(Vector3 pos)
        {
            Debug.DrawLine(pos, pos + Vector3.up * 0.5f, Color.red, 2);
            Debug.DrawLine(pos, pos + Vector3.down * 0.5f, Color.red, 2);
            Debug.DrawLine(pos, pos + Vector3.left * 0.5f, Color.red, 2);
            Debug.DrawLine(pos, pos + Vector3.right * 0.5f, Color.red, 2);


            Debug.DrawLine(pos + Vector3.up * 0.5f + Vector3.left * 0.5f, pos + Vector3.right * 0.5f + Vector3.up * 0.5f, Color.red, 2);
            Debug.DrawLine(pos + Vector3.down * 0.5f + Vector3.left * 0.5f, pos + Vector3.right * 0.5f + Vector3.down * 0.5f, Color.red, 2);
            Debug.DrawLine(pos + Vector3.up * 0.5f + Vector3.left * 0.5f, pos + Vector3.down * 0.5f + Vector3.left * 0.5f, Color.red, 2);
            Debug.DrawLine(pos + Vector3.up * 0.5f + Vector3.right * 0.5f, pos + Vector3.right * 0.5f + Vector3.down * 0.5f, Color.red, 2);


        }

    }
}

