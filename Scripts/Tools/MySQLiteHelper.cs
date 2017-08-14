using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using System.Text;

public class MySQLiteHelper : Singleton<MySQLiteHelper> {


	private IDbCommand m_command;

	private IDbConnection m_connection;

	private IDataReader m_reader;

	// 已建立连接的数据库
	private Dictionary<string,IDbConnection> connectionDic = new Dictionary<string, IDbConnection>();


	private MySQLiteHelper(){


	}


	/// <summary>
	/// 连接指定名称的数据库
	/// </summary>
	/// <returns>The connection with.</returns>
	/// <param name="DbName">Db name.</param>
	public IDbConnection GetConnectionWith(string dbName,string password = null){

		// 如果数据库连接字典中查询到指定名称的数据库连接，则直接返回该连接
		foreach (KeyValuePair<string,IDbConnection> kv in connectionDic) {

			if(kv.Key.Equals(dbName)){
				if (kv.Value.State != ConnectionState.Open) {
					kv.Value.Open ();
				}
				return kv.Value;
			}
		}


		// 如果数据库连接字典中未能查询到指定名称的数据库连接（例如退出游戏后重新进入），
		// 则在本地文件中根据文件名进行查询，查到后重新建立连接

		// 根据运行平台获取存放数据库的文件夹路径
		string dbDirectoryPath = GetDbDirectoryPath ();

		DirectoryInfo folder = new DirectoryInfo (dbDirectoryPath);
		
		// 根据运行平台获取数据库的存储路径
		string dbBuildString = GetDbStrBuilder (dbName,password);

		foreach (FileInfo fileInfo in folder.GetFiles("*.db"))
		{
			// 如果本地有指定名称的数据库，则尝试打开数据库
			if (dbName.Equals(fileInfo.Name)){

				try{
					// 创建数据库
					m_connection = new SqliteConnection (dbBuildString);
					// 打开数据库
					if(m_connection.State != ConnectionState.Open){
						m_connection.Open ();
					}
					// 数据库连接写入字典数据中
					connectionDic.Add(dbName,m_connection);

					return m_connection;

				}catch(Exception e){

					Debug.Log (e);

					// 如果出现异常则关闭所有的数据库连接
					CloseAllConnections ();

//					if (m_connection != null && m_connection.State != System.Data.ConnectionState.Closed) {
//						m_connection.Close ();
//						m_connection = null;
//					}
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
	public IDbConnection CreateDatabase(string dbName,string password = null){

		// 如果数据库连接字典中查询到指定名称的数据库连接，则直接返回该连接
		foreach (KeyValuePair<string,IDbConnection> kv in connectionDic) {

			if(kv.Key.Equals(dbName)){
				m_connection = kv.Value;
				return m_connection;
			}

		}

		// 如果数据库连接字典中未能查询到指定名称的数据库连接，则根据文件名进行连接
		// 没有指定名称的数据库则创建新数据库
		// 与数据库建立连接
		string dbBuildString = GetDbStrBuilder (dbName,password);

		try{
			// 创建数据库
			m_connection = new SqliteConnection (dbBuildString);
			// 打开数据库
			m_connection.Open ();
			// 数据库连接写入字典数据中
			connectionDic.Add(dbName,m_connection);

			return m_connection;

		}catch(Exception e){

			Debug.Log (e);

			CloseAllConnections ();

//			if (m_connection != null && m_connection.State != System.Data.ConnectionState.Closed) {
//				m_connection.Close ();
//				m_connection = null;
//			}
			return null;
		}
	}



//	public IDbConnection CreatDataBaseWithDetailInfo(string dbName,string password,

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
	private string GetDbStrBuilder(string dbName,string password = null){
				
		SqliteConnectionStringBuilder connBuilder = new SqliteConnectionStringBuilder ();

		// pc平台 or 编译器环境
		if (Application.platform == RuntimePlatform.WindowsEditor ||
			Application.platform == RuntimePlatform.OSXEditor ||
			Application.platform == RuntimePlatform.WindowsPlayer ||
			Application.platform == RuntimePlatform.OSXPlayer) {
			connBuilder.DataSource = string.Format("{0}/{1}", Application.streamingAssetsPath,dbName);
		} 
		// android平台
		else if (Application.platform == RuntimePlatform.Android) {
			connBuilder.Uri = string.Format ("file:{0}/{1}", Application.persistentDataPath,dbName);
		}
		// ios平台
		else if (Application.platform == RuntimePlatform.IPhonePlayer) {
			connBuilder.DataSource = string.Format ("{0}/{1}", Application.persistentDataPath,dbName);
		}

		if(password != null){
			connBuilder.Password = password;
		}

		return connBuilder.ToString();

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
				
			connection.Dispose ();
			connection.Close ();

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
//	public void CleanSQLiteReference()
//	{
//		// 销毁Command
//		if (m_command != null)
//		{
//			m_command.Cancel ();
//			m_command.Dispose();
//			m_command = null;
//		}
//
//		// 销毁Reader
//		if (m_reader != null)
//		{
//			m_reader.Close();
//			m_reader = null;
//		}
//	}

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

//		Debug.Log (queryString);

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

			CloseAllConnections ();

//			if (m_connection != null && m_connection.State != System.Data.ConnectionState.Closed) {
//				m_connection.Close ();
//				m_connection = null;
//			}

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
	public bool CreateTable(string tableName,string[] colNames,string[] constraints,string[] colTypes){

		if (colNames.Length != colTypes.Length || colNames.Length != constraints.Length || constraints.Length != colTypes.Length) {
			throw new SqliteException ("类型数量错误");
		}

		if(CheckTableExist(tableName)){
			Debug.Log ("-------表" + tableName + "已存在----------");
			return false;
		}

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat ("CREATE TABLE {0}( {1} {2} {3}", tableName, colNames[0], colTypes[0],constraints[0]);

		for (int i = 1; i < colNames.Length; i++) {
			queryString.AppendFormat (", {0} {1} {2}", colNames [i], colTypes [i],constraints[i]);
		}

		queryString.Append (" )");

		ExecuteQuery (queryString.ToString());

		return CheckTableExist(tableName);
	}


	/// <summary>
	/// 检查指定名称的表是否存在
	/// </summary>
	/// <returns><c>true</c>, if table exist, <c>false</c> otherwise.</returns>
	/// <param name="tableName">Table name.</param>
	public bool CheckTableExist(String tableName){

		string queryString = "SELECT count(*) FROM sqlite_master WHERE type='table' AND name='" + tableName + "'";

		IDataReader reader = ExecuteQuery (queryString);

		while(reader.Read()){

			if (reader.GetInt32(0) == 0) {
				
				return false;
			}
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

			Debug.Log (string.Format ("不存在名为{0}的表", tableName));

			return false;
		}

		string queryString = "DROP TABLE " + tableName;

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
	public IDataReader ReadSpecificRowsAndColsOfTable(string tableName,string fieldName,string[] conditions,bool isLinkStrAND){

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat("SELECT {0} FROM {1}",fieldName == null ? "*" : fieldName,tableName);

		if (conditions != null && conditions.Length > 0) {

			queryString.Append (" WHERE ");

			string linkString = isLinkStrAND ? "AND" : "OR";

			for (int i = 0; i < conditions.Length; i++) {

				queryString.AppendFormat ("{0} {1} ", conditions [i],linkString);

			}

			queryString.Remove (queryString.Length - linkString.Length - 1, linkString.Length);

		}



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

//		string findTypesString = "SELECT"

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

		return ExecuteQuery(queryString.ToString());
	}

	public IDataReader AddTableColumn(string tableName,string colName,string colType){

		if (!CheckTableExist (tableName)) {
			return null;
		}

		string queryString = string.Format ("ALERT TABLE {0} ADD COLUMN {1} {2}", tableName, colName, colType);

		return ExecuteQuery (queryString);
	}

	public IDataReader RenameTable(string oriTableName,string newTableName){

		if (!CheckTableExist (oriTableName)) {
			return null;
		}
		
		string queryString = string.Format ("ALERT TABLE {0} RENAME TO {1}", oriTableName, newTableName);

		return ExecuteQuery (queryString);

	}
		
	/// <summary>
	/// 更新表中指定条件的数据
	/// </summary>
	/// <returns>IDataReader.</returns>
	/// <param name="tableName">Table name.</param>
	/// <param name="cols">字段名.</param>
	/// <param name="values">赋值数组.</param>
	/// <param name="condition">查询条件字符串.</param>
	public IDataReader UpdateSpecificColsWithValues(string tableName,string[] cols,string[] values,string[] conditions,bool isLinkStrAND){

		if (cols.Length != values.Length) {
			throw new SqliteException ("字段数量和赋值数量不匹配");
		}

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat ("UPDATE {0} SET {1} = {2}", tableName, cols [0], values [0]);

		for (int i = 1; i < cols.Length; i++) {
			queryString.AppendFormat (" ,{0} = {1}", cols [i], values [i]);
		}

		if (conditions != null && conditions.Length > 0) {

			queryString.Append (" WHERE ");

			string linkString = isLinkStrAND ? "AND" : "OR";

			for (int i = 0; i < conditions.Length; i++) {

				queryString.AppendFormat ("{0} {1} ", conditions [i],linkString);

			}

			queryString.Remove (queryString.Length - linkString.Length - 1, linkString.Length);

		}

		return ExecuteQuery (queryString.ToString ());
	}


	/// <summary>
	/// 删除如何指定条件的行数据
	/// </summary>
	/// <returns>IDataReader.</returns>
	/// <param name="tableName">Table name.</param>
	/// <param name="condition">Condition.</param>
	public IDataReader DeleteSpecificRows(string tableName,string[] conditions,bool isLinkStrAND){

		StringBuilder queryString = new StringBuilder ();

		queryString.AppendFormat ("DELETE FROM {0}", tableName);

		if (conditions != null && conditions.Length > 0) {

			queryString.Append (" WHERE ");

			string linkString = isLinkStrAND ? "AND" : "OR";

			for (int i = 0; i < conditions.Length; i++) {

				queryString.AppendFormat ("{0} {1} ", conditions [i],linkString);

			}

			queryString.Remove (queryString.Length - linkString.Length - 1, linkString.Length);

		}

		return ExecuteQuery (queryString.ToString());
	}

	public IDataReader DeleteAllDataFromTable(string tableName){

		string queryString = "DELETE FROM " + tableName;

		return ExecuteQuery (queryString);
	}

	/// <summary>
	/// 检查字段名是否一致
	/// </summary>
	/// <param name="tableName">Table name.</param>
	/// <param name="fieldNameStrs">Field name strs.</param>
	public void CheckFiledNames(string tableName,string[] fieldNameStrs){

		string sqlString = "PRAGMA table_info(" + tableName + ")";

		IDataReader reader = ExecuteQuery (sqlString);

		List<string> fieldNamesInTable = new List<string> ();

		while (reader.Read ()) {

			string fieldName = reader.GetString (1);
			fieldNamesInTable.Add (fieldName);

		}

		if (fieldNamesInTable.Count != fieldNameStrs.Length) {
			throw new SqliteException ("字段数量不一致" + "table:" + fieldNamesInTable.Count.ToString() + "data:" + fieldNameStrs.Length.ToString());

		}


		for (int i = 0; i < fieldNameStrs.Length; i++) {
			if (!fieldNamesInTable [i].Equals(fieldNameStrs [i])) {
				throw new SqliteException ("字段名称不一致" + "/" + fieldNamesInTable[i] + "/" + fieldNameStrs [i] + "/");
			}

		}

	}

	/// <summary>
	/// 开启查询事务
	/// </summary>
	public IDataReader BeginTransaction(){

		string queryString = "BEGIN TRANSACTION";

		return ExecuteQuery (queryString);

	}

	/// <summary>
	/// 关闭查询事务
	/// </summary>
	public IDataReader EndTransaction(){
		
		string queryString = "END TRANSACTION";

		return ExecuteQuery (queryString);

	}

	/// <summary>
	/// 回滚到上次保存状态
	/// </summary>
	public IDataReader RollBack(){
		
		string queryString = "ROLLBACK";

		return ExecuteQuery (queryString);
	}



	#region 

	/// <summary>
	/// 移动平台数据库持久化方法
	/// </summary>
	/// <param name= "*.extention"  > 格式为  "*.扩展名"  .</param>
	/// <param name="overrideIfExist">If set to <c>true</c> 如果文件存在是否覆盖目标文件 </param>
	public static IEnumerator PersistDatabases(string fileNameModel,bool overrideIfExist){

		if (Application.platform == RuntimePlatform.Android || 
			Application.platform == RuntimePlatform.IPhonePlayer) {

			string streamingAssetsDirPath = Application.streamingAssetsPath;

			string persistDirPath = Application.persistentDataPath;

			DirectoryInfo folder = new DirectoryInfo (streamingAssetsDirPath);

			FileInfo[] fileInfos = folder.GetFiles (fileNameModel);

			string persistFilePath = string.Empty;

			for(int i = 0;i<fileInfos.Length;i++){

				FileInfo fileInfo = fileInfos [i];

//				File.Move (fileInfo.FullName, persistDirPath + "/" + fileInfo.Name);

				persistFilePath = persistDirPath + "/" + fileInfo.Name;

				if (File.Exists (persistFilePath) && !overrideIfExist) {
					
					continue;

				} else {
					
					WWW fileData = new WWW (fileInfo.FullName);

					yield return fileData;

					try{
						
						File.WriteAllBytes (persistFilePath,fileData.bytes);

					}catch(Exception e){
						
						Debug.Log ("error during persistance " + e.ToString());

					}

				} 
			}

		}

	}

	#endregion


}
