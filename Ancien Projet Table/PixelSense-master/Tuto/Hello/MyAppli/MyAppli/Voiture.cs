using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

/* création de classe en C#*/
namespace MyAppli
{
    class Voiture
    {
        /*quelques méthodes simples*/
        public string modele;
        public void descriptif() { Console.WriteLine(modele);}
        public void rouler() { Console.WriteLine("Vroum"); }
        public void klaxonner() { Console.WriteLine("BIP"); }
     
       
    }
}
