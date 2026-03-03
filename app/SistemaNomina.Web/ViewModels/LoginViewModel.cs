using System.ComponentModel.DataAnnotations;

public class LoginViewModel
{
    [Required]
    public string Usuario { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Clave { get; set; } = string.Empty;

    public bool Recordarme { get; set; }
}