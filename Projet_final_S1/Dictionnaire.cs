using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_final_S1
{
    public class Dictionnaire
    {
        // Attributs
        private string[][] motsParLettre = new string[26][]; // Tableau de 26 tableaux de mots (A-Z)
        private string cheminFichierUtilise;
        private const string Langue = "FRANÇAIS";

        // Propriété de lecture seule
        public string CheminFichierUtilise => cheminFichierUtilise;

        // --- CONSTRUCTEUR (Utilisation de StreamReader sans Try-Catch) ---
        public Dictionnaire(string chemin)
        {
            this.cheminFichierUtilise = chemin;

            if (File.Exists(this.cheminFichierUtilise))
            {
                try
                {
                    // Utilisation de StreamReader, comme exigé pour la lecture.
                    using (StreamReader sr = new StreamReader(this.cheminFichierUtilise))
                    {
                        string ligne;
                        int indexLigne = 0;

                        while ((ligne = sr.ReadLine()) != null && indexLigne < 26)
                        {
                            // Stockage des mots en MAJUSCULE
                            string[] motsLigne = ligne.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToArray();
                            motsParLettre[indexLigne] = motsLigne;
                            indexLigne++;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Erreur I/O lors du chargement du dictionnaire : {ex.Message}");
                    // Vous pouvez ici lancer une exception ou simplement laisser le dictionnaire vide
                }


                // Le dictionnaire doit être trié après le chargement
                Tri_Fusion();
            }
        }

        // --- Méthode StreamWriter (Maintenue pour la Conformité à l'Énoncé) ---

        /// <summary>
        /// Sauvegarde le dictionnaire trié (utilise StreamWriter, obligatoire par l'énoncé).
        /// </summary>
        public void SauvegarderDictionnaireTrié(string cheminFichierSortie)
        {
      
               using (StreamWriter sw = new StreamWriter(cheminFichierSortie))
               {
                    for (int i = 0; i < motsParLettre.Length; i++)
                    {
                        if (motsParLettre[i] != null && motsParLettre[i].Length > 0)
                        {
                            string ligneMots = string.Join(" ", motsParLettre[i]);
                            sw.WriteLine(ligneMots);
                        }
                        else
                        {
                            sw.WriteLine("");
                        }
                    }
                }
                Console.WriteLine("Le dictionnaire trié a été sauvegardé dans : " + cheminFichierSortie);
        }

        // --- Tri Fusion (Tri_XXX) ---

        /// <summary>
        /// Trie chaque sous-tableau de mots (par initiale) en utilisant l'algorithme de Tri Fusion.
        /// </summary>
        public void Tri_Fusion()
        {
            for (int i = 0; i < motsParLettre.Length; i++)
            {
                if (motsParLettre[i] != null && motsParLettre[i].Length > 1)
                {
                    motsParLettre[i] = DiviserPourRegner(motsParLettre[i]);
                }
            }
        }

        private string[] DiviserPourRegner(string[] array)
        {
            if (array == null || array.Length <= 1) return array;

            int milieu = array.Length / 2;  
            // string[] gauche = array[..milieu];
            string[] gauche = new string[milieu];
            Array.Copy(array, 0, gauche, 0, milieu); // (Source, index_source, destination, index_dest, longueur)

            // --- Remplacement de l'opérateur de plage pour la partie DROITE ---
            // string[] droite = array[milieu..];
            int tailleDroite = array.Length - milieu;
            string[] droite = new string[tailleDroite];
            Array.Copy(array, milieu, droite, 0, tailleDroite); // (Source, index_source (milieu), destination, index_dest, longueur)

            gauche = DiviserPourRegner(gauche);
            droite = DiviserPourRegner(droite);

            return Fusionner(gauche, droite);
        }

        private string[] Fusionner(string[] gauche, string[] droite)
        {
            int i = 0, j = 0, k = 0;
            string[] resultat = new string[gauche.Length + droite.Length];
            while (i < gauche.Length && j < droite.Length)
            {
                if (string.Compare(gauche[i], droite[j], StringComparison.Ordinal) <= 0)
                {
                    resultat[k++] = gauche[i++];
                }
                else
                {
                    resultat[k++] = droite[j++];
                }
            }
            while (i < gauche.Length) resultat[k++] = gauche[i++];
            while (j < droite.Length) resultat[k++] = droite[j++];
            return resultat;
        }

        // --- Recherche Dichotomique Récursive ---

        public bool RechDichoRecursif(string mot)
        {
            if (string.IsNullOrEmpty(mot)) return false;
            string motRecherche = mot.ToUpper().Trim();
            char premiereLettre = motRecherche.Length > 0 ? motRecherche[0] : ' ';
            int index = premiereLettre - 'A';
            if (index < 0 || index >= 26 || motsParLettre[index] == null || motsParLettre[index].Length == 0)
            {
                return false;
            }
            string[] tableau = motsParLettre[index];
            return RechercheBinaireRec(tableau, motRecherche, 0, tableau.Length - 1);
        }

        private bool RechercheBinaireRec(string[] tableau, string mot, int min, int max)
        {
            if (min > max) return false;
            int milieu = min + (max - min) / 2;
            int comparaison = string.Compare(tableau[milieu], mot, StringComparison.Ordinal);       //0   Les deux mots sont identiques.
                                                                                                    //< 0 Le mot du tableau(tableau[milieu]) est alphabétiquement avant le mot recherché(mot).
                                                                                                    //> 0 Le mot du tableau(tableau[milieu]) est alphabétiquement après le mot recherché(mot).
            if (comparaison == 0) return true;
            else if (comparaison < 0) return RechercheBinaireRec(tableau, mot, milieu + 1, max);
            else return RechercheBinaireRec(tableau, mot, min, milieu - 1);
        }

        // --- Méthode ToString() ---

        public override string ToString()
        {
            int totalMots = 0;
            for (int i = 0; i < motsParLettre.Length; i++)
            {
                if (motsParLettre[i] != null)
                {
                    totalMots += motsParLettre[i].Length;
                }
            }
            return "Description du Dictionnaire:\n" +
                   "- Langue: {Langue}\n" +
                   "- Chemin du fichier: {cheminFichierUtilise}\n" +
                   "- Nombre total de mots triés: " + totalMots;
        }
    }
}