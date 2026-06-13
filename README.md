# 🏠 SmartHome - Asynchroniczny Symulator Systemu Inteligentnego Domu

Projekt realizowany w ramach przedmiotu **Zaawansowane Programowanie Obiektowe** (Informatyka, Semestr V). Aplikacja symuluje działanie centralnego huba zarządzającego inteligentnymi urządzeniami domowymi, łącząc nowoczesny interfejs tekstowy z zaawansowanymi mechanizmami wielowątkowości w platformie .NET.

---

## 🛠️ Wykorzystane Technologie i Koncepcje Obiektowe

Projekt został zaprojektowany z naciskiem na czysty kod (*Clean Code*), modularność oraz pełne wykorzystanie możliwości języka C#. W aplikacji zaimplementowano:

### 1. Abstrakcja i Polimorfizm
* Kod opiera się na ścisłym kontrakcie interfejsu `IDevice` oraz klasie abstrakcyjnej `Device`.
* Klasa bazowa dostarcza wspólną logikę biznesową (np. stan urządzenia, pokój), a konkretne podklasy (np. `SmartLight`) definiują specyficzne zachowania obiektów.

### 2. Przeciążanie Operatorów
* Zarządzanie kolekcją urządzeń wewnątrz huba (`SmartHomeHub`) odbywa się za pomocą przeciążonego operatora `+=`. Umożliwia to intuicyjną rejestrację nowych węzłów w systemie:
  ```csharp
  myHome += new SmartLight("LGT-01", "Ceiling Light", "Kitchen");
  ```

### 3. Indeksatory (Indexers)
* Klasa centralna udostępnia dedykowany indeksator tekstowy do błyskawicznego wyszukiwania urządzeń po ich unikalnym identyfikatorze (ID).
* Mechanizm został zoptymalizowany pod kątem braku wrażliwości na wielkość liter (*case-insensitivity*), dzięki czemu `lgt-01` oraz `LGT-01` wskazują na ten sam obiekt.

### 4. Przetwarzanie Danych za pomocą LINQ
* Filtrowanie urządzeń przypisanych do konkretnych pomieszczeń realizowane jest za pomocą deklaratywnych zapytań LINQ.
* Lista pokojów w menu generowana jest w pełni dynamicznie w locie (na podstawie aktualnie podpiętych urządzeń) przy użyciu metod `.Select()` oraz `.Distinct()`, co zapobiega dublowaniu pozycji.

### 5. Asynchroniczność i Wielwątkowość (Async/Await)
* Każde urządzenie po uruchomieniu systemu inicjuje niezależną, asynchroniczną pętlę symulacji pracy w tle (`Task.Run` / `Async`).
* Wątki tła w sposób losowy generują zdarzenia awarii sprzętowych, działając w pełni współbieżnie i nie blokując głównego wątku interfejsu użytkownika.
* Cykl życia zadań kontrolowany jest bezpiecznie przy użyciu mechanizmu `CancellationTokenSource`.

### 6. Programowanie Sterowane Zdarzeniami (Events)
* Komunikacja asynchroniczna z warstwą prezentacji CLI bazuje na architekturze zdarzeniowej.
* Hub subskrybuje zdarzenie `OnFailure` (wykorzystujące dedykowany delegat `EventHandler`), które przesyła szczegółowe metadane o usterce (w tym dokładny znacznik czasu *Timestamp*).

### 7. Bezpieczeństwo Wątkowe (Thread Safety)
* Ze względu na współbieżny napływ alertów z wielu niezależnych wątków tła, dostęp do wspólnej kolekcji logów został zabezpieczony przed zjawiskiem wyścigu (*Race Condition*) za pomocą instrukcji synchronicznej `lock` (zapewnienie reguły wzajemnego wykluczania).

### 8. Nowoczesny Interfejs TUI (Text User Interface)
* Warstwa wizualna została zbudowana w oparciu o bibliotekę `Spectre.Console`.
* Klasyczne, tekstowe wpisywanie komend zastąpiono **interaktywnym menu sterowanym strzałkami klawiatury** (`SelectionPrompt`).
* Dane urządzeń wyświetlane są w automatycznie formatowanej, estetycznej tabeli ANSI, a krytyczne awarie prezentowane są w dedykowanym panelu ostrzegawczym (*Dashboard*).

---

## 📂 Struktura Projektu

Projekt podzielony jest na logiczne warstwy (zgodnie ze sztuką podziału odpowiedzialności):
* **`SmartHome.Core`** – Biblioteka klas zawierająca całą logikę biznesową, interfejsy, modele urządzeń, hub oraz mechanizmy symulacji.
* **`SmartHome.CLI`** – Aplikacja konsolowa stanowiąca warstwę prezentacji (TUI) i integrująca bibliotekę Spectre.Console.

---

## 🚀 Jak uruchomić projekt?

1. Upewnij się, że masz zainstalowane środowisko **.NET SDK** (wersja 6.0 lub nowsza).
2. Sklonuj repozytorium:
   ```bash
   git clone https://github.com/gdx173806/SmartHome.git
   ```
3. Przejdź do folderu projektu:
   ```bash
   cd SmartHome
   ```
4. Uruchom aplikację (niezbędne pakiety NuGet pobiorą i skompilują się automatycznie):
   ```bash
   dotnet run --project SmartHome.CLI
   ```

---
_Projekt przygotowany na zaliczenie laboratorium z Zaawansowanego Programowania Obiektowego._