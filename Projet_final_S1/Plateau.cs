using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_final_S1
{
    // Structure de données légère pour retourner le chemin d'un mot trouvé.
    public class CheminMot
    {
        public string MotTrouve { get; set; }
        public List<Position> Coordonnees { get; set; } = new List<Position>();
    }

    public class Position
    {
        public int Ligne { get; private set; }
        public int Colonne { get; private set; }

        public Position(int ligne, int colonne)
        {
            this.Ligne = ligne;
            this.Colonne = colonne;
        }

        public static bool Compare(Position p1, Position p2)
        {
            if (p1 == null || p2 == null) return false;
            return p1.Ligne == p2.Ligne && p1.Colonne == p2.Colonne;
        }
    }

    public class Plateau
    {
        private char[,] grille;
        private int lignes = 8; // Basé sur l'exemple du sujet
        private int colonnes = 8; // Basé sur l'exemple du sujet
        private static Random r = new Random(); // Instancié une seule fois

        // Contient les données de Lettres.txt : {Lettre, (MaxOccurrences, Poids)}
        private Dictionary<char, (int max, int poids)> contraintesLettres = new Dictionary<char, (int, int)>();

        // Propriétés imposées ou nécessaires
        public int NbLignes => lignes;
        public int NbColonnes => colonnes;
        public char[,] Grille => grille;

        // --- CONSTRUCTEURS (Gestion des deux cas d'initialisation) ---


        /// Constructeur pour générer un plateau aléatoirement selon les contraintes de Lettres.txt. 

        public Plateau(string cheminFichierLettres)
        {
            this.grille = new char[lignes, colonnes];
            ChargerContraintesLettres(cheminFichierLettres);

            if (contraintesLettres.Count > 0)
            {
                GenererPlateauSelonContraintes();
            }
        }


        /// Constructeur pour charger un plateau à partir d'un fichier existant.

        public Plateau(string nomFichier, bool fromFile)
        {
            if (fromFile)
            {
                // ToRead instanciera this.grille et définira lignes/colonnes
                ToRead(nomFichier);
            }
        }

        // --- GESTION DES CONTRAINTES DES LETTRES (Lettres.txt) ---


        /// Charge les contraintes (Max occurrences et Poids) à partir de Lettres.txt.
        public void ChargerContraintesLettres(string cheminFichier)
        {
            //Console.WriteLine(cheminFichier);
            contraintesLettres.Clear();
            try
            {
                // Utiliser File pour la lecture 
                string[] lignesTxt = File.ReadAllLines(cheminFichier);
                foreach (string ligne in lignesTxt)
                {
                    Console.WriteLine(ligne);
                    // Format : A,10,1 (Lettre, Max, Poids)
                    string[] parties = ligne.Split(',');
                    //AfficheTab(parties);
                    if (parties.Length == 3 &&
                        char.TryParse(parties[0].ToUpper().Trim(), out char lettre) &&
                        int.TryParse(parties[1].Trim(), out int max) &&
                        int.TryParse(parties[2].Trim(), out int poids))
                    {
                        //Console.WriteLine(lettre +  " " + max + " " + poids);
                        contraintesLettres[lettre] = (max, poids);
                    }
                }
               
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du chargement des contraintes des lettres : {ex.Message}");
            }
        }
        public void AfficheTab(string[] tab)
        {
            foreach (string elem in tab)
            {
                Console.WriteLine(elem);
            }
        }


        /// Fournit le poids d'une lettre (pour le scoring). 

        public int GetPoids(char lettre)
        {
            char lettreMajuscule = char.ToUpper(lettre); // Conversion pour la robustesse
            //Console.WriteLine(lettre + " " +  lettreMajuscule);
            if (contraintesLettres.ContainsKey(lettreMajuscule))
            {
                //Console.WriteLine("je suis dans le if");
                //Console.WriteLine(contraintesLettres[lettreMajuscule].poids);
                return contraintesLettres[lettreMajuscule].poids;
            }
            return 0; 
        }

        // --- GÉNÉRATION ---

        private void GenererPlateauSelonContraintes()
        {
            // La génération tient compte de ces contraintes. 
            List<char> lettresDisponibles = new List<char>();
            int tailleGrille = lignes * colonnes;

            foreach (var entry in contraintesLettres)
            {
                int maxOccurrences = Math.Min(entry.Value.max, tailleGrille);
                for (int i = 0; i < maxOccurrences; i++)
                {
                    lettresDisponibles.Add(entry.Key);
                }
            }

            while (lettresDisponibles.Count > tailleGrille)
            {
                int indexAleatoireARetirer = r.Next(lettresDisponibles.Count);
                lettresDisponibles.RemoveAt(indexAleatoireARetirer);
            }

            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    if (lettresDisponibles.Count > 0)
                    {
                        int indexAleatoire = r.Next(lettresDisponibles.Count);
                        grille[i, j] = lettresDisponibles[indexAleatoire];
                        lettresDisponibles.RemoveAt(indexAleatoire);
                    }
                    else
                    {
                        grille[i, j] = ' ';
                    }
                }
            }
        }

        // --- I/O (ToString, ToFile, ToRead) ---


        /// Retourne une chaîne de caractères qui décrit le plateau. [cite: 218]

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("  A B C D E F G H");
            for (int i = 0; i < lignes; i++)
            {
                sb.Append($"{i + 1} ");
                for (int j = 0; j < colonnes; j++)
                {
                    sb.Append($"{grille[i, j]} ");
                }
                if (i == lignes - 1)
                {
                    sb.AppendLine(" <-- Base de départ"); // Indique la ligne de départ
                }
                else
                {
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }


        /// Sauvegarde l'instance du plateau dans un fichier. 

        public void ToFile(string nomFile)
        {
            List<string> lignesTXT = new List<string>();
            for (int i = 0; i < lignes; i++)
            {
                string ligne = "";
                for (int j = 0; j < colonnes; j++)
                {
                    ligne += grille[i, j];
                    if (j < colonnes - 1) { ligne += ";"; }
                }
                lignesTXT.Add(ligne);
            }
            try { File.WriteAllLines(nomFile, lignesTXT); }
            catch (IOException ex) { Console.WriteLine($"Erreur I/O lors de la sauvegarde : {ex.Message}"); }
        }


        /// Instancie un plateau à partir d'un fichier. [cite: 220]

        public void ToRead(string nomFile)
        {
            try
            {
                string[] lignesTXT = File.ReadAllLines(nomFile);
                this.lignes = lignesTXT.Length;
                if (this.lignes > 0) { this.colonnes = lignesTXT[0].Split(';').Length; }
                else { this.colonnes = 0; }

                grille = new char[lignes, colonnes];

                for (int i = 0; i < lignes; i++)
                {
                    string[] valeurs = lignesTXT[i].Split(';');
                    for (int j = 0; j < colonnes; j++)
                    {
                        if (valeurs[j].Length > 0) { grille[i, j] = valeurs[j].ToUpper()[0]; }
                        else { grille[i, j] = ' '; }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la lecture du plateau : {ex.Message}");
                this.lignes = 8; this.colonnes = 8; grille = new char[lignes, colonnes];
            }
        }


        /// Vérifie si le plateau est vide.

        public bool EstVide()
        {
            for (int i = 0; i < lignes; i++)
            {
                for (int j = 0; j < colonnes; j++)
                {
                    if (grille[i, j] != ' ') { return false; }
                }
            }
            return true;
        }

        // --- RECHERCHE RÉCURSIVE DE MOT ---

        /// Teste si le mot est éligible sur le plateau. 

        public CheminMot Recherche_Mot(string mot)
        {
            // 1. Préparation et Validation de base
            mot = mot.ToUpper();
            if (mot.Length < 2)
                return null;

            int ligneDepart = lignes - 1;

            // 2. Parcours de la ligne de base (point de départ obligatoire)
            for (int col = 0; col < colonnes; col++)
            {
                // Vérifie si la première lettre correspond à une case de la base
                if (grille[ligneDepart, col] == mot[0])
                {
                    Position posDepart = new Position(ligneDepart, col);

                    // 3. Initialisation des états pour la récursion
                    CheminMot chemin = new CheminMot { MotTrouve = mot };
                    List<Position> dejaVisite = new List<Position>();

                    // Ajout de la première position au chemin et aux visités
                    chemin.Coordonnees.Add(posDepart);
                    dejaVisite.Add(posDepart);

                    // 4. Lancement de la récursion pour la DEUXIÈME lettre (index 1)
                    // Signature utilisée : ChercheRec(chemin, mot, indexLettre, posActuelle, dejaVisite)
                    if (ChercheRec(chemin, mot, 1, posDepart, dejaVisite))
                    {
                        return chemin; // Mot trouvé
                    }

                    // Si le ChercheRec échoue, la boucle passe à la colonne suivante (col++).
                    // Le chemin et dejaVisite sont réinitialisés au début de la boucle.
                }
            }

            return null; // Mot non trouvé sur la ligne de base
        }


        /// Vérifie manuellement si une position est déjà dans le chemin actuel.

        private bool EstDejaDansChemin(List<Position> chemin, int ligne, int col)
        {
            foreach (Position p in chemin)
            {
                if (p.Ligne == ligne && p.Colonne == col)
                    return true;
            }
            return false;
        }


        /// Recherche récursive du mot dans la grille.

        private bool ChercheRec(CheminMot chemin, string mot, int indexLettre, Position posActuelle, List<Position> dejaVisite)
        {
            // 1. CONDITION D'ARRÊT (SUCCÈS)
            // Si l'index à chercher est égal à la longueur du mot, le mot est complet.
            if (indexLettre == mot.Length)
            {
                return true;
            }

            char lettreAchercher = mot[indexLettre];

            // 2. EXPLORATION DES VOISINS (8 directions)
            // Note : On explore toutes les directions sauf le bas et les diagonales bas
            int[][] mouvements = new int[][]
            {
        new int[] {-1, 0},   // Haut 
        new int[] { 0, -1},  // Gauche 
        new int[] { 0, 1},   // Droite 
        new int[] {-1, -1},  // Diagonale Haut-Gauche 
        new int[] {-1, 1}    // Diagonale Haut-Droite 
            };


            foreach (var move in mouvements)
            {
                Position posVoisine = new Position(posActuelle.Ligne + move[0], posActuelle.Colonne + move[1]);

                
               // FIX CRITIQUE : Vérification des limites (Évite l'IndexOutOfBounds)
               
                if (posVoisine.Ligne >= 0 && posVoisine.Ligne < lignes &&
                    posVoisine.Colonne >= 0 && posVoisine.Colonne < colonnes)
                {
                    // Vérifie si la lettre correspond ET si la position n'a pas été visitée dans ce chemin.
                    if (grille[posVoisine.Ligne, posVoisine.Colonne] == lettreAchercher &&
                        !dejaVisite.Any(p => Position.Compare(p, posVoisine)))
                    {
                        // --- 3. AVANCE : Enregistrement et Appel Récursif ---

                        // Ajout au chemin et aux visités
                        chemin.Coordonnees.Add(posVoisine);
                        dejaVisite.Add(posVoisine);

                        // Appel récursif pour la lettre suivante (indexLettre + 1)
                        if (ChercheRec(chemin, mot, indexLettre + 1, posVoisine, dejaVisite))
                        {
                            return true; // Succès trouvé, on remonte.
                        }

                        // --- 4. BACKTRACKING (Échec de cette branche) ---
                        // Si l'appel récursif est revenu FALSE, cette direction était une impasse.
                        // On retire la dernière position ajoutée pour essayer une autre branche.
                        dejaVisite.RemoveAt(dejaVisite.Count - 1);
                        chemin.Coordonnees.RemoveAt(chemin.Coordonnees.Count - 1);
                    }
                }
            }

            // 5. CONDITION D'ÉCHEC
            // Si les voisins ont été explorés sans succès.
            return false;
        }




        /// Met à jour la matrice en fonction du mot trouvé (glissement des lettres). [cite: 231]

        public void MAJ_Plateau(CheminMot chemin)
        {
            // 1. Effacer les lettres
            foreach (Position p in chemin.Coordonnees)
            {
                grille[p.Ligne, p.Colonne] = ' ';
            }

            // 2. Glissement colonne par colonne 
            List<int> colonnesImpactees = chemin.Coordonnees.Select(c => c.Colonne).Distinct().ToList();

            foreach (int col in colonnesImpactees)
            {
                FaireTomber(col);
            }
        }

        private void FaireTomber(int col)
        {
            // Parcourt la colonne de bas en haut
            for (int ligne = lignes - 1; ligne >= 0; ligne--)
            {
                // Si la case courante est vide
                if (grille[ligne, col] == ' ')
                {
                    // Cherche la première lettre non vide au-dessus
                    for (int haut = ligne - 1; haut >= 0; haut--)
                    {
                        if (grille[haut, col] != ' ')
                        {
                            // Glissement
                            grille[ligne, col] = grille[haut, col];
                            grille[haut, col] = ' ';
                            break;
                        }
                    }
                }
            }
        }
    }
}
