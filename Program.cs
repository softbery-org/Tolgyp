// Version: 0.1.0
/* 
 * LICENSE MIT License
 * 
 * Copyright (c) 2025 Softbery by Paweł Tobis
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
// W pliku Program.cs
using System;
using System.Linq;
using Tolgyp.lang; // Załóżmy, że to Twoja przestrzeń nazw

/// <summary>
/// Główna klasa programu.
/// </summary>
public class Program
{
    /// <summary>
    /// Główna metoda uruchamiająca program.
    /// </summary>
    /// <param name="args">Argumenty powłoki</param>
    /// <returns>Główne zadanie</returns>
    public static async Task Main(string[] args)
    {
        if (args.Length == 0 || args.Contains("-h") || args.Contains("--help"))
        {
            ShowHelp();
            return;
        }

        if (args.Contains("-v") || args.Contains("--version"))
        {
            ShowVersion();
            return;
        }

        // Zakładamy, że ostatni argument to tekst do tłumaczenia,
        // jeśli nie jest to opcja.
        // Bardziej zaawansowane parsowanie byłoby lepsze dla opcji typu --detect-lang
        string textToTranslate = args.LastOrDefault(arg => !arg.StartsWith("-"));

        if (string.IsNullOrWhiteSpace(textToTranslate))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("[Tolgyp]: Błąd: Nie podano tekstu do przetłumaczena.");
            Console.ForegroundColor = ConsoleColor.White;
            ShowHelp();
            return;
        }

        // Przykład użycia --detect-lang (jeśli zaimplementujesz)
        bool noTranslate = args.Contains("--detect-lang");

        Language languageService = new Language();

        if (noTranslate)
        {
            Console.WriteLine($"[Tolgyp]: Wykrywanie języka dla: \"{textToTranslate}\"");
            await languageService.DetectLanguageAsync(textToTranslate);
            // Wynik detekcji jest już wyświetlany wewnątrz DetectLanguageAsync
        }
        else
        {
            Console.WriteLine($"[Tolgyp]: Tłumaczenie tekstu: \"{textToTranslate}\"");
            string translatedText = await languageService.TranslateAsync(textToTranslate);
            // Wynik tłumaczenia jest już wyświetlany wewnątrz TranslateAsync
            // Możesz chcieć go tu jeszcze raz wyświetlić lub zwrócić jako kod wyjścia
            // Console.WriteLine($"\n[Tolgyp]: Finalny przetłumaczony tekst:\n{translatedText}");
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine(@"
Tolgyp - Narzędzie do Tłumaczenia Tekstu (wersja 0.1.0)

OPIS:
    Tolgyp.exe to narzędzie konsolowe służące do automatycznego wykrywania języka
    podanego tekstu oraz jego tłumaczenia. Program specjalizuje się w tłumaczeniu
    pomiędzy językiem polskim (pl) a angielskim (en). Jeśli wykryty zostanie
    język polski, tekst zostanie przetłumaczony na angielski. W przypadku wykrycia
    innego języka (domyślnie traktowanego jako angielski lub język do przetłumaczania
    na polski), tekst zostanie przetłumaczony na język polski.
    Narzędzie korzysta z Google Cloud Translation API.

SKŁADNIA:
    Tolgyp.exe [OPCJE] ""TEKST DO PRZETŁUMACZENIA""

ARGUMENTY:
    ""TEKST DO PRZETŁUMACZENIA""
        Tekst, który ma zostać poddany detekcji języka i tłumaczeniu.
        Jeśli tekst zawiera spacje, należy ująć go w cudzysłów.
        Ten argument jest wymagany do wykonania tłumaczenia.

OPCJE:
    -h, --help
        Wyświetla tę wiadomość pomocy i kończy działanie programu.

    -v, --version
        Wyświetla informacje o wersji programu i kończy działanie.
    
    --detect-lang
        Tylko wykrywa język podanego tekstu, bez wykonywania tłumaczenia.

PRZYKŁADY:
    1. Przetłumacz tekst z polskiego na angielski:
       Tolgyp.exe ""Witaj świecie, jak się masz?""

    2. Przetłumacz tekst z angielskiego na polski:
       Tolgyp.exe ""Hello world, how are you?""

    3. Wykryj język bez tłumaczenia:
       Tolgyp.exe --detect-lang ""Bonjour le monde""

    4. Wyświetl pomoc:
       Tolgyp.exe -h

WYMAGANIA:
    Aby program działał poprawnie, środowisko, w którym jest uruchamiany,
    musi być skonfigurowane do uwierzytelniania z usługami Google Cloud.
    Zazwyczaj oznacza to ustawienie zmiennej środowiskowej
    `GOOGLE_APPLICATION_CREDENTIALS` wskazującej na plik klucza konta usługi
    lub zalogowanie się za pomocą `gcloud auth application-default login`.

UWAGI:
    - Wyniki detekcji języka oraz tłumaczenia są wyświetlane na standardowym wyjściu.
    - Ewentualne błędy podczas komunikacji z API lub przetwarzania są logowane
      do pliku ""error.log"" w katalogu uruchomienia programu oraz wyświetlane
      na konsoli.
        ");
    }

    private static void ShowVersion()
    {
        // Wersję można pobrać np. z AssemblyInfo
        Console.WriteLine("Tolgyp.exe wersja 0.1.0"); // Przykładowa wersja
        Console.WriteLine("Copyright (C) 2025 Softbery by Paweł Tobis");
        Console.WriteLine("Korzysta z Google Cloud Translation API.");
    }
}
