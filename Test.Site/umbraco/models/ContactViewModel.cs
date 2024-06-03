using System.ComponentModel.DataAnnotations;

namespace Test.Site.umbraco.models;

public class ContactViewModel
{
    [Required]
    public string Name { get; set; }
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    [Required]
    public string Phone { get; set; }
    [Required]
    public string MessageBody { get; set; }
}
