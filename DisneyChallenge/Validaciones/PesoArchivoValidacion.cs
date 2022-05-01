using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.Validaciones
{
    public class PesoArchivoValidacion : ValidationAttribute
    {
        private readonly int pesoMaximoEnMegabytes;

        //En el Constructor recibiremos el tamaño máximo permitido del archivo en Megabytes
        public PesoArchivoValidacion(int PesoMaximoEnMegabytes)
        {
            pesoMaximoEnMegabytes = PesoMaximoEnMegabytes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            IFormFile formFile = value as IFormFile;
            if (formFile == null)
            {
                return ValidationResult.Success;
            }
            //Length está en bytes
            if(formFile.Length > pesoMaximoEnMegabytes * 1024 * 1024)
            {
                return new ValidationResult($"El peso del archivo no debe ser mayor a {pesoMaximoEnMegabytes}mb");
            }
            
            //Si alcanzamos esta instrucción está todo OK.
            return ValidationResult.Success;

        }
    }
}
