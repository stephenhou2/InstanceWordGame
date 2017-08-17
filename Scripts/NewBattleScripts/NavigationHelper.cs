using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace WordJourney
{
	public class NavigationHelper : MonoBehaviour {



		// 地图上可行走位置的二维数组
		// 0表示障碍物,不可行走
		// 1表示正常地面
		// 2表示有陷阱
		// 0和1同时也是寻路参数中的G值
		public int[,] mapWalkableInfoArray;

		// 可以用来检测的点阵
		private List<Point> openList = new List<Point>();

		// 已关闭的点
		private List<Point> closedList = new List<Point>();

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


			openList.Clear ();
			closedList.Clear ();
			pathPos.Clear ();

			this.mapWalkableInfoArray = mapWalkableInfoArray;

			Point currentPoint = startPoint;

			openList.Add (startPoint);

			while (openList.Count != 0) {

				openList.Remove (currentPoint);

				closedList.Add (currentPoint);

				List<Point> surroundedPoints = currentPoint.GetSurroundedPoints ();

				for(int i = 0; i<surroundedPoints.Count; i++) {
					Point p = surroundedPoints[i];
					if (p.Equals (endPoint)) {
						
						p.fatherPoint = currentPoint;

						while (p.fatherPoint != null) {
							pathPos.Add (new Vector3 (p.x, p.y, 0));
							p = p.fatherPoint;
							pathPos.Reverse ();
						}
						return pathPos;
					}

					if (UnwalkableOrClosed (p)) {
						continue;
					}

					int G = mapWalkableInfoArray [p.x, p.y];

					p.CaculateFGH (currentPoint, endPoint, G);

					p.fatherPoint = currentPoint;

					openList.Add (p);

				}


				Point minFPoint = GetMinFPoint (openList);

				currentPoint = minFPoint;
			}
			return pathPos;
		}


		private Point GetMinFPoint(List<Point> openList){

			openList = openList.OrderBy (p => p.F).ToList();
			return openList [0];
		}

		/// <summary>
		/// 点是否可行走或者是否已关闭
		/// </summary>
		/// 不可行走或者是已经关闭返回true，其余返回false
		/// <param name="p">P.</param>
		private bool UnwalkableOrClosed(Point p){

			if (mapWalkableInfoArray [p.x, p.y] == 0) {
				return true;
			}

			foreach (Point closedPoint in closedList) {

				if (closedPoint.Equals (p)) {
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
