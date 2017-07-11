using UnityEngine;
using System.Collections;
using Mono.Data.Sqlite;
using System;
using System.IO;
using System.Data;
using System.Text;
using System.Collections.Generic;

public class SQLiteHelper : Singleton<SQLiteHelper>
{
	public enum SQLiteDataType
	{
		DT_NULL,
		DT_INTEGER,
		DT_REAL,
		DT_TEXT,
		DT_BLOB,
	}

	#region Static 私有字段
	/// <summary>
	/// 所有数据库文件名的数组
	/// </summary>
	private string[] m_DatabaseFiles = new string[] {CommonData.playerPrefsDB};

	// <summary>
	/// 数据库连接定义
	/// </summary>
	private IDbConnection m_Connection;

	public IDbConnection Connection
	{
		get { return m_Connection; }
	}

	/// <summary>
	/// SQL命令定义
	/// </summary>
	private IDbCommand m_Command;

	/// <summary>
	/// 数据读取定义
	/// </summary>
	private IDataReader m_Reader;

	/// <summary>
	/// 保留字段，暂时无用
	/// </summary>
	private bool m_ReleasedFromStreamingAssets = false;

	/// <summary>
	/// 存储上次出连接的数据库的文件名字符串
	/// </summary>
	private string m_CommonDataChache = string.Empty;
	#endregion

	/// <summary>
	/// 空构造函数
	/// </summary>
	private SQLiteHelper() { }

	/// <summary>
	/// 异常信息格式化
	/// </summary>
	/// <param name="msg">抛出的异常字符串</param>
	/// <returns>格式化之后的字符串</returns>
	public static string MakeDatabaseExceptionMessage(string msg)
	{
		return string.Format("<color=magenta>SQLite Exception : {0}</color>", msg);
	}

	#region Static 数据库相关操作
	///// <summary>
	///// 加密数据库
	///// </summary>
	///// <param name="password"></param>
	//public void EncryptDatabase(string password)
	//{
	//    SqliteConnection con = m_Connection as SqliteConnection;
	//    con.ChangePassword(password);
	//}

	/// <summary>
	/// 设置当前要使用的数据库
	/// </summary>
	/// <param name="databaseFileName"></param>
	/// <returns></returns>
	public bool SetCurrentDatabaseConnection(string databaseFileName)
	{
		m_Connection = GetOrCreateConnect(databaseFileName);

		if (m_Connection != null)
			return true;
		else
			return false;
	}

	Dictionary<string, IDbConnection> connectDic = new Dictionary<string, IDbConnection>(); // 存储数据连接类型的字典

	private IDbConnection GetOrCreateConnect(string databaseFileName)
	{
		foreach (KeyValuePair<string, IDbConnection> dc in connectDic)
		{
			if (string.Compare(dc.Key, databaseFileName) == 0)
			{
				return dc.Value;
			}
		}

		string connectionString = this.GetConnectionString(databaseFileName);

		IDbConnection curConnection = null; // 创建连接对象

		try
		{
			// 构造数据库链接
			curConnection = (new SqliteConnection(connectionString)) as IDbConnection;

			// 打开数据库
			curConnection.Open();

			connectDic.Add(databaseFileName, curConnection);

			return curConnection;
		}
		catch (Exception e)
		{
			Debug.LogError("connection command " + connectionString);
			Debug.LogError(SQLiteHelper.MakeDatabaseExceptionMessage(e.Message));

			if (curConnection != null && curConnection.State != System.Data.ConnectionState.Closed)
			{
				curConnection.Close();
				curConnection = null;
			}

			return null; 
		}
	}

	/// <summary>
	/// 关闭某个数据库连接
	/// </summary>
	public void CloseConnection(string databaseFileName)
	{            
		foreach(KeyValuePair<string, IDbConnection> kv in connectDic)
		{
			if (string.Compare(kv.Key, databaseFileName) == 0)
			{
				kv.Value.Close();                    
			}
		}

		connectDic.Remove(databaseFileName);

		// 销毁Command
		if (m_Command != null)
		{
			m_Command.Cancel();
			m_Command = null;

		}

		// 销毁Reader
		if (m_Reader != null)
		{
			m_Reader.Close();
			m_Reader = null;
		}

		// Connection置空
		if (m_Connection != null)
		{
			m_Connection = null;
		}
	}

