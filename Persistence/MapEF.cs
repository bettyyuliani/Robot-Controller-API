namespace robot_controller_api.Persistence;
public class MapEF : RobotContext, IMapDataAccess
{
  private RobotContext context => this;

  public Boolean CheckEntry(int id)
  {
    Map map = context.Maps.FirstOrDefault(d=>d.Id==id);
    if (map != null)
    {
      return true;
    }
    return false;
  }

  public Boolean CheckName(string name)
  {
    Map map = context.Maps.FirstOrDefault(d=>d.Name==name);
    if (map != null)
    {
      return true;
    }
    return false;
  }

   public int DeleteMap(int id)
  {
    Map map = context.Maps.FirstOrDefault(d=>d.Id==id);
    context.Remove(map);
    context.SaveChanges();
    return map.Id;
  }

  public List<Map> GetMaps()
  {
    var maps = context.Maps.ToList();
    return maps;
  }

  public List<Map> GetSquareMaps()
  {
    var maps = context.Maps.Where(d=>d.Columns==d.Rows).ToList();
    return maps;
  }

  public int InsertMap(Map newMap)
  {
    Map map = newMap;
    map.CreatedDate = System.DateTime.Now;
    map.ModifiedDate = System.DateTime.Now;

    context.Maps.Add(map);
    context.SaveChanges();

    return map.Id;
  }

  public Boolean IsOnMap(int id, int x, int y)
  {
    Map map = context.Maps.FirstOrDefault(d=>d.Id==id);

    int columns = map.Columns;
    int rows = map.Rows;

    if (x < columns && y < rows) return true;
    else return false;
  }

  public int UpdateMap(int id, Map updatedMap)
  {
    Map map = context.Maps.FirstOrDefault(d=>d.Id==id);
    if (map != null)
    {
      if (updatedMap.Name != null) map.Name = updatedMap.Name;
      if (updatedMap.Description != null) map.Description = updatedMap.Description;
      map.ModifiedDate = System.DateTime.Now;
      if (updatedMap.Columns != 0) map.Columns = updatedMap.Columns;
      if (updatedMap.Rows != 0)map.Rows = updatedMap.Rows;
      context.SaveChanges();
    }
    return map.Id;
  }
}
