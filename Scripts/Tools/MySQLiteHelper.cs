using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Text;

public class MySQLiteHelper : Singleton<MySQLiteHelper> {

	private SqlDbType type;

	private IDbCommand m_command;

	private IDbConnection m_connection;

	private IDataReader m_reader;

	private IDbDataParameter m_param;

	private IDbTransaction m_transaction;

	// 已建立连接的数据库
	private Dictionary<string,IDbConnection> connectionDic = new Dictionary<string, IDbConnection>();


	private MySQLiteHelper(){

	}


	/// <summary>
	/// 连接指定名称的数据库
	/// </summary>
	/// <returns>The connection with.</returns>
	/// <param name="DbName">Db name.</param>
	public IDbConnection GetConnectionWith(string dbName){

		// 根据运行平台获取数据库的存储路径
//		string dbFilePath = GetDbFilePath (DbName);


		string dbDirectoryPath = GetDbDirectoryPath ();

		string dbFilePath = GetDbFilePath (dbName);

		DirectoryInfo folder = new DirectoryInfo (dbDirectoryPath);


		foreach (FileInfo file in folder.GetFiles("*.db"))
		{
			// 如果本地有指定名称的数据库，则尝试打开数据库
			if (dbFilePath.Contains(dbFilePath)){

				try{
					// 创建数据库
					m_connection = new SqliteConnection (dbFilePath);
					// 打开数据库
					m_connection.Open ();
					// 数据库连接写入字典数据中
					connectionDic.Add(dbName,m_connection);

					return m_connection;

				}catch(Exception e){
					
					Debug.Log (e);

					if (m_connection != null && m_connection.State != System.Data.ConnectionState.Closed) {
						m_connection.Close ();
						m_connection = null;
					}
					return null;
				}
			}
		}

		// 如果本地没有指定名称的数据库，则给出提示
		Debug.Log ("未找到指定名称的数据库");
		return null;

	}


	/// <summary>
	/// 创建数据库
	/// </summary>
	/// <returns>The database.</returns>
	/// <param name="DbName">Db name.</param>
	public IDbConnection CreatDatabase(string DbName){

		string dbFilePath = GetDbFilePath (DbName);

		try{
			// 创建数据库
			m_connection = new SqliteConnection (dbFilePath);
			// 打开数据库
			m_connection.Open ();
			// 数据库连接写入字典数据中
			connectionDic.Add(dbFilePath,m_connection);

			return m_connection;

		}catch(Exception e){

			Debug.Log (e);

			if (m_connection != null && m_connection.State != System.Data.ConnectionState.Closed) {
				m_connection.Close ();
				m_connection = null;
			}
			return null;
		}
	}

