# MonsterTradingCards

Git-Link:
https://github.com/johannesfor/MonsterTradingCards.git

Anfangs habe ich erstmal das SQL für die Tables geschrieben.

Danach habe ich bereits versucht eine Verbindung zur Datenbank herzustellen. Nachdem ich die ersten Repositories erstellt hatte, viel mir auf das der Code fast genau gleich war. Deshalb habe ich ein generisches Repository geschrieben. Das hat einiges an Zeit beansprucht, aber als es fertig war konnte ich sehr einfach neue Repositories von Entities erstellen.

Danach habe ich die Vorlage vom Herrn Prof. für den HTTP-Server genommen. Ich habe es insofern geändert, dass ich ihn Multi-Threaded gemacht habe und mit Enums erweitert habe, um mehr Type-Safe zu sein.

Nachdem quasi Front (Webserver) und Back (Repositories) standen, hab ich angefangen die Features "dazwischen" zu implementieren mittels dem MediatR NuGet Package.

Meine Infrastruktur sieht also wie folgt aus:

![oijafds drawio (1)](https://github.com/johannesfor/MonsterTradingCards/assets/62501211/3893a5f9-ba69-4551-960b-21b19e7a3cd3)


Die Authorizer (in rot) von MediatR werden vor der Business-Logic (in blau) ausgeführt, und wenn die Authorizer nicht erfolgreich durchlaufen, wird die Business-Logic nicht ausgeführt.

# Tests
Ich habe mich dafür entschieden das Registrieren/Login/Session-Management zu testen, da dies der Grundstein ist der Security von dieser Applikation.
Ebenso habe ich das Erstellen von Packages getestet, da nur der Admin dies darf.
Das Trading-Feature habe ich auch sehr zahlreich getestet, da es hier viele Fälle gibt, wo ein Trade erstellen/abschließen ungültig ist.
Insbesondere habe ich allerdings das Battle-System und das dazugehörige Lootboxen-System (mein Unique-Feature) getestet.

# Unique-Feature
Der Gewinner eines Battles erhält eine Lootbox, welche eine zufällige Seltenheit haben kann.
Der User kann sich seine Lootboxen ausgeben und seine öffnen.
Beim Öffnen einer Lootbox wird eine neue Karte für den User generiert. Die Seltenheit beeinflusst den Schaden beim Erstellen der Karte.

# Lessons-learned
Mein Fehler war, dass ich relativ schnell angefangen habe zu implementieren, bevor ich die Angabe zu 100% verstanden hatte. Dadurch musste ich relativ viel Code umschreiben.
Beim nächsten Mal werde ich mir die Angabe genauer durchlesen, und planmäßig jeden Use und Edgecase durchgehen bevor ich mit der Implementierung starte.

# Gebrauchte Zeit
Insgesamt habe ich 46 Stunden gebraucht für diese Lösung.
