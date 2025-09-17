using System.ComponentModel.DataAnnotations;
using vehicle_rental.api.Validators;

namespace vehicle_rental.api.Models;

public class LicenseImageUploadModel
{
    [Required(ErrorMessage = "Arquivo de imagem é obrigatório")]
    [AllowedFileExtensions(".jpg", ".jpeg", ".png", ".gif", ErrorMessage = "Apenas arquivos de imagem são permitidos")]
    [MaxFileSize(5, ErrorMessage = "O arquivo deve ter no máximo 5 MB")]
    public IFormFile LicenseImage { get; set; } = null!;
}
