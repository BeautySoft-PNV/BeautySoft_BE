using System.Text.Json.Serialization;

namespace BeautySoftBE.Models
{
	public class RoleModel
	{
		public int Id { get; set; }
		public string Name { get; set; }
		
		[JsonIgnore]
		public ICollection<UserModel?> Users { get; set; } = new List<UserModel?>();
	}
}