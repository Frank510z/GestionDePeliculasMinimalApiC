namespace AnimalApiPeliculas.Validaciones {
    public static class Utilidades {
        public static string CampoRequeridoMensaje = "El campo Nombre es requerido o {PropertyName}";
        public static string CampoMaximoDeCacteresMensaje = "El Campo {PropertyName} debe tener menos de {MaxLength} Caracteres";
        public static string PrimeraLetraMayusculaMensaje = "En El Campo {PropertyName} Debe Comenzar Con Mayuscula";
        public static string EmailMensaje = "El campo {PropertyName} debe ser un Email valido";
        public static string FechaMayorOIgualMensaje(DateTime fechaMinima) {
            return "El Campo {PropertyName} debe ser mayor o posterior a " + fechaMinima.ToString("yyyy-MM-dd");
        }

        public static bool PrimeraLetraEnMayuscula(string valor) {
            if (string.IsNullOrEmpty(valor)) { //Si esta vacio/null retorna true ya que esa validacion la hace el validator.NotEmpty
                return true;
            }
            var primeraLetra = valor[0].ToString(); //toma la primera letra del valor
            return primeraLetra == primeraLetra.ToUpper(); //Retornara True/False si si es igual o no a su contraria mayuscula 
        }
    }
}
