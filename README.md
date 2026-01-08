# 📈 StockMarket Broker API

> Aplikacja backendowa symulująca działanie domu maklerskiego, umożliwiająca handel akcjami w oparciu o rzeczywiste i historyczne dane rynkowe.

## 📋 Opis Projektu
System "Broker API" to rozbudowana aplikacja REST API stworzona w technologii **.NET 10**, służąca do symulacji dziennych inwestycji giełdowych.

Projekt realizuje zaawansowaną logikę biznesową (obliczanie średniej ceny zakupu, realizacja zysków/strat, obsługa portfela) oraz integruje się z zewnętrznym dostawcą danych rynkowych (AlphaVantage -> https://www.alphavantage.co)

### Kluczowe funkcjonalności:
* **Architektura:** REST API (ASP.NET Core).
* **Baza Danych:** Podejście **Database First** (SQL Server).
* **Bezpieczeństwo:** JWT
* **Role:** Podział na role `Administrator` i `User`.


## 🛠 Technologie
* **Backend:** .NET 10 (C#)
* **Framework:** ASP.NET Core Web API
* **Baza danych:** Microsoft SQL Server
* **ORM:** Entity Framework Core (Database First)
* **Integracje:** `HttpClient` do komunikacji z AlphaVantage API
* **Testy:** xUnit (Testy jednostkowe logiki biznesowej)
* **Dokumentacja API:** Swagger

## 🚀 Funkcjonalności Systemu

### 1. Moduł Użytkownika (Inwestora)
* **Rejestracja i Logowanie:** Bezpieczne zakładanie konta z hashowaniem haseł (BCrypt) i generowaniem tokena JWT.
* **Zarządzanie Portfelem:** Podgląd aktualnego salda, posiadanych akcji oraz wyceny portfela w czasie rzeczywistym.
* **Składanie Zleceń (Logika Biznesowa):**
    * **Kupno (BUY):** System weryfikuje saldo, pobiera aktualną cenę, aktualizuje średnią cenę zakupu (Weighted Average Cost) i zapisuje pozycję.
    * **Sprzedaż (SELL):** System weryfikuje stan posiadania, oblicza zrealizowany zysk/stratę (Profit/Loss) i aktualizuje historię transakcji.
* **Historia Transakcji:** Przegląd zamkniętych pozycji i operacji finansowych.

### 2. Moduł Administratora
* **Zarządzanie Rynkiem:** Dodawanie nowych spółek (Tickerów) do obrotu.
* **Zarządzanie Użytkownikami:** Podgląd listy inwestorów, usuwanie kont.
* **Raportowanie:** Generowanie zaawansowanych raportów statystycznych z wykorzystaniem **Procedur Składowanych SQL** (np. `sp_GetSystemStats`, `sp_GetAdminUserReport`).