	/// <summary>
	/// 获取不同平台下存储数据库的文件夹路径
	/// </summary>
	/// <returns>存储数据库的文件夹路径.</returns>
	private string GetDbDirectoryPath(){
		string dbDirectoryPath = string.Empty;
		// pc平台 or 编译器环境
		if (Application.platform == RuntimePlatform.WindowsEditor ||
		    Application.platform == RuntimePlatform.OSXEditor ||
		    Application.platform == RuntimePlatform.WindowsPlayer ||
		    Application.platform == RuntimePlatform.OSXPlayer) {
			dbDirectoryPath = Application.streamingAssetsPath;
		} 
		// android平台
		else if (Application.platform == RuntimePlatform.Android) {
			dbDirectoryPath =  Application.persistentDataPath;
		}
		// ios平台
		else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			dbDirectoryPath =  Application.persistentDataPath;
		}
		return dbDirectoryPath;

	}

	// 数据库名称 -> 数据库完整路径
	private string GetDbFilePath(string dbName){
				
		string dbFilePath = string.Empty;

		// pc平台 or 编译器环境
		if (Application.platform == RuntimePlatform.WindowsEditor ||
			Application.platform == RuntimePlatform.OSXEditor ||
			Application.platform == RuntimePlatform.WindowsPlayer ||
			Application.platform == RuntimePlatform.OSXPlayer) {
			dbFilePath = string.Format ("data source={0}/{1}", Application.streamingAssetsPath,dbName);
		} 
		// android平台
		else if (Application.platform == RuntimePlatform.Android) {
			dbFilePath = string.Format ("URI=file:{0}/{1}", Application.persistentDataPath,dbName);
		}
		// ios平台
		else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			dbFilePath = string.Format ("data source={0}/{1}", Application.persistentDataPath,dbName);
		}
		return dbFilePath;

	}

	// 关闭指定名称的数据库连接
	public void CloseConnection(string dbName){

		CloseConnection (dbName, true);

	}
	// 关闭指定名称的数据库连接
	private void CloseConnection(string dbName,bool removeConnection){
		IDbConnection connection = null;

		if (connectionDic [dbName] != null) {

			connection = connectionDic [dbName];

			// 销毁Reader
			if (m_reader != null)
			{
				m_reader.Close();
				m_reader = null;
			}

			// 销毁Command
			if (m_command != null) {
				m_command.Cancel ();
				m_command.Dispose();
				m_command = null;
			}
				
			connection.Close ();

			connection.Dispose ();

			connection = null;

			if (removeConnection) {
				connectionDic.Remove (dbName);
			}

		}
	}

	// 关闭所有数据库连接
	public void CloseAllConnections(){
		
		foreach (KeyValuePair<string,IDbConnection> kv in connectionDic) {

			CloseConnection (kv.Key,false);

		}

		connectionDic.Clear ();
	}


	/// <summary>
	/// 清理SQLite的引用关系
	/// </summary>
	public void CleanSQLiteReference()
	{
		// 销毁Command
		if (m_command != null)
		{
			m_command.Cancel ();
			m_command.Dispose();
			m_command = null;
		}

		// 销毁Reader
		if (m_reader != null)
		{
			m_reader.Close();
			m_reader = null;
		}
	}

	/// <summary>
	/// 执行无参数SQL命令
	/// </summary>
	/// <returns>The query.</returns>
	/// <param name="queryString">SQL命令字符串</param>
	public IDataReader ExecuteQuery(string queryString)
	{
		return ExecuteQuery (queryString, null);
	}

	/// <summary>
	/// 执行带参数SQL命令
	/// </summary>
	/// <param name="queryString"></param>
	/// <param name="para"></param>
	/// <returns></returns>
	public IDataReader ExecuteQuery(string queryString, IDataParameter para)
	{
		try
		{
			m_command = m_connection.CreateCommand();

			if (m_reader != null && !m_reader.IsClosed)
				m_reader.Close();

			m_command.CommandText = queryString;

			if(para != null){
				m_command.Parameters.Add(para);
			}

			m_reader = m_command.ExecuteReader();

		}
		catch (Exception e)
		{
			Debug.LogError(e);

			if (m_connection != null && m_connection.State != System.Data.ConnectionState.Closed) {
				m_connection.Close ();
				m_connection = null;
			}

			return null;
		}

		return m_reader;
	}

	/// <summary>
	/// 新建一个表
	/// </summary>
	/// <returns>The table.</returns>
	/// <param name="tableName">Table name.</param>
	/// <param name="colNames">Col names.</param>
	/// <param name="colTypes">Col types.</param>
	public bool CreatTable(string tableName,string[] colNames,string[] colTypes){

		if (colNames.Length != colTypes.Length) {
			throw new SqliteException ("类型数量错误");
		}

//		if(CheckTableExist(tableName)){
//			Debug.Log ("-------表" + tableName + "已存在----------");
//			return false;
//		}

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat ("CREATE TABLE {0}( {1} {2}", tableName, colNames[0], colTypes[0]);

		for (int i = 1; i < colNames.Length; i++) {
			queryString.AppendFormat (", {0} {1}", colNames [i], colTypes [i]);
		}

		queryString.Append (" )");

		ExecuteQuery (queryString.ToString());

		return true;
	}


	/// <summary>
	/// 检查指定名称的表是否存在
	/// </summary>
	/// <returns><c>true</c>, if table exist, <c>false</c> otherwise.</returns>
	/// <param name="tableName">Table name.</param>
	public bool CheckTableExist(String tableName){

		string queryString = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";

		IDataReader reader = ExecuteQuery (queryString);

		if (reader.GetInt32(0) == 0) {
			return false;
		}

		return true;
	}

	/// <summary>
	/// 删除指定名称的表
	/// </summary>
	/// <returns><c>true</c>, if table was deleted, <c>false</c> otherwise.</returns>
	/// <param name="tableName">Table name.</param>
	public bool DeleteTable(string tableName){

		if (!CheckTableExist (tableName)) {

			Debug.Log ("------表" + tableName + "不存在------");

			return false;
		}

		string queryString = "DROP TABLE" + tableName;

		ExecuteQuery (queryString);

		return !CheckTableExist (tableName);

	}


	/// <summary>
	/// 读取整张表的数据
	/// </summary>
	/// <returns> IDataReader </returns>
	/// <param name="tableName">表名</param>
	public IDataReader ReadFullTable(string tableName)
	{
		string queryString = "SELECT * FROM " + tableName;

		return ExecuteQuery (queryString);
	} 

	/// <summary>
	/// 查询指定条件下的数据
	/// </summary>
	/// <returns>IDataReader.</returns>
	/// <param name="tableName">表名.</param>
	/// <param name="fieldName">字段名.</param>
	/// <param name="condition">查询条件.</param>
	public IDataReader ReadSpecificRowsOfTable(string tableName,string fieldName,string[] conditions,string ANDorOR){

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat("SELECT {0} FROM {1}",fieldName,tableName);

		if (conditions.Length > 0) {

			queryString.Append (" WHERE ");

			for (int i = 0; i < conditions.Length; i++) {

				queryString.AppendFormat ("{0} {1} ", conditions [i],ANDorOR);

			}

			queryString.Remove (queryString.Length - ANDorOR.Length - 1, ANDorOR.Length);

		}

		Debug.Log (queryString);

		return ExecuteQuery (queryString.ToString());
	}



	/// <summary>
	/// 向指定数据表中插入数据
	/// </summary>
	/// <param name="tableName">数据表名称</param>
	/// <param name="values">插入的数组</param>
	/// <returns>IDataReader</returns>
	public IDataReader InsertValues(string tableName, string[] values)
	{
		//获取数据表中字段数目
		int fieldCount = ReadFullTable(tableName).FieldCount;

		//当插入的数据长度不等于字段数目时引发异常
		if (values.Length != fieldCount)
		{
			throw new SqliteException("values.Length != fieldCount");
		}

		StringBuilder queryString = new StringBuilder();

		queryString.AppendFormat("INSERT INTO {0} VALUES ({1}", tableName, values[0]);

		for (int i = 1; i < values.Length; i++)
		{
			queryString.AppendFormat(", {0}", values[i]);
		}
		queryString.Append(" )");

		Debug.Log (queryString);

		return ExecuteQuery(queryString.ToString());
	}
		
	/// <summary>
	/// 更新表中指定条件的数据
	/// </summary>
	/// <returns>IDataReader.</returns>
	/// <param name="tableName">Table name.</param>
	/// <param name="cols">字段名.</param>
	/// <param name="values">赋值数组.</param>
	/// <param name="condition">查询条件字符串.</param>
	public IDataReader UpdateSpecificColsWithValues(string tableName,string[] cols,string[] values,string[] conditions,string ANDorOR){

		if (cols.Length != values.Length) {
			throw new SqliteException ("字段数量和赋值数量不匹配");
		}

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat ("UPDATE {0} SET {1} = {2}", tableName, cols [0], values [0]);

		for (int i = 1; i < cols.Length; i++) {
			queryString.AppendFormat (" ,{0} = {1}", cols [i], values [i]);
		}

		if (conditions.Length > 0) {

			queryString.Append (" WHERE ");

			for (int i = 0; i < conditions.Length; i++) {

				queryString.AppendFormat ("{0} {1} ", conditions [i],ANDorOR);

			}

			queryString.Remove (queryString.Length - ANDorOR.Length - 1, ANDorOR.Length);

		}
			

		Debug.Log (queryString);

		return ExecuteQuery (queryString.ToString ());
	}


	/// <summary>
	/// 删除如何指定条件的行数据
	/// </summary>
	/// <returns>IDataReader.</returns>
	/// <param name="tableName">Table name.</param>
	/// <param name="condition">Condition.</param>
	public IDataReader DeleteSpecificRows(string tableName,string[] conditions,string ANDorOR){

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat ("DELETE FROM {0}", tableName);

		if (conditions.Length > 0) {

			queryString.Append (" WHERE ");

			for (int i = 0; i < conditions.Length; i++) {

				queryString.AppendFormat ("{0} {1} ", conditions [i],ANDorOR);

			}

			queryString.Remove (queryString.Length - ANDorOR.Length - 1, ANDorOR.Length);

		}
		Debug.Log (queryString);

		return ExecuteQuery (queryString.ToString());
	}

	#region 

	#endregion

}
