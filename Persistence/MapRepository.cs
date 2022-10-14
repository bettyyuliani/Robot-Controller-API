using Npgsql;
namespace robot_controller_api.Persistence;

public class MapRepository : IMapDataAccess, IRepository
{
  private IRepository _repo => this;

  public Boolean CheckEntry(int id)
  {
    var entries = _repo.ExecuteReader<Map>("SELECT * FROM map WHERE id=" + id, null);

    if (entries.Count > 0) return true;
    else return false;
  }

  public Boolean CheckName(string name)
  {
    var entries = _repo.ExecuteReader<Map>("SELECT * FROM map WHERE name=" + "\'" + name + "\'", null);

    if (entries.Count > 0) return true;
    else return false;
  }

   public int DeleteMap(int id)
  {
    var result = _repo.ExecuteReader<Map>("DELETE FROM map WHERE id=" + id + "RETURNING id", null).Single();
    return result.Id;
  }

  public List<Map> GetMaps()
  {
    var maps = _repo.ExecuteReader<Map>("SELECT * FROM public.map;");
    return maps;
  }

  public List<Map> GetSquareMaps()
  {
    var maps = _repo.ExecuteReader<Map>("SELECT * FROM public.map WHERE rows = columns;");
    return maps;
  }

  public int InsertMap(Map newMap)
  {

        int rows = newMap.Rows > 2 && newMap.Rows < 100 ? newMap.Rows : 25;
        int columns = newMap.Columns > 2 && newMap.Columns < 100 ? newMap.Columns : 25;

        var sqlParams = new NpgsqlParameter[]{
                      new("id", newMap.Id),
                      new("columns", columns),
                      new("rows", rows),
                      new("name", newMap.Name),
                      new("description", newMap.Description ??
                      (object)DBNull.Value)
            };

    var result = _repo.ExecuteReader<Map>("INSERT INTO map (id, columns, rows, name, description, created_date, modified_date) VALUES(default, @columns, @rows, @name, @description, current_timestamp, current_timestamp) RETURNING id;", sqlParams).Single();
    return result.Id;
  }

  public Boolean IsOnMap(int id, int x, int y)
  {
    List<Map> maps = _repo.ExecuteReader<Map>("SELECT * FROM map WHERE id=" + id, null);

    int columns = maps[0].Columns;
    int rows = maps[0].Rows;

    if (x < columns && y < rows) return true;
    else return false;
  }

  public int UpdateMap(int id, Map updatedMap)
  {
    List<Map> maps = _repo.ExecuteReader<Map>("SELECT * FROM map WHERE id=" + id);

    int rows = updatedMap.Rows == 0 ? maps[0].Rows : updatedMap.Rows;
    int columns = updatedMap.Columns == 0 ? maps[0].Columns : updatedMap.Columns;

    var sqlParams = new NpgsqlParameter[]{
                new("id", id),
                new("columns", columns),
                new("rows", rows),
                new("name", updatedMap.Name),
                new("description", updatedMap.Description ??
                (object)DBNull.Value)
      };

    var result = _repo.ExecuteReader<Map>(
      "UPDATE map SET name=@name, description=@description, " +
      "rows=@rows, columns=@columns, modified_date=current_timestamp WHERE id=@id RETURNING id;", sqlParams).Single();
      return result.Id;
  }
}
