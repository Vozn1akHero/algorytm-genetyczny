using System;
using System.Collections.Generic;
using System.Linq;

namespace AlgorytmGenetyczny
{
    static class Helpers
    {
        public static double ConvertChromosomToDouble(List<Osobnik> b)
        {
            List<string> arr = b.Select(e => Convert.ToString(e.Wartosc)).ToList();
            string arrTemp = string.Join("", arr.ToArray());
            double wynik = Convert.ToDouble(Convert.ToString(Convert.ToInt32(arrTemp, 2), 10));
           
            return wynik;
        }
    }

    static class Selekcja
    {
        public static HashSet<int> MetodaKolaRuletki(List<double> wynikiZFunkcjiPrzystosowania, 
            int rozmiarPopulacji)
        {
            double wynikiFPSuma = wynikiZFunkcjiPrzystosowania.Sum(e => e);

            HashSet<int> wyniki = new HashSet<int>();
            Dictionary<int, double> kolo = new Dictionary<int, double>();

            for (int i = 0; i < rozmiarPopulacji; i++)
            {
                double wynikTemp = (100 / wynikiFPSuma) * wynikiZFunkcjiPrzystosowania[i];
                kolo.Add(i, wynikTemp);
            }

            var posortowaneKolo = from entry in kolo orderby entry.Value descending select entry;
            
            posortowaneKolo.ToList().ForEach(e => wyniki.Add(e.Key));

            return wyniki;
        }
    }
    class Osobnik
    {
        public int Wartosc { get; set; }    
        public Osobnik()
        {
            Wartosc = new Random().Next(2);
        }

        public static List<Osobnik> InicjalizacjaOsobnika(int rozmiarChromosomu)
        {
            List<Osobnik> osobnikiTemp = new List<Osobnik>();

            for (int i = 0; i < rozmiarChromosomu; i++)
            {
                osobnikiTemp.Add(new Osobnik());
            }

            return osobnikiTemp;
        }
    }

    static class Przystosowanie
    {
        private static readonly Func<double, double> FunkcjaCelu = osobnikiWartoscDouble =>
            (Math.Sin(osobnikiWartoscDouble) + 10) / (Math.Pow(Math.Exp(osobnikiWartoscDouble), 2) - 1);

        public static double FunkcjaPrzystosowania(List<Osobnik> osobniki)
        { 
            return FunkcjaCelu(Helpers.ConvertChromosomToDouble(osobniki));
        }
    }

    static class OperacjeGenetyczne
    {
        public static void Mutacja(List<Osobnik> osobniki)
        {
            int losowaPozycja = new Random().Next(osobniki.Count);
            if (osobniki.ElementAt(losowaPozycja).Wartosc == 0)
                osobniki.ElementAt(losowaPozycja).Wartosc = 1;
            else osobniki.ElementAt(losowaPozycja).Wartosc = 0;
        }
        public static void Krzyzowanie(List<Osobnik> osobniki1, List<Osobnik> osobniki2, int rozmiarChromosomu)
        {
            int losowaPozycja = new Random().Next(0, osobniki1.Count);
            for (int i = losowaPozycja; i < rozmiarChromosomu; i++)
            {
                int wartoscOsobnikaTemp = osobniki1.ElementAt(i).Wartosc;
                osobniki1.ElementAt(i).Wartosc = osobniki2.ElementAt(i).Wartosc;
                osobniki2.ElementAt(i).Wartosc = wartoscOsobnikaTemp;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            int rozmiarPopulacji = 10;
            int rozmiarChromosomu = 8;
            int liczbaPokolen = 1;

            List<List<Osobnik>> populacja = new List<List<Osobnik>>();


            for(int i = 0; i < rozmiarPopulacji; i++) populacja.Add(Osobnik.InicjalizacjaOsobnika(8));

            for (int i = 0; i < liczbaPokolen; i++)
            {
                for (int populacjaIndex = 0; populacjaIndex < rozmiarPopulacji; populacjaIndex++)
                {
                    int wylosowanaOperacjaGenetyczna = new Random().Next(2);

                    if (wylosowanaOperacjaGenetyczna == 0)
                    {
                        OperacjeGenetyczne.Mutacja(populacja.ElementAt(populacjaIndex));
                    }
                    else
                    {
                        OperacjeGenetyczne.Krzyzowanie(populacja.ElementAt(populacjaIndex), 
                            populacja.ElementAt(new Random().Next(rozmiarPopulacji)), rozmiarChromosomu);
                    }
                }

                List<double> wynikiZFunkcjiPrzystosowania = new List<double>();
                for (int index = 0; index < rozmiarPopulacji; index++)
                {
                    wynikiZFunkcjiPrzystosowania.Add(Przystosowanie.FunkcjaPrzystosowania(populacja.ElementAt(index)));
                }

                HashSet<int> wyniki = Selekcja.MetodaKolaRuletki(wynikiZFunkcjiPrzystosowania, rozmiarPopulacji);

               
                for (int index = 0; index < wyniki.Count; index++)
                {
                    List<Osobnik> chromosom = populacja.ElementAt(wyniki.ElementAt(index));
                    
                    chromosom.ForEach(e =>
                    {
                        Console.Write(e.Wartosc);
                    });
                    Console.WriteLine("\n");
                }
            }

        }
    }
}
