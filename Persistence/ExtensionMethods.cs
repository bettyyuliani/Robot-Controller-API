using FastMember;
using Npgsql;
using System.Globalization;
namespace robot_controller_api.Persistence;

  public static class ExtensionMethods
  {
    public static void MapTo<T>(this NpgsqlDataReader dr, T entity)
    {
      if (entity == null) throw new ArgumentNullException(nameof(entity));
      var fastMember = TypeAccessor.Create(entity.GetType()); // get class that will be mapped into database columns later, creates fast type accessor to iterate through properties
      var props = fastMember.GetMembers().Select(x => x.Name).ToHashSet(StringComparer.OrdinalIgnoreCase); // get all properties and put it in hashset
      for (int i = 0; i < dr.FieldCount; i++)
      {
        var prop = props.FirstOrDefault(x => x.Equals(dr.GetName(i).Replace("_", string.Empty), StringComparison.OrdinalIgnoreCase));
        fastMember[entity, prop] = dr.IsDBNull(i) ? null : dr.GetValue(i);
      }
    }
  }
