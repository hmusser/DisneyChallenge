using System.ComponentModel.DataAnnotations;

namespace DisneyChallenge.Validaciones
{
    public class TipoArchivoValidacion : ValidationAttribute
    {
        private readonly string[] tiposValidos;

        public TipoArchivoValidacion(string[] tiposValidos)
        {
            this.tiposValidos = tiposValidos;
        }

        public TipoArchivoValidacion(GrupoTipoArchivo grupoTipoArchivo)
        {
            if(grupoTipoArchivo == GrupoTipoArchivo.Imagen)
            {
                tiposValidos = new string[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/tiff", "image/bmp" };
            }
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
            //Verificamos si el archivo enviado es de uno de los tipos soportados.
            if (!tiposValidos.Contains(formFile.ContentType)){
                return new ValidationResult($"El tipo  del archivo debe ser uno de los siguientes: {string.Join(", ", tiposValidos)}");
            }
            //Si alcanzamos esta instrucción está todo OK.
            return ValidationResult.Success;

        }
    }
}
