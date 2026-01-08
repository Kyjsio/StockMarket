# 📈 StockMarket Broker API
<img width="1818" height="834" alt="image" src="https://github.com/user-attachments/assets/919965b0-a5d2-48d9-98bc-8a700ea7aa0c" />

> Aplikacja backendowa symulująca działanie domu maklerskiego, umożliwiająca handel akcjami w oparciu o dzienne dane rynkowe.

## 📋 Opis Projektu
System "Broker API" to rozbudowana aplikacja REST API stworzona w technologii **.NET 10**, służąca do symulacji dziennych inwestycji giełdowych.

Projekt realizuje zaawansowaną logikę biznesową (obliczanie średniej ceny zakupu, realizacja zysków/strat, obsługa portfela) oraz integruje się z zewnętrznym dostawcą danych rynkowych (AlphaVantage -> https://www.alphavantage.co)

### Kluczowe funkcjonalności:
* **Architektura:** REST API (ASP.NET Core).
* **Baza Danych:** Podejście **Database First** (SQL Server).
* **Bezpieczeństwo:** JWT
* **Role:** Podział na role `Administrator` i `User`.
<img width="1306" height="767" alt="image" src="https://github.com/user-attachments/assets/f5cd0b73-961f-4c16-b55a-3f100f52a924" />


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
* **Zarządzanie Portfelem:** Podgląd aktualnego salda, posiadanych akcji oraz wyceny portfela.
 <img width="1813" height="318" alt="image" src="https://github.com/user-attachments/assets/eab89d11-922a-4108-95af-a4c88426d597" />

* **Składanie Zleceń (Logika Biznesowa):**
    * **Kupno (BUY):** System weryfikuje saldo, pobiera aktualną cenę, aktualizuje średnią cenę zakupu (Weighted Average Cost) i zapisuje pozycję.
    * **Sprzedaż (SELL):** System weryfikuje stan posiadania, oblicza zrealizowany zysk/stratę (Profit/Loss) i aktualizuje historię transakcji.
* **Historia Transakcji:** Przegląd zamkniętych pozycji i operacji finansowych.
<img width="1689" height="348" alt="image" src="https://github.com/user-attachments/assets/7e9fac56-81b9-43de-97ab-3bcea5785de5" />

### 2. Moduł Administratora
* **Zarządzanie Rynkiem:** Dodawanie nowych spółek (Tickerów) do obrotu.
 <img width="1785" height="852" alt="image" src="https://github.com/user-attachments/assets/2dbf43b4-2f5d-4ab0-bba0-9957fb545436" />

* **Zarządzanie Użytkownikami:** Podgląd listy inwestorów, usuwanie kont.
<img width="1666" height="362" alt="image" src="https://github.com/user-attachments/assets/28e87d0b-970f-4460-9826-b84158c3a2f7" />


* **Raportowanie:** Generowanie raportów statystycznych z wykorzystaniem **Procedur Składowanych SQL** (np. `sp_GetSystemStats`, `sp_GetAdminUserReport`).
<img width="1628" height="287" alt="image" src="https://github.com/user-attachments/assets/f5cda13e-d267-4239-a663-e4430dc6aeb4" />

