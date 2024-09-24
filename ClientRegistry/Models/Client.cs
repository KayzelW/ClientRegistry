using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ClientRegistry.Models;

public class Client
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
    [StringLength(Program.MaxCharsInn)]
    public string Inn { get; set; }
    public string Name { get; set; }
    public ClientType Type { get; set; }
    
    [Column(TypeName = "timestamp(6)")] public DateTime? AddDate { get; set; } = DateTime.Now;
    [Column(TypeName = "timestamp(6)")] public DateTime? UpdateDate { get; set; } = DateTime.Now;

    public ICollection<Founder> Founders { get; set; } = new List<Founder>();
}