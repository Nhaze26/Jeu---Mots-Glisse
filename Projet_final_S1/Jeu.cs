using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_final_S1
{
    internal class Jeu
    {
        private Dictionnaire dictionnaire;
        private Plateau plateau;
        private Joueur[] joueurs;

        // Temps par défaut, modifiable
        private int tempsParTour = 30;   // 30 secondes par tour
        private int tempsPartie = 120;  //  2 minutes pour la partie totale

        private const string NOM_FICHIER_LETTRES = "Lettre.txt";      // Correspond à votre fichier
        private const string NOM_FICHIER_DICO = "Mots_Français.txt";  // Correspond à votre fichier
        private const string NOM_FICHIER_SAUVEGARDE = "Save.csv";      // Fichier de sauvegarde standard

        public Jeu()
        {
            // Tente de trouver le chemin du dictionnaire
            string cheminDico = TrouverFichier(NOM_FICHIER_DICO);

            if (cheminDico == null)
            {
                Console.WriteLine($"[ERREUR FATALE] Impossible de trouver '{NOM_FICHIER_DICO}'. Le jeu ne peut pas démarrer.");
               
            }
            else
            {
                dictionnaire = new Dictionnaire(cheminDico);
                if (dictionnaire == null)
                {
                    Console.WriteLine("[ERREUR] Problème lors du chargement ou du tri du dictionnaire.");
                }
            }

            joueurs = new Joueur[2];
        }

        // --- GESTION DES CHEMINS ---
        /// Recherche le fichier de données en remontant les dossiers si nécessaire.

        private string TrouverFichier(string nomFichier)
        {
            string exe = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string dossier = Path.GetDirectoryName(exe);

            // Remonte jusqu'à 5 niveaux (pour inclure les dossiers bin/Debug)
            for (int i = 0; i < 5; i++)
            {
                string test = Path.Combine(dossier, nomFichier);
                if (File.Exists(test))
                {
                    return test;
                }
                dossier = Directory.GetParent(dossier)?.FullName;
                if (dossier == null) break;
            }
            return null;
        }

        // --- BOUCLE PRINCIPALE ET MENU ---

        public void LancerJeu()
        {
            bool continuer = true;

            while (continuer)
            {
                Console.Clear();
                Console.WriteLine("╔═════════════════════════════════╗");
                Console.WriteLine("║      BIENVENUE À MOTS GLISSÉS   ║");
                Console.WriteLine("╚═════════════════════════════════╝");
                Console.WriteLine("1. Nouvelle partie (Plateau aléatoire)");
                Console.WriteLine($"2. Continuer partie (Charger {NOM_FICHIER_SAUVEGARDE})");
                Console.WriteLine("3. Quitter");
                Console.Write("Votre choix : ");

                string choix = Console.ReadLine();

                if (choix == "1")
                {
                    InitJoueurs();
                    string cheminLettres = TrouverFichier(NOM_FICHIER_LETTRES);
                    if (cheminLettres == null)
                    {
                        Console.WriteLine($"Erreur, {NOM_FICHIER_LETTRES} introuvable. Appuyez sur Entrée.");
                        Console.ReadKey();
                    }
                    else
                    {
                        // Crée un nouveau plateau aléatoire basé sur Lettre.txt
                        plateau = new Plateau(cheminLettres);
                        BoucleDeJeu();
                    }
                }
                else if (choix == "2")
                {
                    InitJoueurs();
                    string cheminSave = TrouverFichier(NOM_FICHIER_SAUVEGARDE);
                    string cheminLettres = TrouverFichier(NOM_FICHIER_LETTRES);
                    if (cheminSave == null || cheminLettres == null)
                    {
                        Console.WriteLine("Erreur : Fichier de sauvegarde ou contraintes de lettres introuvables. Appuyez sur Entrée.");
                        Console.ReadKey();
                    }
                    else
                    {
                        // Charge le plateau depuis le fichier et charge les contraintes
                        plateau = new Plateau(cheminSave, true);
                        plateau.ChargerContraintesLettres(cheminLettres);
                        BoucleDeJeu();
                    }
                }
                else if (choix == "3")
                {
                    continuer = false;
                }
                else
                {
                    Console.WriteLine("Choix invalide. Appuyez sur Entrée.");
                    Console.ReadKey();
                }
            }
        }

        private void InitJoueurs()
        {
            Console.WriteLine("--- Initialisation des Joueurs ---");
            Console.Write("Nom du Joueur 1 : ");
            joueurs[0] = new Joueur(Console.ReadLine());
            Console.Write("Nom du Joueur 2 : ");
            joueurs[1] = new Joueur(Console.ReadLine());
        }

        private void BoucleDeJeu()
        {
            if (plateau == null || dictionnaire == null) return;

            DateTime finPartie = DateTime.Now.AddSeconds(tempsPartie);
            int indexJoueur = 0;

            // La partie se termine si le temps est écoulé OU si le plateau est vide
            while (DateTime.Now < finPartie && !plateau.EstVide())
            {
                Joueur joueurActuel = joueurs[indexJoueur];
                DateTime finTour = DateTime.Now.AddSeconds(tempsParTour);
                bool tourTermine = false;

                // Le tour se termine si le joueur trouve un mot, passe, ou le temps s'écoule.
                while (!tourTermine && DateTime.Now < finTour && !plateau.EstVide())
                {
                    Console.Clear();
                    Console.WriteLine("--- GRILLE DE JEU ---");
                    Console.WriteLine(plateau.ToString());

                    Console.WriteLine("\n--- SCORES ---");
                    Console.WriteLine($" [1] {joueurs[0].Nom} : {joueurs[0].Score} points");
                    Console.WriteLine($" [2] {joueurs[1].Nom} : {joueurs[1].Score} points");

                    int tempsTotalRestant = (int)(finPartie - DateTime.Now).TotalSeconds;
                    int tempsTourRestant = (int)(finTour - DateTime.Now).TotalSeconds;
                    Console.WriteLine($"\nTEMPS PARTIE : {tempsTotalRestant}s | TEMPS TOUR {joueurActuel.Nom} : {tempsTourRestant}s");

                    Console.WriteLine("\n----------------------------------------------------------------");
                    Console.Write($"C'est à {joueurActuel.Nom} de jouer. Entrez un mot (ou PASS pour passer) : ");

                    string saisie = Console.ReadLine();
                    string mot = saisie.ToUpper();

                    if (DateTime.Now >= finTour)
                    {
                        Console.WriteLine("Temps du tour écoulé !");
                        tourTermine = true;
                        break;
                    }

                    if (mot == "PASS")
                    {
                        tourTermine = true;
                    }
                    else if (mot.Length >= 2)
                    {
                        if (joueurActuel.Contient(mot))
                        {
                            Console.WriteLine("-> Mot déjà trouvé par ce joueur ! Appuyez sur Entrée.");
                            Console.ReadKey();
                        }
                        else if (dictionnaire.RechDichoRecursif(mot))
                        {
                            CheminMot result = plateau.Recherche_Mot(mot);

                            if (result != null)
                            {
                                int scoreObtenu = CalculerScoreMot(result.MotTrouve);

                                Console.WriteLine($"\nBRAVO ! Mot valide et trouvé sur le plateau. Score : {scoreObtenu} points.");

                                joueurActuel.Add_Mot(mot);
                                joueurActuel.Add_Score(scoreObtenu);
                                plateau.MAJ_Plateau(result); // Glissement du plateau

                                Console.WriteLine("Appuyez sur Entrée pour passer au joueur suivant...");
                                Console.ReadLine();
                                tourTermine = true;
                            }
                            else
                            {
                                Console.WriteLine("-> Mot valide, mais introuvable sur le plateau (doit partir de la ligne de base). Appuyez sur Entrée.");
                                Console.ReadKey();
                            }
                        }
                        else
                        {
                            Console.WriteLine("-> Mot inconnu au dictionnaire. Appuyez sur Entrée.");
                            Console.ReadKey();
                        }
                    }
                    else
                    {
                        Console.WriteLine("-> Mot trop court. Appuyez sur Entrée.");
                        Console.ReadKey();
                    }
                }

                // Change de joueur pour le tour suivant
                if (tourTermine)
                {
                    indexJoueur = (indexJoueur + 1) % 2;
                }
            }
            AfficherFinDePartie();
        }

        /// Calcule le score d'un mot : Somme des poids des lettres * Longueur du mot.

        private int CalculerScoreMot(string mot)
        {
            if (plateau == null) return 0;

            int sommePoids = 0;
            // Utilise la méthode GetPoids de Plateau (qui elle-même gère le ToUpper et le dictionnaire de contraintes)
            foreach (char lettre in mot.ToUpper())
            {
                sommePoids += plateau.GetPoids(lettre);
                //Console.WriteLine(plateau.GetPoids(lettre)); 
            }

            // Pondération par la longueur 
            return sommePoids;
        }


        private void AfficherFinDePartie()
        {
            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine("       FIN DE LA PARTIE !        ");
            Console.WriteLine("=================================");

            Console.WriteLine($"\n{joueurs[0].Nom} | Score : {joueurs[0].Score}");
            Console.WriteLine($"Mots trouvés : {string.Join(", ", joueurs[0].GetMotsTrouves().OrderBy(m => m))}");
            Console.WriteLine("\n---");
            Console.WriteLine($"{joueurs[1].Nom} | Score : {joueurs[1].Score}");
            Console.WriteLine($"Mots trouvés : {string.Join(", ", joueurs[1].GetMotsTrouves().OrderBy(m => m))}");

            Console.WriteLine("\n--- RÉSULTAT FINAL ---");
            if (joueurs[0].Score > joueurs[1].Score)
                Console.WriteLine($"FÉLICITATIONS, le VAINQUEUR est : {joueurs[0].Nom} !");
            else if (joueurs[1].Score > joueurs[0].Score)
                Console.WriteLine($"FÉLICITATIONS, le VAINQUEUR est : {joueurs[1].Nom} !");
            else
                Console.WriteLine("MATCH NUL ! Quel suspense.");

            Console.WriteLine("\nAppuyez sur Entrée pour revenir au menu...");
            Console.ReadKey();
        }
    }
}
