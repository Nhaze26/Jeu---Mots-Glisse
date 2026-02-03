namespace Projet_final_S1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // 1. Création de l'instance du jeu
                Jeu jeu = new Jeu();

                // 2. Lancement de la boucle principale du jeu (menu, parties, etc.)
                jeu.LancerJeu();
            }
            catch (Exception ex)
            {
                // Gestion générale des erreurs non capturées (utile pour le débogage)
                Console.Clear();
                Console.WriteLine("╔═════════════════════════════════╗");
                Console.WriteLine("║        ERREUR CRITIQUE          ║");
                Console.WriteLine("╚═════════════════════════════════╝");
                Console.WriteLine($"Une erreur inattendue a fait planter l'application : {ex.Message}");
                Console.WriteLine("Appuyez sur Entrée pour quitter...");
                Console.ReadKey();
            }
        }
    }
}