	/// <summary>
	/// 关闭所有数据库连接
	/// </summary>
	public void CloseConnection()
	{
		foreach (KeyValuePair<string, IDbConnection> kv in connectDic)
		{                
			kv.Value.Close();               
		}
		connectDic.Clear();

		// 销毁Command
		if (m_Command != null)
		{
			m_Command.Cancel();
			m_Command = null;
		}

		// 销毁Reader
		if (m_Reader != null)
		{
			m_Reader.Close();
			m_Reader = null;
		}

		// Connection置空
		if (m_Connection != null)
		{
			m_Connection = null;
		}
	}

	/// <summary>
	/// 清理SQLite的引用关系
	/// </summary>
	public void CleanSQLiteReference()
	{
		// 销毁Command
		if (m_Command != null)
		{
			m_Command.Dispose();
			m_Command = null;
		}

		// 销毁Reader
		if (m_Reader != null)
		{
			m_Reader.Close();
			m_Reader = null;
		}
	}

	/// <summary>
	/// 执行SQL命令
	/// </summary>
	/// <returns>The query.</returns>
	/// <param name="queryString">SQL命令字符串</param>
	public IDataReader ExecuteQuery(string queryString)
	{
		try
		{
			m_Command = m_Connection.CreateCommand();

			if (m_Reader != null && !m_Reader.IsClosed)
				m_Reader.Close();

			m_Command.CommandText = queryString;
			m_Reader = m_Command.ExecuteReader();
		}
		catch (Exception e)
		{
			Debug.LogError(SQLiteHelper.MakeDatabaseExceptionMessage(e.ToString()));
			this.CleanSQLiteReference();
		}

		return m_Reader;
	}

	/// <summary>
	/// 执行SQL命令
	/// </summary>
	/// <param name="queryString"></param>
	/// <param name="para"></param>
	/// <returns></returns>
	public IDataReader ExecuteQuery(string queryString, IDataParameter para)
	{
		try
		{
			m_Command = m_Connection.CreateCommand();

			if (m_Reader != null && !m_Reader.IsClosed)
				m_Reader.Close();

			m_Command.CommandText = queryString;

			m_Command.Parameters.Add(para);

			m_Reader = m_Command.ExecuteReader();
		}
		catch (Exception e)
		{
			Debug.LogError(SQLiteHelper.MakeDatabaseExceptionMessage(e.ToString()));


			this.CleanSQLiteReference();
		}

		return m_Reader;
	}

	/// <summary>
	/// 获取数据库链接字符窜
	/// </summary>
	/// <returns>链接字符窜</returns>
	private string GetConnectionString(string databaseFileName)
	{
		string connectionString = string.Empty;

		if (RuntimePlatform.WindowsEditor == Application.platform ||
			RuntimePlatform.WindowsPlayer == Application.platform ||
			RuntimePlatform.OSXEditor == Application.platform ||
			RuntimePlatform.OSXPlayer == Application.platform)
		{
			//connectionString = string.Format("data source=:{0}/{1}", Application.streamingAssetsPath, databaseFileName);
			connectionString = string.Format("URI=file:{0}/{1}", Application.streamingAssetsPath, databaseFileName);
		}
		else if (RuntimePlatform.Android == Application.platform) // 在android真机上运行
		{
			connectionString = string.Format("URI=file:{0}/{1}", Application.persistentDataPath, databaseFileName);
		}

		return connectionString;
	}

	/// <summary>
	/// 创建数据表
	/// </summary> +
	/// <returns>The table.</returns>
	/// <param name="tableName">数据表名</param>
	/// <param name="colNames">字段名</param>
	/// <param name="colTypes">字段名类型</param>
	public IDataReader CreateTable(string tableName, string[] colNames, string[] colTypes)
	{
		StringBuilder queryString = new StringBuilder();

		queryString.AppendFormat("CREATE TABLE {0}( {1} {2}", tableName, colNames[0], colTypes[0]);
		for (int i = 1; i < colNames.Length; i++)
		{
			queryString.AppendFormat(", {0} {1}", colNames[i], colTypes[i]);
		}

		queryString.Append(" )");
		return ExecuteQuery(queryString.ToString());
	}

