# Notes
## Uruchamianie
Polecenie `docker-compose up`
## Podstrony
### Logowanie
/auth/register
/auth/login
### Notatki
/note/create - tworzenie notatek
/note/index - lista notatek
/note/details/{id} - szczegóły notatki
## Funkcjonalności
### Logowanie
Aplikacja pozwala na tworzenie nowych użytkowników i na logowanie się istniejących. Podczas rejestracji aplikacja wyświetla siłę hasła (na podstawie zawierania małych i wielkich liter, cyfr i znaków specjalnych), wymaga powtórzenia go i pozwala je podjerzeć.
### Notatki
Aplikacja pozwala na tworzenie i wyświetalnie notatek. Każda notatka podczas tworzenia wymaga ustawienia tytułu. Dodatkowo można ustawić treść notatki mającą do 5000 znaków, w tym różne znaczniki html i tagi css. Notatki mogą być zaszyfrowane, w takim wypadku przed ich wyświetleniem wymagane jest podanie hasła. Jeżeli notatka nie jest zaszyfrowana, to może być publiczna, w tedy każdy może ją wyświetlić pod odpowienidm adresem. Możliwe też jest udostępnienie notatki tylko konkretnym użytkownikom, w takim wypadku należy w odpowiednim polu podać ich nazwy użytkownika oddzielone przecinkiem w formacie A,B,C gdzie litery reprezentują nazwy użytkowników.
