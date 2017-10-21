using System;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Views;
using Java.Interop;
using System.Collections.Generic;
using System.Linq;
using static Android.Widget.AdapterView;
using System.IO;

namespace R19_BW_laczenia_Android
{
    [Activity(Label = "R19 Blood Wars Łączenia", MainLauncher = true, Icon = "@drawable/Icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Załaduj bazy przedmiotów do list
            BazaHelmow(PrefHelm, BazaHelm, SufHelm);
            BazaZbroi(PrefZbroja, BazaZbroja, SufZbroja);
            BazaSpodni(PrefSpodnie, BazaSpodnie, SufSpodnie);
            BazaPierścieni(PrefPierscien, BazaPierscien, SufPierscien);
            BazaAmuletow(PrefAmulet, BazaAmulet, SufAmulet);
            BazaBialych1h(PrefBiala1h, BazaBiala1h, SufBiala1h);
            BazaBialych2h(PrefBiala2h, BazaBiala2h, SufBiala2h);
            BazaPalnych1h(PrefPalan1h, BazaPalna1h, SufPalna1h);
            BazaPalnych2h(PrefPalna2h, BazaPalna2h, SufPalna2h);
            BazaDystansow(PrefDystans, BazaDystans, SufDystans);

            // Dodaj zdarzenie zmiany wybranego typu przedmiotu
            FindViewById<Spinner>(Resource.Id.itemSelector).ItemSelected += new EventHandler<ItemSelectedEventArgs>(ItemSelector_Click);

            // Dodaj pozycje do listy opcji
            listaOpcji.Add("Dodaj przedmiot");
            listaOpcji.Add("Importuj przedmioty");
            listaOpcji.Add("Wyczyść listę przedmiotów");
            listaOpcji.Add("Wyczyść przebieg łączenia");
            listaOpcji.Add("Tabele łączeń");
        }

        // Typy przedmiotów
        string[] typyPrzedmiotow = new string[] { "Hełm", "Zbroja", "Spodnie", "Pierścień", "Amulet", "Biała 1h", "Biała 2h", "Palna 1h", "Palna 2h", "Dystans" };
        // Wybrany typ przedmiotu
        string typPrzedmiotu = "";

        // Listy prefiksów, baz i sufiksów każdego typu przedmiotów
        List<string> PrefHelm = new List<string>();
        List<string> BazaHelm = new List<string>();
        List<string> SufHelm = new List<string>();

        List<string> PrefZbroja = new List<string>();
        List<string> BazaZbroja = new List<string>();
        List<string> SufZbroja = new List<string>();

        List<string> PrefSpodnie = new List<string>();
        List<string> BazaSpodnie = new List<string>();
        List<string> SufSpodnie = new List<string>();

        List<string> PrefPierscien = new List<string>();
        List<string> BazaPierscien = new List<string>();
        List<string> SufPierscien = new List<string>();

        List<string> PrefAmulet = new List<string>();
        List<string> BazaAmulet = new List<string>();
        List<string> SufAmulet = new List<string>();

        List<string> PrefBiala1h = new List<string>();
        List<string> BazaBiala1h = new List<string>();
        List<string> SufBiala1h = new List<string>();

        List<string> PrefBiala2h = new List<string>();
        List<string> BazaBiala2h = new List<string>();
        List<string> SufBiala2h = new List<string>();

        List<string> PrefPalan1h = new List<string>();
        List<string> BazaPalna1h = new List<string>();
        List<string> SufPalna1h = new List<string>();

        List<string> PrefPalna2h = new List<string>();
        List<string> BazaPalna2h = new List<string>();
        List<string> SufPalna2h = new List<string>();

        List<string> PrefDystans = new List<string>();
        List<string> BazaDystans = new List<string>();
        List<string> SufDystans = new List<string>();

        // Lista zmiennych do łączenia
        List<Item> przedmioty = new List<Item>();
        List<string> listString;
        List<Item> wyniki = new List<Item>();
        Item pierwszySkladnik = new Item();
        Item skladnik1 = new Item();
        Item skladnik2 = new Item();

        // Lista opcji
        List<string> listaOpcji = new List<string>();

        public void ItemSelector_Click(object sender, ItemSelectedEventArgs e)
        {
            /// Funkcja wywoływana przy zmianie typu łączonego przedmiotu

            if (typPrzedmiotu == typyPrzedmiotow[e.Position])
            {
                // Jeżeli wybrano ten sam typ przedmiotu to nic nie rób
            }
            else
            {
                // Jeżeli wybrano inny typ przedmiotu to wyczyść listę przedmiotów, listę wyników, składniki, historię łączeń i ustaw łączony typ przedmiotu
                przedmioty.Clear();
                wyniki.Clear();
                skladnik1 = null;
                skladnik2 = null;
                FindViewById<TextView>(Resource.Id.textWynik).Text = "";
                typPrzedmiotu = typyPrzedmiotow[e.Position];
            }
        }

        [Export("Dodaj_Click")]
        public void Dodaj_Click(View v)
        {
            /// Zmień widok na widok dodania przedmiotu do łączenia
            if (przedmioty.Count == 0)
            {
                Toast.MakeText(this, "Brak przedmiotów w liście przedmiotów!", ToastLength.Short).Show();
                return;
            }
            else ZmienWidok("AddItem");
        }

        [Export("OK_Click")]
        public void OK_Click(View v)
        {
            /// Wybrano przedmiot do łączenia, zidentyfikuj go i połącz z poprzednim składnikiem

            // Jeżeli nie wybrano żadnego przedmiotu to poinformuj o tym urzytkownika i wyjdź z funkcji
            if (FindViewById<ListView>(Resource.Id.listaPrzedmiotow).CheckedItemCount == 0)
            {
                Toast.MakeText(this, "Zaznacz przedmiot do dodania!", ToastLength.Short).Show();
                return;
            }

            // Jeżeli pierwszy raz wybrano przedmiot do łączenia to ustaw go jako składnik1 i dodaj go do tekstu łączenia
            if (skladnik1 == null || skladnik1.Sum() == 0)
            {
                skladnik1 = new Item(przedmioty[FindViewById<ListView>(Resource.Id.listaPrzedmiotow).CheckedItemPosition]);
                pierwszySkladnik = new Item(skladnik1);

                // Zanim dodasz składnik do okienka z tekstem przełącz się na widok Main
                ZmienWidok("Main");

                // Dodaj składnik do okienka z tekstem
                switch (typPrzedmiotu)
                {
                    case "Hełm":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefHelm, BazaHelm, SufHelm);
                        break;
                    case "Zbroja":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefZbroja, BazaZbroja, SufZbroja);
                        break;
                    case "Spodnie":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefSpodnie, BazaSpodnie, SufSpodnie);
                        break;
                    case "Pierścień":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefPierscien, BazaPierscien, SufPierscien);
                        break;
                    case "Amulet":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefAmulet, BazaAmulet, SufAmulet);
                        break;
                    case "Biała 1h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefBiala1h, BazaBiala1h, SufBiala1h);
                        break;
                    case "Biała 2h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefBiala2h, BazaBiala2h, SufBiala2h);
                        break;
                    case "Palna 1h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefPalan1h, BazaPalna1h, SufPalna1h);
                        break;
                    case "Palna 2h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefPalna2h, BazaPalna2h, SufPalna2h);
                        break;
                    case "Dystans":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefDystans, BazaDystans, SufDystans);
                        break;
                }
            }
            else
            {
                // Jeżeli wcześniej wybrano składnik1 to ustaw składnik2 i połącz składniki
                skladnik2 = new Item(przedmioty[FindViewById<ListView>(Resource.Id.listaPrzedmiotow).CheckedItemPosition]);
                Item wynik = new Item();

                switch (typPrzedmiotu)
                {
                    case "Hełm":
                        wynik = skladnik1.Polacz(skladnik2, PrefHelm, BazaHelm, SufHelm, true);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefHelm, BazaHelm, SufHelm) + " + " + UsunSpacje(skladnik2, PrefHelm, BazaHelm, SufHelm) + " = " + UsunSpacje(wynik, PrefHelm, BazaHelm, SufHelm);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefHelm, BazaHelm, SufHelm) + " = " + UsunSpacje(wynik, PrefHelm, BazaHelm, SufHelm);
                        break;
                    case "Zbroja":
                        wynik = skladnik1.Polacz(skladnik2, PrefZbroja, BazaZbroja, SufZbroja);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefZbroja, BazaZbroja, SufZbroja) + " + " + UsunSpacje(skladnik2, PrefZbroja, BazaZbroja, SufZbroja) + " = " + UsunSpacje(wynik, PrefZbroja, BazaZbroja, SufZbroja);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefZbroja, BazaZbroja, SufZbroja) + " = " + UsunSpacje(wynik, PrefZbroja, BazaZbroja, SufZbroja);
                        break;
                    case "Spodnie":
                        wynik = skladnik1.Polacz(skladnik2, PrefSpodnie, BazaSpodnie, SufSpodnie);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefSpodnie, BazaSpodnie, SufSpodnie) + " + " + UsunSpacje(skladnik2, PrefSpodnie, BazaSpodnie, SufSpodnie) + " = " + UsunSpacje(wynik, PrefSpodnie, BazaSpodnie, SufSpodnie);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefSpodnie, BazaSpodnie, SufSpodnie) + " = " + UsunSpacje(wynik, PrefSpodnie, BazaSpodnie, SufSpodnie);
                        break;
                    case "Pierścień":
                        wynik = skladnik1.Polacz(skladnik2, PrefPierscien, BazaPierscien, SufPierscien);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefPierscien, BazaPierscien, SufPierscien) + " + " + UsunSpacje(skladnik2, PrefPierscien, BazaPierscien, SufPierscien) + " = " + UsunSpacje(wynik, PrefPierscien, BazaPierscien, SufPierscien);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefPierscien, BazaPierscien, SufPierscien) + " = " + UsunSpacje(wynik, PrefPierscien, BazaPierscien, SufPierscien);
                        break;
                    case "Amulet":
                        wynik = skladnik1.Polacz(skladnik2, PrefAmulet, BazaAmulet, SufAmulet);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefAmulet, BazaAmulet, SufAmulet) + " + " + UsunSpacje(skladnik2, PrefAmulet, BazaAmulet, SufAmulet) + " = " + UsunSpacje(wynik, PrefAmulet, BazaAmulet, SufAmulet);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefAmulet, BazaAmulet, SufAmulet) + " = " + UsunSpacje(wynik, PrefAmulet, BazaAmulet, SufAmulet);
                        break;
                    case "Biała 1h":
                        wynik = skladnik1.Polacz(skladnik2, PrefBiala1h, BazaBiala1h, SufBiala1h);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefBiala1h, BazaBiala1h, SufBiala1h) + " + " + UsunSpacje(skladnik2, PrefBiala1h, BazaBiala1h, SufBiala1h) + " = " + UsunSpacje(wynik, PrefBiala1h, BazaBiala1h, SufBiala1h);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefBiala1h, BazaBiala1h, SufBiala1h) + " = " + UsunSpacje(wynik, PrefBiala1h, BazaBiala1h, SufBiala1h);
                        break;
                    case "Biała 2h":
                        wynik = skladnik1.Polacz(skladnik2, PrefBiala2h, BazaBiala2h, SufBiala2h);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefBiala2h, BazaBiala2h, SufBiala2h) + " + " + UsunSpacje(skladnik2, PrefBiala2h, BazaBiala2h, SufBiala2h) + " = " + UsunSpacje(wynik, PrefBiala2h, BazaBiala2h, SufBiala2h);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefBiala2h, BazaBiala2h, SufBiala2h) + " = " + UsunSpacje(wynik, PrefBiala2h, BazaBiala2h, SufBiala2h);
                        break;
                    case "Palna 1h":
                        wynik = skladnik1.Polacz(skladnik2, PrefPalan1h, BazaPalna1h, SufPalna1h);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefPalan1h, BazaPalna1h, SufPalna1h) + " + " + UsunSpacje(skladnik2, PrefPalan1h, BazaPalna1h, SufPalna1h) + " = " + UsunSpacje(wynik, PrefPalan1h, BazaPalna1h, SufPalna1h);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefPalan1h, BazaPalna1h, SufPalna1h) + " = " + UsunSpacje(wynik, PrefPalan1h, BazaPalna1h, SufPalna1h);
                        break;
                    case "Palna 2h":
                        wynik = skladnik1.Polacz(skladnik2, PrefPalna2h, BazaPalna2h, SufPalna2h);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefPalna2h, BazaPalna2h, SufPalna2h) + " + " + UsunSpacje(skladnik2, PrefPalna2h, BazaPalna2h, SufPalna2h) + " = " + UsunSpacje(wynik, PrefPalna2h, BazaPalna2h, SufPalna2h);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefPalna2h, BazaPalna2h, SufPalna2h) + " = " + UsunSpacje(wynik, PrefPalna2h, BazaPalna2h, SufPalna2h);
                        break;
                    case "Dystans":
                        wynik = skladnik1.Polacz(skladnik2, PrefDystans, BazaDystans, SufDystans);
                        if (wyniki.Count == 0) wynik.h = UsunSpacje(skladnik1, PrefDystans, BazaDystans, SufDystans) + " + " + UsunSpacje(skladnik2, PrefDystans, BazaDystans, SufDystans) + " = " + UsunSpacje(wynik, PrefDystans, BazaDystans, SufDystans);
                        if (wyniki.Count != 0) wynik.h = skladnik1.h + " + " + UsunSpacje(skladnik2, PrefDystans, BazaDystans, SufDystans) + " = " + UsunSpacje(wynik, PrefDystans, BazaDystans, SufDystans);
                        break;
                }

                // Dodaj otrzymany wynik do listy wyników
                wyniki.Add(new Item(wynik));
                // Ustaw wynik jako skladnik1
                skladnik1 = new Item(wynik);
                // Przełącz na widok okna głównego
                ZmienWidok("Main");
                // Zaktualizuj historię łączeń
                FindViewById<TextView>(Resource.Id.textWynik).Text = wyniki[wyniki.Count - 1].h;
            }
        }

        [Export("Cofnij_Click")]
        public void Cofnij_Click(View v)
        {
            /// Cofnij przeprowadzone połączenia o 1 wstecz
            if (wyniki.Count == 0)
            {
                wyniki.Clear();
                pierwszySkladnik = null;
                skladnik1 = null;
                FindViewById<TextView>(Resource.Id.textWynik).Text = "";
            }
            else if (wyniki.Count == 1)
            {
                wyniki.Clear();
                skladnik1 = new Item(pierwszySkladnik);
                switch (typPrzedmiotu)
                {
                    case "Hełm":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefHelm, BazaHelm, SufHelm);
                        break;
                    case "Zbroja":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefZbroja, BazaZbroja, SufZbroja);
                        break;
                    case "Spodnie":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefSpodnie, BazaSpodnie, SufSpodnie);
                        break;
                    case "Pierścień":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefPierscien, BazaPierscien, SufPierscien);
                        break;
                    case "Amulet":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefAmulet, BazaAmulet, SufAmulet);
                        break;
                    case "Biała 1h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefBiala1h, BazaBiala1h, SufBiala1h);
                        break;
                    case "Biała 2h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefBiala2h, BazaBiala2h, SufBiala2h);
                        break;
                    case "Palna 1h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefPalan1h, BazaPalna1h, SufPalna1h);
                        break;
                    case "Palna 2h":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefPalna2h, BazaPalna2h, SufPalna2h);
                        break;
                    case "Dystans":
                        FindViewById<TextView>(Resource.Id.textWynik).Text = UsunSpacje(skladnik1, PrefDystans, BazaDystans, SufDystans);
                        break;
                }
            }
            else if (wyniki.Count > 1)
            {
                wyniki.RemoveAt(wyniki.Count - 1);
                skladnik1 = wyniki[wyniki.Count - 1];
                FindViewById<TextView>(Resource.Id.textWynik).Text = wyniki[wyniki.Count - 1].h;
            }
        }

        [Export("Zapisz_Click")]
        public void Zapisz_Click(View v)
        {
            /// Zapisz przeprowadzone łączenia do pliku txt
            try
            {
                // Utwórz nazwę pliku i odczytaj tekst do zapisu
                string filename = "Łączenia " + typPrzedmiotu + ".txt";
                string text = FindViewById<TextView>(Resource.Id.textWynik).Text;

                // Utwórz ścierzkę do głównego folderu pamięci wewnętrznej urządzenia
                var documentsPath = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
                var filePath = Path.Combine(documentsPath, filename);
                System.IO.File.WriteAllText(filePath, text);

                // Poinformuj urzytkownika o pomyślnym zapisaniu
                Toast.MakeText(this, "Zapisano jako " + filename + "!", ToastLength.Short).Show();
            }
            catch
            {
                Toast.MakeText(this, "Nie udało się zapisać", ToastLength.Short).Show();
            }
        }

        [Export("Opcje_Click")]
        public void Opcje_Click(View v)
        {
            /// Zmień widok na widok Opcji
            ZmienWidok("Opcje");
        }

        private void WybierzOpcje(object sender, ItemClickEventArgs e)
        {
            // Wybrano "Dodaj przedmiot"
            if (e.Position == 0)
            {
                ZmienWidok("AddNewItem");
            }
            // Wybrano "Importuj listę przedmiotów"
            if (e.Position == 1)
            {
                ZmienWidok("LoadItems");
            }
            // Wybrano "Wyczyść listę przedmiotów"
            if (e.Position == 2)
            {
                przedmioty.Clear();
                Toast.MakeText(this, "Wyczyszczono listę przedmiotów!", ToastLength.Short).Show();
            }
            // Wybrano "Wyczyść przebieg łączenia"
            if (e.Position == 3)
            {
                wyniki.Clear();
                skladnik1 = null;
                skladnik2 = null;
                Toast.MakeText(this, "Wyczyszczono przebieg łączenia!", ToastLength.Short).Show();
            }
            // Wybrano "Tabele łączeń"
            if (e.Position == 4)
            {
                ZmienWidok("WybierzTable");
            }
        }

        [Export("DodajPrzedmiot")]
        public void DodajPrzedmiot(View v)
        {
            /// Funkcja dodająca nowy przedmiot do listy przedmiotów
            Item i = new Item();

            // Odczytaj wybrany prefiks, bazę i sufiks
            i.p = FindViewById<Spinner>(Resource.Id.spinnerPref).SelectedItemPosition;
            i.b = FindViewById<Spinner>(Resource.Id.spinnerBaza).SelectedItemPosition;
            i.s = FindViewById<Spinner>(Resource.Id.spinnerSuf).SelectedItemPosition;

            // Jeżeli nie wybrano żadnego prefiksu, bazy i sufiksu to poinformuj o tym urzytkownika i opuść funkcje
            if (i.Sum() == 0)
            {
                Toast.MakeText(this, "Próbujesz dodać pusty przedmiot!", ToastLength.Short).Show();
                return;
            }

            // Dodaj przedmiot do listy przedmiotów
            przedmioty.Add(new Item(i));

            // Wyczyść poprzednio wybrany prefiks, bazę i sufiks
            FindViewById<Spinner>(Resource.Id.spinnerPref).SetSelection(0);
            FindViewById<Spinner>(Resource.Id.spinnerBaza).SetSelection(0);
            FindViewById<Spinner>(Resource.Id.spinnerSuf).SetSelection(0);

            // Wyświetl informację "Dodano"
            Toast.MakeText(this, "Dodano", ToastLength.Short).Show();
        }

        [Export("ImportujPrzedmioty")]
        public void ImportujPrzedmioty(View v)
        {
            /// Funkcja do importowania przedmiotów
            switch (typPrzedmiotu)
            {
                case "Hełm":
                    Zaladuj(PrefHelm, BazaHelm, SufHelm);
                    break;
                case "Zbroja":
                    Zaladuj(PrefZbroja, BazaZbroja, SufZbroja);
                    break;
                case "Spodnie":
                    Zaladuj(PrefSpodnie, BazaSpodnie, SufSpodnie);
                    break;
                case "Pierścień":
                    Zaladuj(PrefPierscien, BazaPierscien, SufPierscien);
                    break;
                case "Amulet":
                    Zaladuj(PrefAmulet, BazaAmulet, SufAmulet);
                    break;
                case "Biała 1h":
                    Zaladuj(PrefBiala1h, BazaBiala1h, SufBiala1h);
                    break;
                case "Biała 2h":
                    Zaladuj(PrefBiala2h, BazaBiala2h, SufBiala2h);
                    break;
                case "Palna 1h":
                    Zaladuj(PrefPalan1h, BazaPalna1h, SufPalna1h);
                    break;
                case "Palna 2h":
                    Zaladuj(PrefPalna2h, BazaPalna2h, SufPalna2h);
                    break;
                case "Dystans":
                    // Normalna identyfikacja nie działa :/
                    //Zaladuj(PrefDystans, BazaDystans, SufDystans);
                    przedmioty.Clear();

                    string[] linie = FindViewById<EditText>(Resource.Id.textImportPrzedm).Text.Split('\n');

                    Item przedmiot = new Item();

                    foreach (string line in linie)
                    {
                        przedmiot = new Item();

                        for (int i = 0; i < PrefDystans.Count; i++)
                        {
                            if (line.Contains(PrefDystans[i])) przedmiot.p = i;
                        }
                        for (int i = 0; i < BazaDystans.Count; i++)
                        {
                            if (line.Contains(BazaDystans[i])) przedmiot.b = i;
                        }
                        for (int i = 0; i < SufDystans.Count; i++)
                        {
                            if (line.Contains(SufDystans[i])) przedmiot.s = i;
                        }


                        if (przedmiot.Sum() > 0)
                        {
                            przedmioty.Add(przedmiot);
                        }
                    }
                    break;
            }

            ZmienWidok("Main");
            Toast.MakeText(this, "Zaimportowano " + przedmioty.Count.ToString() + " przedmiotów", ToastLength.Short).Show();
        }

        [Export("WyswietlPref_Click")]
        public void WyswietlPref_Click(View v)
        {
            /// Kliknięto "Pokarz tabelę łączenia prefisków"
            string typ = typyPrzedmiotow[FindViewById<Spinner>(Resource.Id.spinnerTabelaTyp).SelectedItemPosition];

            ZmienWidok("Table", typ, "pref");
        }

        [Export("WyswietlBaza_Click")]
        public void WyswietlBaza_Click(View v)
        {
            /// Kliknięto "Pokarz tabelę łączenia baz"
            string typ = typyPrzedmiotow[FindViewById<Spinner>(Resource.Id.spinnerTabelaTyp).SelectedItemPosition];

            ZmienWidok("Table", typ, "baza");
        }

        [Export("WyswietlSuf_Click")]
        public void WyswietlSuf_Click(View v)
        {
            /// Kliknięto "Pokarz tabelę łączenia sufiksów"
            string typ = typyPrzedmiotow[FindViewById<Spinner>(Resource.Id.spinnerTabelaTyp).SelectedItemPosition];

            ZmienWidok("Table", typ, "suf");
        }

        [Export("Wroc_Click")]
        public void Wroc_Click(View v)
        {
            /// Powrót do widoku okna głównego
            ZmienWidok("Main");
        }

        private void Zaladuj(List<string> pref, List<string> baza, List<string> suf)
        {
            // Funkcja ładująca przedmioty - identyfikuje przedmioty z ciągu znaków
            // Wyczyść listy załadowanych przedmiotów
            przedmioty.Clear();

            // Podziel wklejony tekst na linie
            string[] linie = FindViewById<EditText>(Resource.Id.textImportPrzedm).Text.Split('\n');

            // Identyfikowany przedmiot
            Item przedmiot;

            // Pętla dzieląca tekst z okienka na poszczególne linie
            foreach (string line in linie)
            {
                przedmiot = new Item();
                // Podziel linię tekstu na wyrazy oddzielone spacjami
                string[] wyrazy = line.Split(' ');

                // Przeszukaj poszczególne wyrazy
                for (int i = 0; i < wyrazy.Count(); i++)
                {
                    // Pomiń spacje i inne wyrazy z ilością znaków < 3
                    if (wyrazy[i].Length < 3) continue;

                    // Przytnij końcówkę wyrazu - ułatwienie identyfikacji przedmiotów
                    wyrazy[i] = wyrazy[i].Substring(0, wyrazy[i].Length - 1);

                    // Odnajdź w liście prefiksów / baz / sufiksów wyraz
                    if (pref.Any(p => p.Contains(wyrazy[i])) && przedmiot.p == 0) przedmiot.p = pref.IndexOf(pref.Find(p => p.Contains(wyrazy[i])));
                    if (baza.Any(b => b.Contains(wyrazy[i]))) przedmiot.b = baza.IndexOf(baza.Find(b => b.Contains(wyrazy[i])));
                    if (suf.Any(s => s.Contains(wyrazy[i]))) przedmiot.s = suf.IndexOf(suf.Find(s => s.Contains(wyrazy[i])));
                }

                // Jeżeli znaleziono prefiks / bazę / sufiks to dodaj przedmiot do listy przedmiotów
                if (przedmiot.Sum() > 0)
                {
                    przedmioty.Add(przedmiot);
                }
            }
        }

        private void ZmienWidok(string widok, string typ = "", string baza = "")
        {
            /// Funkcja zmieniająca widoki
            /// Oprócz zmiany widoków dodaje elementy listview, spinnerów oraz zdarzenia z nimi powiązane

            switch (widok)
            {
                case "Main":
                    // Główny widok programu
                    SetContentView(Resource.Layout.Main);
                    // Po powrocie do głównego widoku dodaj ponownie zdarzenie do itemSelector i wybierz wcześniej wybrany typ przedmiotu
                    FindViewById<Spinner>(Resource.Id.itemSelector).ItemSelected += new EventHandler<ItemSelectedEventArgs>(ItemSelector_Click);
                    FindViewById<Spinner>(Resource.Id.itemSelector).SetSelection(typyPrzedmiotow.ToList().IndexOf(typPrzedmiotu));
                    // Wyświetl ponownie historię łączeń
                    if (wyniki.Count > 0) FindViewById<TextView>(Resource.Id.textWynik).Text = wyniki[wyniki.Count - 1].h;
                    break;
                case "AddItem":
                    // Widok wyboru przedmiotu do łączenia
                    SetContentView(Resource.Layout.AddItem);

                    // Zamień przedmioty z typu Item na string
                    listString = new List<string>();
                    listString = ItemsToString(przedmioty);

                    // Dodaj przedmioty do listView, dodaj zdarzenie kliknięcia na przedmiocie oraz ustaw opcję ChoiceMode.Single
                    FindViewById<ListView>(Resource.Id.listaPrzedmiotow).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleListItemSingleChoice, listString);
                    FindViewById<ListView>(Resource.Id.listaPrzedmiotow).ItemClick += new EventHandler<ItemClickEventArgs>(ListViewItemClick);
                    FindViewById<ListView>(Resource.Id.listaPrzedmiotow).ChoiceMode = ChoiceMode.Single;
                    break;
                case "Opcje":
                    // Widok listy opcji
                    SetContentView(Resource.Layout.Opcje);

                    // Dodaj elementy listy opcji oraz zdarzenie kliknięcia w element
                    FindViewById<ListView>(Resource.Id.listaOpcji).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSelectableListItem, listaOpcji);
                    FindViewById<ListView>(Resource.Id.listaOpcji).ItemClick += new EventHandler<ItemClickEventArgs>(WybierzOpcje);
                    break;
                case "AddNewItem":
                    // Widok dodawania nowych przedmiotów do listy przedmiotów
                    SetContentView(Resource.Layout.AddNewItem);

                    // W zależności od wybranego typu łączonego przedmiotu wypełnij spinnery odpowiednimi prefiksami, bazami i sufiksami
                    switch (typPrzedmiotu)
                    {
                        case "Hełm":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefHelm);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaHelm);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufHelm);
                            break;
                        case "Zbroja":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefZbroja);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaZbroja);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufZbroja);
                            break;
                        case "Spodnie":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefSpodnie);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaSpodnie);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufSpodnie);
                            break;
                        case "Pierścień":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefPierscien);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaPierscien);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufPierscien);
                            break;
                        case "Amulet":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefAmulet);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaAmulet);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufAmulet);
                            break;
                        case "Biała 1h":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefBiala1h);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaBiala1h);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufBiala1h);
                            break;
                        case "Biała 2h":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefBiala2h);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaBiala2h);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufBiala2h);
                            break;
                        case "Palna 1h":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefPalan1h);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaPalna1h);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufPalna1h);
                            break;
                        case "Palna 2h":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefPalna2h);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaPalna2h);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufPalna2h);
                            break;
                        case "Dystans":
                            FindViewById<Spinner>(Resource.Id.spinnerPref).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, PrefDystans);
                            FindViewById<Spinner>(Resource.Id.spinnerBaza).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, BazaDystans);
                            FindViewById<Spinner>(Resource.Id.spinnerSuf).Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, SufDystans);
                            break;
                    }
                    break;
                case "LoadItems":
                    // Widok importowania listy przedmiotów
                    SetContentView(Resource.Layout.LoadItems);
                    // Zaktualizuj typ importowanych przedmiotów
                    FindViewById<TextView>(Resource.Id.textImportTyp).Text = "Importujesz przedmioty typu: " + typPrzedmiotu;
                    break;
                case "WybierzTable":
                    // Widok wyboru tabeli łączeń
                    SetContentView(Resource.Layout.WybierzTable);
                    break;
                case "Table":
                    // Widok tabeli łączeń
                    SetContentView(Resource.Layout.Table);
                    // Zapełnij Tabelę
                    switch (typ)
                    {
                        case "Hełm":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefHelm);
                                    break;
                                case "baza":
                                    PopulateTable(BazaHelm);
                                    break;
                                case "suf":
                                    PopulateTable(SufHelm);
                                    break;
                            }
                            break;
                        case "Zbroja":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefZbroja);
                                    break;
                                case "baza":
                                    PopulateTable(BazaZbroja);
                                    break;
                                case "suf":
                                    PopulateTable(SufZbroja);
                                    break;
                            }
                            break;
                        case "Spodnie":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefSpodnie);
                                    break;
                                case "baza":
                                    PopulateTable(BazaSpodnie);
                                    break;
                                case "suf":
                                    PopulateTable(SufSpodnie);
                                    break;
                            }
                            break;
                        case "Pierścień":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefPierscien);
                                    break;
                                case "baza":
                                    PopulateTable(BazaPierscien);
                                    break;
                                case "suf":
                                    PopulateTable(SufPierscien);
                                    break;
                            }
                            break;
                        case "Amulet":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefAmulet);
                                    break;
                                case "baza":
                                    PopulateTable(BazaAmulet);
                                    break;
                                case "suf":
                                    PopulateTable(SufAmulet);
                                    break;
                            }
                            break;
                        case "Biała 1h":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefBiala1h);
                                    break;
                                case "baza":
                                    PopulateTable(BazaBiala1h);
                                    break;
                                case "suf":
                                    PopulateTable(SufBiala1h);
                                    break;
                            }
                            break;
                        case "Biała 2h":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefBiala2h);
                                    break;
                                case "baza":
                                    PopulateTable(BazaBiala2h);
                                    break;
                                case "suf":
                                    PopulateTable(SufBiala2h);
                                    break;
                            }
                            break;
                        case "Palna 1h":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefPalan1h);
                                    break;
                                case "baza":
                                    PopulateTable(BazaPalna1h);
                                    break;
                                case "suf":
                                    PopulateTable(SufPalna1h);
                                    break;
                            }
                            break;
                        case "Palna 2h":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefPalna2h);
                                    break;
                                case "baza":
                                    PopulateTable(BazaPalna2h);
                                    break;
                                case "suf":
                                    PopulateTable(SufPalna2h);
                                    break;
                            }
                            break;
                        case "Dystans":
                            switch (baza)
                            {
                                case "pref":
                                    PopulateTable(PrefDystans);
                                    break;
                                case "baza":
                                    PopulateTable(BazaDystans);
                                    break;
                                case "suf":
                                    PopulateTable(SufDystans);
                                    break;
                            }
                            break;
                    }
                    break;
            }
        }

        private List<string> GenerateTable(List<string> baza)
        {
            /// Funkcja do generowania tabeli łączeń
            /// Na podstawie listy prefiksów / baz / sufiksów łączy każdy z każdym i zapisuje do listy
            List<string> ls = new List<string>();
            Item i1, i2;
            int wynik = 0;

            for (int i = 0; i < baza.Count; i++)
            {
                i1 = new Item(0, i, 0);

                for (int j = 0; j < baza.Count; j++)
                {
                    if (i == 0) ls.Add(baza.ElementAt(j));
                    else if (j == 0) ls.Add(baza.ElementAt(i));
                    else
                    {
                        i2 = new Item(0, j, 0);
                        wynik = i1.Polacz(i2, null, baza, null).b;

                        ls.Add(baza.ElementAt(wynik));
                    }
                }
            }

            return ls;
        }

        private void PopulateTable(List<string> ls)
        {
            /// Funkcja do zapełnienia tabeli łączeń
            FindViewById<LinearLayout>(Resource.Id.linearLayout_gridtableLayout).LayoutParameters.Width = 400 * ls.Count;
            FindViewById<GridView>(Resource.Id.tabela).Adapter = new ArrayAdapter(this, Android.Resource.Layout.SimpleDropDownItem1Line, GenerateTable(ls));
            FindViewById<GridView>(Resource.Id.tabela).NumColumns = ls.Count;
            FindViewById<GridView>(Resource.Id.tabela).TextAlignment = TextAlignment.Center;
            FindViewById<GridView>(Resource.Id.tabela).SetGravity(GravityFlags.Center);
        }

        public void ListViewItemClick(object sender, ItemClickEventArgs e)
        {
            // Zaznacz kliknięty przedmiot
            FindViewById<ListView>(Resource.Id.listaPrzedmiotow).SetSelection(e.Position);
        }

        private List<string> ItemsToString(List<Item> listItem)
        {
            // Funkcja konwertująca List<Item> na List<string> w oparciu o wybrany typ przedmiotu do łączenia
            List<string> ls = new List<string>();

            switch (typPrzedmiotu)
            {
                case "Hełm":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefHelm, BazaHelm, SufHelm));
                    break;
                case "Zbroja":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefZbroja, BazaZbroja, SufZbroja));
                    break;
                case "Spodnie":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefSpodnie, BazaSpodnie, SufSpodnie));
                    break;
                case "Pierścień":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefPierscien, BazaPierscien, SufPierscien));
                    break;
                case "Amulet":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefAmulet, BazaAmulet, SufAmulet));
                    break;
                case "Biała 1h":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefBiala1h, BazaBiala1h, SufBiala1h));
                    break;
                case "Biała 2h":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefBiala2h, BazaBiala2h, SufBiala2h));
                    break;
                case "Palna 1h":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefPalan1h, BazaPalna1h, SufPalna1h));
                    break;
                case "Palna 2h":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefPalna2h, BazaPalna2h, SufPalna2h));
                    break;
                case "Dystans":
                    foreach (Item i in listItem) ls.Add(UsunSpacje(i, PrefDystans, BazaDystans, SufDystans));
                    break;
            }

            return ls;
        }

        private string UsunSpacje(Item i, List<string> pref, List<string> baza, List<string> suf)
        {
            // Funkcja zwracająca prefiks, bazę i sufiks przedmiotu w postaci stringa bez niepotrzebnych spacji
            string s = "";
            if (i.p != 0) s += pref.ElementAt(i.p);
            if (i.p != 0 && i.b != 0) s += " ";
            if (i.b != 0) s += baza.ElementAt(i.b);
            if ((i.p != 0 || i.b != 0) && i.s != 0) s += " ";
            if (i.s != 0) s += suf.ElementAt(i.s);
            return s;
        }

        private void BazaHelmow(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy hełmu w uporządkowanej kolejności
            pref.Add("");
            pref.Add("Utwardzany");
            pref.Add("Wzmocniony");
            pref.Add("Pomocny");
            pref.Add("Ozdobny");
            pref.Add("Elegancki");
            pref.Add("Rogaty");
            pref.Add("Złośliwy");
            pref.Add("Leniwy");
            pref.Add("Śmiercionośny");
            pref.Add("Bojowy");
            pref.Add("Magnetyczny");
            pref.Add("Krwawy");
            pref.Add("Kunsztowny");
            pref.Add("Kuloodporny");
            pref.Add("Szamański");
            pref.Add("Tygrysi");
            pref.Add("Szturmowy");
            pref.Add("Runiczny");
            pref.Add("Rytualny");

            // Bazy hełmu w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Czapka");
            baza.Add("Kask");
            baza.Add("Hełm");
            baza.Add("Maska");
            baza.Add("Obręcz");
            baza.Add("Kominiarka");
            baza.Add("Kapelusz");
            baza.Add("Opaska");
            baza.Add("Bandana");
            baza.Add("Korona");

            // Sufiksy hełmu w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Podróżnika");
            suf.Add("Przezorności");
            suf.Add("Wytrzymałości");
            suf.Add("Pasterza");
            suf.Add("Narkomana");
            suf.Add("Ochrony");
            suf.Add("Zmysłów");
            suf.Add("Wieszcza");
            suf.Add("Kary");
            suf.Add("Gladiatora");
            suf.Add("Krwi");
            suf.Add("Skorupy żółwia");
            suf.Add("Słońca");
            suf.Add("Adrenaliny");
            suf.Add("Prekognicji");
            suf.Add("Smoczej Łuski");
            suf.Add("Mocy");
            suf.Add("Magii");
        }

        private void BazaZbroi(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy zbroi w uporządkowanej kolejności
            pref.Add("");
            pref.Add("Wzmocniony");
            pref.Add("Ćwiekowany");
            pref.Add("Władczy");
            pref.Add("Lekki");
            pref.Add("Łuskowy");
            pref.Add("Bojowy");
            pref.Add("Płytowy");
            pref.Add("Giętki");
            pref.Add("Krwawy");
            pref.Add("Łowiecki");
            pref.Add("Szamański");
            pref.Add("Kuloodporny");
            pref.Add("Tygrysi");
            pref.Add("Elfi");
            pref.Add("Runiczny");
            pref.Add("Śmiercionośny");

            // Bazy zbroi w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Koszulka");
            baza.Add("Kurtka");
            baza.Add("Marynarka");
            baza.Add("Kamizelka");
            baza.Add("Gorset");
            baza.Add("Peleryna");
            baza.Add("Smoking");
            baza.Add("Kolczuga");
            baza.Add("Zbroja warstwowa");
            baza.Add("Pełna zbroja");

            // Sufiksy zbroi w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Adepta");
            suf.Add("Strażnika");
            suf.Add("Złodzieja");
            suf.Add("Siłacza");
            suf.Add("Narkomana");
            suf.Add("Szermierza");
            suf.Add("Zabójcy");
            suf.Add("Gwardzisty");
            suf.Add("Kobry");
            suf.Add("Skorupy żółwia");
            suf.Add("Uników");
            suf.Add("Grabieżcy");
            suf.Add("Mistrza");
            suf.Add("Adrenaliny");
            suf.Add("Centuriona");
            suf.Add("Odporności");
            suf.Add("Kaliguli");
            suf.Add("Siewcy Śmierci");
            suf.Add("Szybkości");
            suf.Add("Orchidei");
        }

        private void BazaSpodni(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy spodni w uporządkowanej kolejności
            pref.Add("");
            pref.Add("Krótkie");
            pref.Add("Pikowane");
            pref.Add("Lekkie");
            pref.Add("Wzmocnione");
            pref.Add("Aksamitne");
            pref.Add("Ćwiekowane");
            pref.Add("Kuloodporne");
            pref.Add("Giętkie");
            pref.Add("Kolcze");
            pref.Add("Szamańskie");
            pref.Add("Krwawe");
            pref.Add("Elfie");
            pref.Add("Tygrysie");
            pref.Add("Pancerne");
            pref.Add("Runiczne");
            pref.Add("Kompozytowe");
            pref.Add("Śmiercionośne");

            // Bazy spodni w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Szorty");
            baza.Add("Spodnie");
            baza.Add("Spódnica");
            baza.Add("Kilt");

            // Sufiksy spodni w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Rzezimieszka");
            suf.Add("Przemytnika");
            suf.Add("Narkomana");
            suf.Add("Siłacza");
            suf.Add("Cichych Ruchów");
            suf.Add("Uników");
            suf.Add("Skrytości");
            suf.Add("Słońca");
            suf.Add("Handlarza Bronią");
            suf.Add("Pasterza");
            suf.Add("Łowcy Cieni");
            suf.Add("Węża");
            suf.Add("Inków");
            suf.Add("Tropiciela");
            suf.Add("Nocy");
        }

        private void BazaPierścieni(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy pierścieni w uporządkowanej kolejności
            pref.Add("");
            pref.Add("Miedziany");
            pref.Add("Srebrny");
            pref.Add("Szmaragdowy");
            pref.Add("Złoty");
            pref.Add("Platynowy");
            pref.Add("Rubinowy");
            pref.Add("Dystyngowany");
            pref.Add("Przebiegły");
            pref.Add("Kardynalski");
            pref.Add("Elastyczny");
            pref.Add("Nekromancki");
            pref.Add("Gwiezdny");
            pref.Add("Niedźwiedzi");
            pref.Add("Twardy");
            pref.Add("Zwierzęcy");
            pref.Add("Tańczący");
            pref.Add("Archaiczny");
            pref.Add("Hipnotyczny");
            pref.Add("Diamentowy");
            pref.Add("Mściwy");
            pref.Add("Spaczony");
            pref.Add("Plastikowy");
            pref.Add("Zdradziecki");
            pref.Add("Tytanowy");
            pref.Add("Słoneczny");
            pref.Add("Pajęczy");
            pref.Add("Jastrzębi");
            pref.Add("Czarny");

            // Bazy pierścieni w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Pierścień");
            baza.Add("Sygnet");
            baza.Add("Bransoleta");

            // Sufiksy pierścieni w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Występku");
            suf.Add("Urody");
            suf.Add("Władzy");
            suf.Add("Siły");
            suf.Add("Geniuszu");
            suf.Add("Mądrości");
            suf.Add("Twardej Skóry");
            suf.Add("Wilkołaka");
            suf.Add("Sztuki");
            suf.Add("Celności");
            suf.Add("Młodości");
            suf.Add("Lisa");
            suf.Add("Szczęścia");
            suf.Add("Krwi");
            suf.Add("Nietoperza");
            suf.Add("Koncentracji");
            suf.Add("Lewitacji");
            suf.Add("Przebiegłości");
            suf.Add("Szaleńca");
            suf.Add("Łatwości");
        }

        private void BazaAmuletow(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy amuletów w uporządkowanej kolejności
            pref.Add("");
            pref.Add("Miedziany");
            pref.Add("Srebrny");
            pref.Add("Szmaragdowy");
            pref.Add("Złoty");
            pref.Add("Platynowy");
            pref.Add("Rubinowy");
            pref.Add("Dystyngowany");
            pref.Add("Przebiegły");
            pref.Add("Kardynalski");
            pref.Add("Elastyczny");
            pref.Add("Nekromancki");
            pref.Add("Gwiezdny");
            pref.Add("Niedźwiedzi");
            pref.Add("Twardy");
            pref.Add("Diamentowy");
            pref.Add("Mściwy");
            pref.Add("Archaiczny");
            pref.Add("Tańczący");
            pref.Add("Hipnotyczny");
            pref.Add("Zwierzęcy");
            pref.Add("Spaczony");
            pref.Add("Plastikowy");
            pref.Add("Zdradziecki");
            pref.Add("Tytanowy");
            pref.Add("Słoneczny");
            pref.Add("Pajęczy");
            pref.Add("Jastrzębi");
            pref.Add("Czarny");

            // Bazy amuletów w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Naszyjnik");
            baza.Add("Amulet");
            baza.Add("Łańcuch");
            baza.Add("Apaszka");
            baza.Add("Krawat");

            // Sufiksy amuletów w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Występku");
            suf.Add("Urody");
            suf.Add("Władzy");
            suf.Add("Geniuszu");
            suf.Add("Siły");
            suf.Add("Mądrości");
            suf.Add("Twardej Skóry");
            suf.Add("Pielgrzyma");
            suf.Add("Wilkołaka");
            suf.Add("Celności");
            suf.Add("Sztuki");
            suf.Add("Młodości");
            suf.Add("Szczęścia");
            suf.Add("Krwi");
            suf.Add("Zdolności");
            suf.Add("Koncentracji");
            suf.Add("Lewitacji");
            suf.Add("Przebiegłości");
            suf.Add("Szaleńca");
            suf.Add("Łatwości");
        }

        private void BazaBialych1h(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy białej 1h w uporządkowanej kolejności
            pref.Add("");
            pref.Add("Ostry");
            pref.Add("Zębaty");
            pref.Add("Kościany");
            pref.Add("Wzmacniający");
            pref.Add("Kryształowy");
            pref.Add("Mistyczny");
            pref.Add("Lekki");
            pref.Add("Okrutny");
            pref.Add("Przyjacielski");
            pref.Add("Kąsający");
            pref.Add("Opiekuńczy");
            pref.Add("Świecący");
            pref.Add("Jadowity");
            pref.Add("Zabójczy");
            pref.Add("Zatruty");
            pref.Add("Przeklęty");
            pref.Add("Zwinny");
            pref.Add("Antyczny");
            pref.Add("Szybki");
            pref.Add("Demoniczny");

            // Bazy białej 1h w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Pałka");
            baza.Add("Nóż");
            baza.Add("Sztylet");
            baza.Add("Kastet");
            baza.Add("Miecz");
            baza.Add("Rapier");
            baza.Add("Kama");
            baza.Add("Topór");
            baza.Add("Wakizashi");
            baza.Add("Pięść Niebios");

            // Sufiksy białej 1h w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Dowódcy");
            suf.Add("Sekty");
            suf.Add("Bólu");
            suf.Add("Władzy");
            suf.Add("Zwinności");
            suf.Add("Mocy");
            suf.Add("Zarazy");
            suf.Add("Odwagi");
            suf.Add("Trafienia");
            suf.Add("Przodków");
            suf.Add("Zdobywcy");
            suf.Add("Kontuzji");
            suf.Add("Męstwa");
            suf.Add("Precyzji");
            suf.Add("Krwi");
            suf.Add("Zemsty");
            suf.Add("Podkowy");
            suf.Add("Drakuli");
            suf.Add("Biegłości");
            suf.Add("Klanu");
            suf.Add("Imperatora");
            suf.Add("Samobójcy");
        }

        private void BazaBialych2h(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy białej 2h w uporządkowanej kolejności
            pref.Add("");
            pref.Add("Kosztowny");
            pref.Add("Ostry");
            pref.Add("Kryształowy");
            pref.Add("Zębaty");
            pref.Add("Szeroki");
            pref.Add("Okrutny");
            pref.Add("Mistyczny");
            pref.Add("Wzmacniający");
            pref.Add("Kąsający");
            pref.Add("Lekki");
            pref.Add("Ciężki");
            pref.Add("Zatruty");
            pref.Add("Napromieniowany");
            pref.Add("Świecący");
            pref.Add("Opiekuńczy");
            pref.Add("Jadowity");
            pref.Add("Zabójczy");
            pref.Add("Przeklęty");
            pref.Add("Zwinny");
            pref.Add("Antyczny");
            pref.Add("Demoniczny");

            // Bazy białej 2h w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Maczuga");
            baza.Add("Łom");
            baza.Add("Miecz dwuręczny");
            baza.Add("Topór dwuręczny");
            baza.Add("Korbacz");
            baza.Add("Kosa");
            baza.Add("Pika");
            baza.Add("Halabarda");
            baza.Add("Katana");
            baza.Add("Piła łańcuchowa");

            // Sufiksy białej 2h w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Zdrady");
            suf.Add("Podstępu");
            suf.Add("Bólu");
            suf.Add("Hazardzisty");
            suf.Add("Ołowiu");
            suf.Add("Mocy");
            suf.Add("Inkwizytora");
            suf.Add("Krwiopijcy");
            suf.Add("Zdobywcy");
            suf.Add("Władzy");
            suf.Add("Zemsty");
            suf.Add("Zarazy");
            suf.Add("Podkowy");
            suf.Add("Autokraty");
            suf.Add("Krwi");
            suf.Add("Bazyliszka");
            suf.Add("Samobójcy");
            suf.Add("Drakuli");
        }

        private void BazaPalnych1h(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy palnych 1h w uporządkowanej kolejności
            pref.Add("");

            // Bazy palnych 1h w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Glock");
            baza.Add("Beretta");
            baza.Add("Uzi");
            baza.Add("Magnum");
            baza.Add("Desert Eagle");
            baza.Add("Mp5k");
            baza.Add("Skorpion");

            // Sufiksy palnych 1h w uporządkowanej kolejności
            suf.Add("");
        }

        private void BazaPalnych2h(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy palnych 2h w uporządkowanej kolejności
            pref.Add("");

            // Bazy palnych 2h w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Karabin myśliwski");
            baza.Add("Półautomat snajperski");
            baza.Add("Karabin snajperski");
            baza.Add("AK-47");
            baza.Add("Fn-Fal");
            baza.Add("Strzelba");
            baza.Add("Miotacz płomieni");

            // Sufiksy palnych 2h w uporządkowanej kolejności
            suf.Add("");
        }

        private void BazaDystansow(List<string> pref, List<string> baza, List<string> suf)
        {
            // Prefiksy dystansu w uporządkowanej kolejności
            pref.Add("");

            // Bazy dystansu w uporządkowanej kolejności
            baza.Add("");
            baza.Add("Krótki łuk");
            baza.Add("Łuk");
            baza.Add("Shuriken");
            baza.Add("Długi łuk");
            baza.Add("Kusza");
            baza.Add("Nóż do rzucania");
            baza.Add("Łuk refleksyjny");
            baza.Add("Oszczep");
            baza.Add("Pilum");
            baza.Add("Toporek do rzucania");
            baza.Add("Ciężka kusza");

            // Sufiksy dystansu w uporządkowanej kolejności
            suf.Add("");
            suf.Add("Dalekiego zasięgu");
            suf.Add("Doskonałości");
            suf.Add("Precyzji");
            suf.Add("Zemsty");
            suf.Add("Reakcji");
            suf.Add("Driady");
            suf.Add("Szybkostrzelności");
            suf.Add("Wilka");
        }
    }
}

