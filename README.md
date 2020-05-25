# Photobox application

### Starta applikationen

För att starta applikationen (backend):

Öppna backend-foldern i Visual Studio Code (alltså inte gruppmappen)

Ctrl+F5 i VSC för att starta, sedan kan man göra requests till localhost:5001/weatherforecasts (som är vår "template" api-route)

### Om branching

Ett bra tankesätt kan vara att betrakta varje branch som sitt eget, separata repo. Det funkar som vanligt med git add, commit, push när man jobbar i en branch. Om man är två som samarbetar i en branch, är det viktigt att bara en jobbar åt gången, och när man vill byta så kan den ena pusha upp och den andra göra en pull

##### Innan man gör en ny branch:

git checkout master (ändra branch till master-branch)

git pull (dra ned senaste)

git checkout -b "<branch-name>" (skapa en ny branch och byt till den)
  
OBS! När man byter branch så byts alla filerna som ligger lokalt ut. Så kom ihåg att spara er branch innan ni byter branch! (detta varnar git för)


### Databashantering

Varje controller får ett databasobjekt till sig i stil med "SomeController(PhotoboxDB photoboxDB)" som man sedan kan använda lokalt i sin controller för att hantera calls till databasen.
