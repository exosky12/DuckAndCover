namespace Models.Exceptions
{
    public class ErrorHandler
    {
        private readonly int _ErrorCode;

        public ErrorHandler(int errorCode)
        {
            _ErrorCode = errorCode;
        }

        public string Handle()
        {
            return _ErrorCode switch
            {
                1 => "Erreur : La carte ne peut pas être jouée.",
                2 => "Erreur : La carte ne peut pas être recouverte.",
                3 => "Erreur : La carte ne peut pas être déplacée.",
                4 => "Erreur : Le joueur n'a pas de carte à jouer.",
                5 => "Erreur : Le joueur a déjà joué.",
                6 => "Erreur : Le joueur a déjà passé son tour.",
                7 => "Erreur : Le joueur n'a pas de carte à recouvrir.",
                _ => "Erreur inconnue."
            };
        }
    }
}