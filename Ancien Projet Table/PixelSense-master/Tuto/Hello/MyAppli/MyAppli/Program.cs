using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace MyAppli
{
    class Program
    {
        static void Main(string[] args)
        {
            MyAppli.Voiture voiture = new MyAppli.Voiture();
            MyAppli.Voiture voiture2 = new MyAppli.Voiture();

            voiture.rouler();
            voiture.klaxonner();

            voiture2.modele = "ferrari";
            voiture.modele = "mazeratti";

            /*Gestion des listes en C#*/
            List<Voiture> garage = new List<Voiture>() ;
            garage.Add(voiture);
            garage.Add(voiture2);

            /*Affichage du garage*/
            foreach (Voiture voit in garage)
            {
                Console.WriteLine(voit.modele);
            }

            /*Chaines de caractères et tableeaux*/
            string[] helloworld = new string[] { "Hello","World", "Groupe", "Pri","Pixel","Sense" };
            Array.Sort(helloworld);
            for (int i = 0; i < helloworld.Length; i++) {
                Console.WriteLine(helloworld[i]);
            }
            //string hello = "hello";
            //Console.WriteLine(hello);

            /*Exemples de listes en C# */
            List<int> chiffres = new List<int>(); // création de la liste chiffres 
            chiffres.Add(8); // chiffres contient 8
            chiffres.Add(9); // chiffres contient 8, 9
            chiffres.Add(4); // chiffres contient 8, 9, 4
            chiffres.RemoveAt(0); // chiffres contient 8, 4
            
            Console.WriteLine(DateTime.Now);
            /*parcours de la liste chiffres*/
            foreach (int chiffre in chiffres)
            {
                Console.WriteLine(chiffre);
            }
            /*Nous allons jouer à un petit jeu , devinez à quel nombre je pense entre 0 et 99*/
            int valeurATrouver = new Random().Next(0, 100);
            int nombreDeCoups = 0;
            bool trouve = false;
            Console.WriteLine("Veuillez saisir un nombre compris entre 0 et 100 (exclu)");
            while (!trouve)
            {
                string saisie = Console.ReadLine();
                int valeurSaisie;
                if (int.TryParse(saisie, out valeurSaisie))
                {
                    if (valeurSaisie == valeurATrouver)
                        trouve = true;
                    else
                    {
                        if (valeurSaisie < valeurATrouver)
                            Console.WriteLine("Trop petit ...");
                        else
                            Console.WriteLine("Trop grand ...");
                    }
                    nombreDeCoups++;
                }
                else
                    Console.WriteLine("La valeur saisie est incorrecte, veuillez recommencer ...");
            }
            Console.WriteLine("Vous avez trouvé en " + nombreDeCoups + " coup(s)");
        }
    }
}
