using Npgsql;
namespace robot_controller_api.Persistence;

public interface IMapDataAccess
{
  Boolean CheckEntry(int id);
  Boolean CheckName(string name);
  int DeleteMap(int id);
  List<Map> GetMaps();
  List<Map> GetSquareMaps();
  int InsertMap(Map map);
  Boolean IsOnMap(int id, int x, int y);
  int UpdateMap(int mapId, Map newMap);
}

public class MapADO : IMapDataAccess
{
  private const string CONNECTION_STRING = "Host=localhost;Username=postgres;Password=12345;Database=sit331";

  public List<Map> GetMaps()
  {
    var maps = new List<Map>();
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();
    using var cmd = new NpgsqlCommand("SELECT * FROM map", conn);
    using var dr = cmd.ExecuteReader();
    while (dr.Read())
    {
      var id = (int)dr["id"];
      int rows = (int)dr["rows"];
      int columns = (int)dr["columns"];
      string desc = dr["description"] == null ? null : dr["description"] as string;
      string name = (string)dr["name"];
      DateTime createdDate = (DateTime)dr["created_date"];
      DateTime modifiedDate = (DateTime)dr["modified_date"];

      maps.Add(new Map(id, columns, rows, name, createdDate, modifiedDate, desc));
    }
    return maps;
  }

  public List<Map> GetSquareMaps()
  {
    var maps = new List<Map>();
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE columns=rows", conn);
    using var dr = cmd.ExecuteReader();
    while (dr.Read())
    {
      var id = (int)dr["id"];
      int rows = (int)dr["rows"];
      int columns = (int)dr["columns"];
      string desc = dr["description"] == null ? null : dr["description"] as string;
      string name = (string)dr["name"];
      DateTime createdDate = (DateTime)dr["created_date"];
      DateTime modifiedDate = (DateTime)dr["modified_date"];

      maps.Add(new Map(id, columns, rows, name, createdDate, modifiedDate, desc));
    }
    return maps;
  }

  public int UpdateMap(int mapId, Map newMap)
  {
    int id = -1;
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id=" + mapId, conn);
    using var dr = cmd.ExecuteReader();

    if (dr.Read())
    {
      var _id = (int)dr["id"];
      int _rows = (int)dr["rows"];
      int _columns = (int)dr["columns"];
      string _desc = dr["description"] == null ? null : dr["description"] as string;
      string _name = (string)dr["name"];
      DateTime _createdDate = (DateTime)dr["created_date"];
      DateTime _modifiedDate = (DateTime)dr["modified_date"];

      dr.Close();

      int columns = newMap.Columns == 0 ? _columns : newMap.Columns;
      int rows = newMap.Rows == 0 ? _rows : newMap.Rows;
      string desc = newMap.Description == null ? null : newMap.Description;
      string name = newMap.Name;
      DateTime modifiedDate = (DateTime) DateTime.Now;

      using var cmd2 = new NpgsqlCommand("UPDATE map SET name= \'" + name + "\', " +
              "columns=" + columns + "," +
              "rows=" + rows + "," +
              "description=\'" + desc + "\'," +
              "modified_date=\'" + modifiedDate + "\'" +
              " WHERE id=" + mapId, conn);

      object a = cmd2.ExecuteScalar();
      if (a != null) {
        id = (int)a;
      }
    }
    return id;
  }

  public int InsertMap(Map map)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();
    int id = map.Id;
    int rows = map.Rows;
    int columns = map.Columns;
    string desc = map.Description == null ? null : map.Description;
    string name = map.Name;
    DateTime createdDate = (DateTime) DateTime.Now;
    DateTime modifiedDate = (DateTime) DateTime.Now;

    using var cmd = new NpgsqlCommand("INSERT INTO map (id, columns, rows, name, description, created_date, modified_date)" +
        " VALUES(DEFAULT," +
        columns + "," +
        rows + "," +
        "\'" + name + "\'," +
        "\'" + desc + "\'," +
        "\'" + createdDate + "\'," +
        "\'" + modifiedDate + "\')"
        , conn);

    int _id = -1;
    object a = cmd.ExecuteScalar();
      if (a != null) {
        _id = (int)a;
      }

    return _id;
  }

  public int DeleteMap(int id)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd2 = new NpgsqlCommand("DELETE FROM map WHERE id=" + id, conn);

    int _id = -1;
    object a = cmd2.ExecuteScalar();
      if (a != null) {
        _id = (int)a;
      }

    return _id;
  }

  public Boolean CheckEntry(int id)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id=" + id, conn);
    using var dr = cmd.ExecuteReader();

    if (dr.Read())
    {
      return true;
    }
    return false;
  }

  public Boolean CheckName(string name)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE name=" + "\'" + name + "\'", conn);
    using var dr = cmd.ExecuteReader();

    if (dr.Read())
    {
      return true;
    }
    return false;
  }

  public Boolean IsOnMap(int id, int x, int y)
  {
    using var conn = new NpgsqlConnection(CONNECTION_STRING);
    conn.Open();

    using var cmd = new NpgsqlCommand("SELECT * FROM map WHERE id=" + id, conn);
    using var dr = cmd.ExecuteReader();

    int columns = 0;
    int rows = 0;

    while (dr.Read())
    {
      columns = (int)dr["columns"];
      rows = (int)dr["rows"];
    }

    if (x < columns && y < rows) return true;
    else return false;
  }
}