	/// <summary>
	/// 读取整张数据表
	/// </summary>
	/// <returns>The full table.</returns>
	/// <param name="tableName">数据表名称</param>
	public IDataReader ReadFullTable(string tableName)
	{
		string queryString = "SELECT * FROM " + tableName;
		return ExecuteQuery(queryString);
	}

	/// <summary>
	/// 向指定数据表中插入数据
	/// </summary>
	/// <param name="tableName">数据表名称</param>
	/// <param name="values">插入的数组</param>
	/// <returns></returns>
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
		return ExecuteQuery(queryString.ToString());
	}

	/// <summary>
	/// 插入二进制数据
	/// </summary>
	/// <param name="tableName"></param>
	/// <param name="column"></param>
	/// <param name="content"></param>
	/// <returns></returns>
	public IDataReader InsertBlob(string tableName, string column, Byte[] content)
	{
		string queryString = "INSERT INTO " + tableName + " VALUES(" + column + ", @data)";

		IDataParameter dPara = new SqliteParameter("@data", DbType.Binary);

		dPara.Value = content;
		return ExecuteQuery(queryString, dPara);
	}

	/// <summary>
	/// 修改某个Blob字段的值
	/// </summary>
	/// <param name="tableName">表名</param>
	/// <param name="columnName">待查询的列名</param>
	/// <param name="content">要设置的值</param>
	/// <param name="key">判定条件用的字段</param>
	/// <param name="operation">判定条件符号，如 = AND OR</param>
	/// <param name="value">判定条件用的字段对应的值</param>
	/// <returns></returns>
	public IDataReader UpdateBlob(string tableName, string columnName, Byte[] content, string key, string operation, string value)
	{
		string queryString = string.Format("UPDATE {0} SET {1}=@data WHERE {2}{3}{4}", tableName, columnName, key, operation, value);
		IDataParameter dPara = new SqliteParameter("@data", DbType.Binary);
		dPara.Value = content;
		return ExecuteQuery(queryString, dPara);
	}

	/// <summary>
	/// 更新指定数据表内的数据
	/// </summary>
	/// <param name="tableName">数据表名称</param>
	/// <param name="colNames">要更新的字段名</param>
	/// <param name="colValues">所要改变的数值</param>
	/// <param name="key">关键字</param>
	/// <param name="operation">条件符号</param>
	/// <param name="value">关键字要符合的数值</param>
	/// <returns></returns>
	public IDataReader UpdateValues(string tableName, string[] colNames, string[] colValues, string key, string operation, string value)
	{
		//当字段名称和字段数值不对应时引发异常
		if (colNames.Length != colValues.Length)
		{
			throw new SqliteException("colNames.Length != colValues.Length");
		}

		StringBuilder queryString = new StringBuilder();

		queryString.AppendFormat("UPDATE {0} SET {1}={2}", tableName, colNames[0], colValues[0]);
		for (int i = 1; i < colValues.Length; i++)
		{
			queryString.AppendFormat(" , {0}={1}", colNames[i], colValues[i]);
		}
		queryString.AppendFormat(" WHERE {0}{1}{2}", key, operation, value);
		return ExecuteQuery(queryString.ToString());
	}

	/// <summary>
	/// 删除指定数据表内的数据（条件为“且（and）”）
	/// </summary>
	/// <param name="tableName">数据表名称</param>
	/// <param name="colNames">判定条件字段名</param>
	/// <param name="operations">条件符号</param>
	/// <param name="colValues">字段名对应的数据</param>
	/// 
	public IDataReader DeleteValuesOR(string tableName, string[] colNames, string[] operations, string[] colValues)
	{
		//当字段名称和字段数值不对应时引发异常
		if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
		{
			throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
		}

		StringBuilder queryString = new StringBuilder();

		queryString.AppendFormat("DELETE FROM {0} WHERE {1}{2}{3}", tableName, colNames[0], operations[0], colValues[0]);
		for (int i = 1; i < colValues.Length; i++)
		{
			queryString.AppendFormat(" OR {0}{1}{2}", colNames[i], operations[i], colValues[i]);
		}
		return ExecuteQuery(queryString.ToString());
	}

	/// <summary>
	/// 删除指定数据表内的数据（条件为“或（or）”）
	/// </summary>
	/// <param name="tableName">数据表名称</param>
	/// <param name="colNames">判定条件字段名</param>
	/// <param name="operations">条件符号</param>
	/// <param name="colValues">字段名对应的数据</param>
	/// <returns></returns>
	public IDataReader DeleteValuesAND(string tableName, string[] colNames, string[] operations, string[] colValues)
	{
		//当字段名称和字段数值不对应时引发异常
		if (colNames.Length != colValues.Length || operations.Length != colNames.Length || operations.Length != colValues.Length)
		{
			throw new SqliteException("colNames.Length!=colValues.Length || operations.Length!=colNames.Length || operations.Length!=colValues.Length");
		}

		StringBuilder queryString = new StringBuilder();

		queryString.AppendFormat("DELETE FROM {0} WHERE {1}{2}{3}", tableName, colNames[0], operations[0], colValues[0]);
		for (int i = 1; i < colValues.Length; i++)
		{
			queryString.AppendFormat(" AND {0}{1}{2}", colNames[i], operations[i], colValues[i]);
		}
		return ExecuteQuery(queryString.ToString());
	}

	/// <summary>
	/// 读取表格中的数据(条件为OR相连)
	/// </summary>
	/// <returns>IDataReader</returns>
	/// <param name="tableNames">所要读取的表</param>
	/// <param name="items">所需字段的名称</param>
	/// <param name="colNames">需要满足条件的字段</param>
	/// <param name="operations">具体条件符号</param>
	/// <param name="colValues">具体条件数值</param>
	public IDataReader ReadTableOR(string[] tableNames, string[] items, string[] colNames, string[] operations, string[] colValues)
	{
		//当字段名称和字段数值不对应时引发异常
		if (colNames.Length != operations.Length || colNames.Length != colValues.Length || operations.Length != colValues.Length)
		{
			throw new SqliteException("colNames.Length != operations.Length || colNames.Length != colValues.Length || operations.Length != colValues.Length");
		}

		StringBuilder queryString = new StringBuilder();

		queryString.AppendFormat("SELECT {0}", items[0]);
		for(int i = 1 ; i<items.Length ; i++)
		{
			queryString.AppendFormat(", {0}", items[i]);
		}

		queryString.AppendFormat(" FROM {0}", tableNames[0]);
		for (int i = 1; i < tableNames.Length; i++)
		{
			queryString.AppendFormat(", {0}", tableNames[i]);
		}

		queryString.AppendFormat(" WHERE {0} {1} {2}", colNames[0], operations[0], colValues[0]);
		for (int i = 1; i < colNames.Length; i++)
		{
			queryString.AppendFormat(" OR {0} {1} {2} ", colNames[i], operations[i], colValues[i]);
		}

		return ExecuteQuery(queryString.ToString());
	}

	/// <summary>
	/// 读取表格中的数据(条件为AND相连)
	/// </summary>
	/// <returns>IDataReader</returns>
	/// <param name="tableNames">所要读取的表</param>
	/// <param name="items">所需字段的名称</param>
	/// <param name="colNames">需要满足条件的字段</param>
	/// <param name="operations">具体条件符号</param>
	/// <param name="colValues">具体条件数值</param>
	public IDataReader ReadTableAND(string[] tableNames, string[] items, string[] colNames, string[] operations, string[] colValues)
	{
		//当字段名称和字段数值不对应时引发异常
		if (colNames.Length != operations.Length || colNames.Length != colValues.Length || operations.Length != colValues.Length)
		{
			throw new SqliteException("colNames.Length != operations.Length || colNames.Length != colValues.Length || operations.Length != colValues.Length");
		}

		StringBuilder queryString = new StringBuilder();

		queryString.AppendFormat("SELECT {0}", items[0]);
		for (int i = 1; i < items.Length; i++)
		{
			queryString.AppendFormat(", {0}", items[i]);
		}

		queryString.AppendFormat(" FROM {0}", tableNames[0]);
		for (int i = 1; i < tableNames.Length; i++)
		{
			queryString.AppendFormat(", {0}", tableNames[i]);
		}

		queryString.AppendFormat(" WHERE {0} {1} {2}", colNames[0], operations[0], colValues[0]);
		for (int i = 1; i < colNames.Length; i++)
		{
			queryString.AppendFormat(" AND {0} {1} {2} ", colNames[i], operations[i], colValues[i]);
		}

		return ExecuteQuery(queryString.ToString());
	}

	/// <summary>
	/// 查询字段自否存在
	/// </summary>
	/// <param name="tableName">要查的表名</param>
	/// <param name="columnName">要查的字段名</param>
	/// <returns></returns>
	public bool ExamineTableColumn(string tableName, string columnName)
	{
		string queryString = string.Format("SELECT {0} FROM {1}", columnName, tableName);
		IDataReader reader = ExecuteQuery(queryString);
		if (reader == null)
			return false;
		else
			return true;
	}

	/// <summary>
	/// 增加数据表字段
	/// </summary>
	/// <param name="tableName">表名</param>
	/// <param name="columnName">字段名</param>
	/// <param name="columnType">类型</param>
	public void AddTableColumn(string tableName, string columnName, SQLiteDataType columnType)
	{
		string dy = string.Empty;
		switch(columnType)
		{
		case SQLiteDataType.DT_NULL: 
			dy = "NULL";
			break;
		case SQLiteDataType.DT_INTEGER: 
			dy = "INTEGER";
			break;
		case SQLiteDataType.DT_REAL:
			dy = "REAL";
			break;
		case SQLiteDataType.DT_TEXT:
			dy = "TEXT";
			break;
		case SQLiteDataType.DT_BLOB:
			dy = "BLOB";
			break;
		}

		string queryString = string.Format("ALTER TABLE {0} ADD COLUMN {1} {2};", tableName, columnName, dy);
		ExecuteQuery(queryString);
	}
	#endregion

	#region Static 文件转移操作
	/// <summary>
	/// 从StreamAssets目录下导出sqlite database文件到Persistent目录，然后
	/// android平台特有的机制，在lua开发阶段使用,
	/// </summary>
	public IEnumerator ReleaseDatabaseFromStreamingAssetsToPersistent()
	{
		if (RuntimePlatform.Android == Application.platform)
		{
			// 首先读取到StreamingAssets目录下llist.txt的目录和文件名，然后依次拷贝和生成
			string dbFilePath = string.Empty;
			string dbFileInPersistent = string.Empty;

			// 遍历所要移动的文件
			for (int i = 0; i < m_DatabaseFiles.Length; i++)
			{                    
				dbFilePath = string.Format("{0}/{1}", Application.streamingAssetsPath, m_DatabaseFiles[i]);
				dbFileInPersistent = string.Format("{0}/{1}", Application.persistentDataPath, m_DatabaseFiles[i]);

				// 如果player数据存盘文件已经在持久化目录中存在了，就不拷贝了
				if (m_DatabaseFiles[i].Equals(CommonData.playerPrefsDB) && File.Exists(dbFileInPersistent))
					continue;

				Debug.LogFormat("Read database file {0} Begin", dbFilePath);
				FileStream dbFS = null;

				WWW dbListGetter = new WWW(dbFilePath);
				yield return dbListGetter;

				if (!string.IsNullOrEmpty(dbListGetter.error)) // 能取出文件的话才执行操作
				{
					Debug.LogError(SQLiteHelper.MakeDatabaseExceptionMessage(dbListGetter.error));
					yield break;
				}

				while (dbListGetter.isDone == false)  // 等待下载完成再继续
					yield return null;

				// 取出原始文件字节流，保存到 persistent 目录下
				dbFS = null;

				try
				{
					// 删旧建新
					if (File.Exists(dbFileInPersistent))
						File.Delete(dbFileInPersistent);

					dbFS = File.Create(dbFileInPersistent);
				}
				catch (System.Exception ex)
				{
					Debug.LogErrorFormat("SQLite Exception : Load datebase file {0} from streaming assets directory error\n,Reason is:\n{1}",
						dbFilePath, ex.ToString());
					yield break; // 读取文件异常了就不处理，直接报错，游戏逻辑上不允许不成功！！！
				}

				Byte[] fileContent = dbListGetter.bytes;
				dbFS.Write(fileContent, 0, fileContent.Length);
				dbFS.Close();

				Debug.LogFormat("SQLiteHelper Read database file {0} success", dbFilePath);                   
			}
			yield return null;
		}
		 
		else
		{
			m_ReleasedFromStreamingAssets = true;
			yield return null;
		}

	}
	#endregion
}