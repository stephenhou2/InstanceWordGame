using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace WordJourney
{
	public class NavigationHelper : MonoBehaviour {

		// 地图上可行走位置的二维数组
		// －1表示是墙或者地图中的镂空区域，不可到达
		// 0表示障碍物,不可行走
		// 1表示正常地面
		// 10表示有陷阱（陷阱的消耗值为10，即如果为了绕过陷阱要多走20步以上时，直接走陷阱，小于10步时，则选择绕路）
		// 1和2同时也是寻路参数中的G值
		public int[,] mapWalkableInfoArray;

		// 可以用来检测的点阵
		private List<Point> openList = new List<Point>();

		// 已关闭的点
		private List<Point> closedList = new List<Point>();

		// 自动寻路路径上的点集
		private List<Vector3> pathPos = new List<Vector3>();


		/// <summary>
		/// 寻路接口
		/// </summary>
		/// <returns>The path.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		/// <param name="mapWalkableInfoArray">Map walkable info array.</param>
		public List<Vector3> FindPath(Vector3 startPos,Vector3 endPos,int[,] mapWalkableInfoArray){

			Point startPoint = new Point(startPos);
			Point endPoint = new Point(endPos);

			// 每次自动探测路径前将待检测点集和已关闭点集和路径点集清空
			openList.Clear ();
			closedList.Clear ();
			pathPos.Clear ();

			// 拿到全地图信息（包括是否可行走和每个点上的行走消耗）
			this.mapWalkableInfoArray = mapWalkableInfoArray;


			// 起点和终点重合，则将该点加入到路径点集中并直接返回
			if (startPoint.Equals (endPoint)) {
				pathPos.Add (endPos);
				return pathPos;
			}

			// 如果终点是不可行走点
			if (mapWalkableInfoArray [endPoint.x, endPoint.y] == -1) {
				Debug.Log ("点击在不可到达区域");
				return pathPos;
			}

			// 记录起始点
			Point currentPoint = startPoint;

			// 将起始点加入待检测点集
			openList.Add (startPoint);

			// 待检测点集内如果有待检测点，就遍历下去
			while (openList.Count != 0) {

				// 移除当前点
				openList.Remove (currentPoint);

				// 当前点加入到关闭点集中
				closedList.Add (currentPoint);

				// 拿到当前点的周围点集（上下左右）
				List<Point> surroundedPoints = currentPoint.GetSurroundedPoints ();

				// 遍历周围点集
				for(int i = 0; i<surroundedPoints.Count; i++) {
					
					Point p = surroundedPoints[i];

					// 如果周围点集中有终点，则停止检测，并利用fatherPoint属性逐级获取路径中的上级点
					if (p.Equals (endPoint)) {
						
						p.fatherPoint = currentPoint;

						while (p.fatherPoint != null) {
							pathPos.Add (new Vector3 (p.x, p.y, 0));
							p = p.fatherPoint;

						}

						pathPos.Reverse ();

						return pathPos;
					}

					// 如果周围点集中有不可行走点或者是已关闭点或者已经在待检测点集中，则这个点什么都不做
					if (UnwalkableOrClosedOrExistInOpen (p)) {
						continue;
					}

					// 从地图信息中获取当前周围点集中可行走待检测点的行走耗费
					int G = mapWalkableInfoArray [p.x, p.y];

					// 计算F，G，H
					p.CaculateFGH (currentPoint, endPoint, G);

					// 设置父级节点
					p.fatherPoint = currentPoint;

					// 将可行走待检测点加入到待检测点集中
					openList.Add (p);

				}

				// 获取待检测点集中F值做小的点
				Point minFPoint = GetMinFPoint (openList);

				if (minFPoint != null) {

					// 将F值最小的点设置未当前点（基点，周围点是由基点找出来的）
					currentPoint = minFPoint;

				}
					
			}

			// 走到这里说明没有有效路径，返回的pathPos内部节点数量为0
			return pathPos;
		}

		/// <summary>
		/// 获取待检测点集中F值最小的点
		/// </summary>
		/// <returns>The minimum F point.</returns>
		/// <param name="openList">Open list.</param>
		private Point GetMinFPoint(List<Point> openList){

			// 如果待检测点集中点的数量不为0，则对待检测点集中的点根据F值进行升序排列，返回F最小的点
			if (openList.Count != 0) {

				openList = openList.OrderBy (p => p.F).ToList ();

				return openList [0];

			} 

			// 待检测点集中没有点，返回null
			return null;

		}

		/// <summary>
		/// 点是否可行走或者是否已关闭或者已经在待检测点集中
		/// </summary>
		/// 障碍物或者不可到达点或者是已经关闭或者已经在待检测点集中返回true，其余返回false
		/// <param name="p">P.</param>
		private bool UnwalkableOrClosedOrExistInOpen(Point p){


			if (mapWalkableInfoArray [p.x, p.y] == 0 || mapWalkableInfoArray [p.x, p.y] == -1) {
				return true;
			}


			foreach (Point closedPoint in closedList) {

				if (closedPoint.Equals (p)) {
					return true;
				}
			}

			foreach (Point openPoint in openList) {

				if (openPoint.Equals (p)) {
					return true;
				}
			}

			return false;
		}
			
	}

	[System.Serializable]
	public class Point{

		public Point fatherPoint;

		public int x;

		public int y;

		public int F{ get; set;}

		private int H{ get; set;}

		private int G{ get; set;}

		public Point(int x,int y){
			this.x = x;
			this.y = y;
		}

		public Point(Vector3 positon){
			this.x = (int)positon.x;
			this.y = (int)positon.y;
		}

		public void CaculateFGH(Point fatherPoint,Point endPoint,int GofNewPoint){

			this.G = fatherPoint.G + GofNewPoint;

			this.H = Mathf.Abs (x - endPoint.x) + Mathf.Abs (y - endPoint.y);

			this.F = this.H + this.G;

		}

		public bool Equals(Point p){

			if (this.x == p.x && this.y == p.y) {
				return true;
			}
			return false;

		}

		/// <summary>
		/// 获得当前点的周围所有点（上下左右）
		/// </summary>
		/// <returns>The surrounded optimize point.</returns>
		/// <param name="currentPoint">Current point.</param>
		public List<Point> GetSurroundedPoints(){

			List<Point> surroundedPoints = new List<Point> ();

			Point upPoint = new Point (x, y + 1);

			Point downPoint = new Point (x, y - 1);

			Point leftPoint = new Point (x - 1, y);

			Point rightPoint = new Point (x + 1, y);


			surroundedPoints.Add (upPoint);
			surroundedPoints.Add (downPoint);
			surroundedPoints.Add (leftPoint);
			surroundedPoints.Add (rightPoint);

			return surroundedPoints;

		}
			
	}
}
