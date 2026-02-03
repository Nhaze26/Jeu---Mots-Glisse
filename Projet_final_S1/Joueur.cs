using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projet_final_S1
{
    public class Joueur
    {
        private List<string> motsTrouves;
        public string Nom { get; }
        public int Score { get; private set; } // Modifiable uniquement par Add_Score interne

        public Joueur(string nom)
        {
            if (string.IsNullOrWhiteSpace(nom))
            {
                throw new ArgumentException("Le nom du joueur ne peut pas être vide ou composé uniquement d'espaces.");
            }
            this.Nom = nom;
            this.Score = 0;
            this.motsTrouves = new List<string>();
        }

        // --- Méthodes de Gestion des Données ---
        /// Ajoute un mot à la liste du joueur, après validation externe (par Jeu).
 
        public void Add_Mot(string mot)
        {
            if (!string.IsNullOrWhiteSpace(mot))
            {
                this.motsTrouves.Add(mot);
            }
        }


        /// Ajoute des points au score du joueur.
        public void Add_Score(int val)
        {
            if (val >= 0) 
            {
                this.Score += val;
            }
        }

    
        /// Vérifie si le mot a déjà été trouvé par ce joueur.
        public bool Contient(string mot)
        {
            // Utilise la méthode Contains optimisée de la List<string> pour une vérification rapide.
            // On suppose que le mot passé ici est déjà en majuscules, comme les éléments de motsTrouves.
            return this.motsTrouves.Contains(mot);
        }


        /// Fournit la liste des mots trouvés, utilisée par la classe Jeu pour l'affichage.
        public List<string> GetMotsTrouves()
        {
            return motsTrouves;
        }

        // --- Affichage ---

        public override string ToString()
        {
            return $"Joueur : {this.Nom}\nScore : {this.Score}\nNombre de mots trouvés : {this.motsTrouves.Count}";
        }
    }
}

    
    
