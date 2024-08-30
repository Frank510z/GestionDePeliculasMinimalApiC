using Microsoft.IdentityModel.Tokens;

namespace AnimalApiPeliculas.Utilidades {
    public static class Llaves {

        public const string IssuerPropio = "nuestra-app";
        private const string SeccionLaves = "Authentication:Schemes:Bearer:SigningKeys";
        private const string SeccionLaves_Emisor = "Issuer";
        private const string SeccionLaves_Valor = "Value";

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration) => ObtenerLlave(configuration, IssuerPropio);

        public static IEnumerable<SecurityKey> ObtenerLlave(IConfiguration configuration, string issuer) {

            var signingKey = configuration.GetSection(SeccionLaves).GetChildren().SingleOrDefault(llave => llave[SeccionLaves_Emisor] == issuer); //Obteniendo las llaves que tengan valor igual al issure 

            if (signingKey is not null && signingKey[SeccionLaves_Valor] is string valorLave) { // si no es nulo y tenemos un valor 
                yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLave)); // retorna las distintas llaves que encontrmos en nuestro secret.json
            }
        }

        public static IEnumerable<SecurityKey> ObtenerTodasLasLlaves(IConfiguration configuration) {

            var signingKeys = configuration.GetSection(SeccionLaves).GetChildren(); //Obteniendo las llaves que tengan valor igual al issure 

            foreach (var signingKey in signingKeys) {

                if (signingKey[SeccionLaves_Valor] is string valorLave) { // si no es nulo y tenemos un valor 
                    yield return new SymmetricSecurityKey(Convert.FromBase64String(valorLave)); // retorna las distintas llaves que encontrmos en nuestro secret.json
                }

            }



        }


    }
}
