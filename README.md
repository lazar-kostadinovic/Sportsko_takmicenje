Veb aplikacija dizajnirana za praćenje i organizaciju različitih aspekata turnira. 
Sistem skladišti i upravlja podacima o klubovima, timovima, sportistima i trenerima timova,
kao i sportovima u kojima su se takmičili.
Održava zapise o odigranim mečevima, pojedinačnim rezultatima mečeva i izračunatim rang listama.
Pored toga, sistem generiše rang liste za svaki pojedinačni meč, kao i kumulativne rang liste za ceo turnir.

Za pokretanje aplikacije potrebno kreirati bazu u Neo4J.
Naredbe za popunu baze se nalaze u txt fajlu sa nazivom "komande za popunu baze".
Nakon unosa komandi potrebno je pokretnuti Neo4J bazu.
Osim toga potrebna je i pokrenut Redis klijent koja se koristi za kreiranje rang liste.
