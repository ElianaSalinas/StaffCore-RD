using System.ComponentModel.DataAnnotations;

namespace StaffCore_RD.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string Nombre { get; set; } = string.Empty;  // Nombre completo

        [Required(ErrorMessage = "La cédula es obligatoria.")]
        [RegularExpression(@"^\d{3}-\d{7}-\d{1}$", ErrorMessage = "Formato de cédula inválido. Use: 001-0000000-0")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = string.Empty;  // Formato: 001-0000000-0

        [Required(ErrorMessage = "El cargo es obligatorio.")]
        [Display(Name = "Cargo")]
        public string Cargo { get; set; } = string.Empty;  // Ej: Analista de Sistemas

        [Required(ErrorMessage = "El departamento es obligatorio.")]
        [Display(Name = "Departamento")]
        public string Departamento { get; set; } = string.Empty;  // Tecnología / RRHH / Finanzas / Operaciones

        [Required(ErrorMessage = "El salario es obligatorio.")]
        [Range(23223, double.MaxValue, ErrorMessage = "Mínimo RD$23,223")]
        [Display(Name = "Salario (RD$)")]
        public decimal Salario { get; set; }

        [Display(Name = "Fecha de ingreso")]
        public DateTime FechaIngreso { get; set; }

        [Display(Name = "Activo")]
        public bool Activo { get; set; } = true;
    }
}
