namespace R19_BW_laczenia_Android
{
    class Item
    {
        public Item(int pref = 0, int baza = 0, int suf = 0, int iloscL = 0)
        {
            p = pref;
            b = baza;
            s = suf;
            iloscLaczen = iloscL;
        }
        public Item(Item i)
        {
            p = i.p;
            b = i.b;
            s = i.s;
            h = i.h;
            iloscLaczen = i.iloscLaczen;
        }

        // Klasa przedmiotu - Typ zmiennej przechowujący statystyki przedmiotu
        public int p; // Prefix index
        public int b; // Base index
        public int s; // Sufix index
        public string h; // History
        public int iloscLaczen; // Ilość wymaganych łączeń aby otrzymać ten przedmiot

        public int Sum()
        {
            return p + b + s;
        }

        public void Set(Item i)
        {
            p = i.p;
            b = i.b;
            s = i.s;
            h = i.h;
            iloscLaczen = i.iloscLaczen;
        }

        public Item Polacz(Item item, System.Collections.Generic.List<string> pref, System.Collections.Generic.List<string> baza, System.Collections.Generic.List<string> suf, bool wyjatekHelm = false)
        {
            // Łączenie 
            int[] wynik = new int[] { 0, 0, 0 };
            double x = 0d, y = 0d;

            for (int i = 0; i < 3; i++)
            {
                switch (i)
                {
                    case 0:
                        // Prefiksy
                        x = this.p;
                        y = item.p;
                        break;
                    case 1:
                        // Bazy
                        x = this.b;
                        y = item.b;
                        break;
                    case 2:
                        // Sufiksy
                        x = this.s;
                        y = item.s;
                        break;
                }

                if ((int)x == 0 || (int)y == 0) wynik[i] = 0;
                else if (x == y) wynik[i] = (int)x;
                else wynik[i] = System.Convert.ToInt32(System.Math.Ceiling((x + y) / 2d) + 1d);
                // Wyjątek przy łączeniu Czapka + Hełm = Maska
                if (wyjatekHelm == true && x == 1 && y == 3 && i == 1) wynik[i] = 4;
                if (wyjatekHelm == true && x == 3 && y == 1 && i == 1) wynik[i] = 4;
            }

            Item w = new Item(wynik[0], wynik[1], wynik[2]);
            w = SprawdzWyjatki(item, pref, baza, suf, w);

            return w;
        }

        private Item SprawdzWyjatki(Item item, System.Collections.Generic.List<string> pref, System.Collections.Generic.List<string> baza, System.Collections.Generic.List<string> suf, Item wynik)
        {
            // Sprawdzenie wyjątków przy łączeniu przy końcu tabeli łączeń
            Item w = new Item(wynik);

            // Prefiksy
            if (pref != null)
            {
                if ((this.p == (pref.Count - 1)) && (item.p == (pref.Count - 2))) w.p = pref.Count - 3;
                if ((this.p == (pref.Count - 2)) && (item.p == (pref.Count - 1))) w.p = pref.Count - 3;
                if ((this.p == (pref.Count - 1)) && (item.p == (pref.Count - 3))) w.p = pref.Count - 2;
                if ((this.p == (pref.Count - 3)) && (item.p == (pref.Count - 1))) w.p = pref.Count - 2;
            }
            // Bazy
            if (baza != null)
            {
                if ((this.b == (baza.Count - 1)) && (item.b == (baza.Count - 2))) w.b = baza.Count - 3;
                if ((this.b == (baza.Count - 2)) && (item.b == (baza.Count - 1))) w.b = baza.Count - 3;
                if ((this.b == (baza.Count - 1)) && (item.b == (baza.Count - 3))) w.b = baza.Count - 2;
                if ((this.b == (baza.Count - 3)) && (item.b == (baza.Count - 1))) w.b = baza.Count - 2;
            }
            // Sufiksy
            if (suf != null)
            {
                if ((this.s == (suf.Count - 1)) && (item.s == (suf.Count - 2))) w.s = suf.Count - 3;
                if ((this.s == (suf.Count - 2)) && (item.s == (suf.Count - 1))) w.s = suf.Count - 3;
                if ((this.s == (suf.Count - 1)) && (item.s == (suf.Count - 3))) w.s = suf.Count - 2;
                if ((this.s == (suf.Count - 3)) && (item.s == (suf.Count - 1))) w.s = suf.Count - 2;
            }

            return w;
        }
    }
}