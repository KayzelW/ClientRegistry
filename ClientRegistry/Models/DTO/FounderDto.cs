namespace ClientRegistry.Models.DTO;

public class FounderDto
{
    public Founder Founder { get; set; }
    public List<string>? ClientsInn { get; set; }
    public string? IndividualInn { get; set; }
}

public class FounderPutDto
{
    public Founder Founder { get; set; }
    public List<string>? removeClientsInn { get; set; }
    public List<string>? addClientsInn { get; set; }
}