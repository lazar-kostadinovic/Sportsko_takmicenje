# Sportsko takmičenje
Veb aplikacija dizajnirana za praćenje i organizaciju različitih aspekata turnira. 

Sistem skladišti i upravlja podacima o klubovima, timovima, sportistima i trenerima timova, kao i sportovima u kojima su se takmičili.

Održava zapise o odigranim mečevima, pojedinačnim rezultatima mečeva i izračunatim rang listama.

Pored toga, sistem generiše rang liste za svaki pojedinačni meč, kao i kumulativne rang liste za ceo turnir.

# Baza 
Za pokretanje aplikacije potrebno kreirati bazu u Neo4j.

Naredbe za popunu baze se nalaze u txt fajlu sa nazivom "komande za popunu baze".

Nakon unosa komandi potrebno je pokretnuti Neo4j bazu.

Osim toga potrebni su i pokrenuti Redis server kao i Redis klijent koji se koristi za kreiranje rang liste.

# Start aplikacije 
Aplikaciju pokrećmo iz terminala pomoću dotnet watch run komande a nakon toga mozemo da otvorimo Client->html->Klub.html u pretrazivaču.

