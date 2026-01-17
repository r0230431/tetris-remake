# Unity

## Opdracht 1 - Kennismaking + Pong
Bekijk de video cursus op [YouTube](https://youtube.com/playlist?list=PLlrxD0HtieHjTwLm-Ip_V9NtVud8Kx58w&si=-HJiAZlW7FOVItv5). Dit zijn 7 videos die ongeveer 40 minuten in totaal duren. De laatste video kan je overslaan tot je spel klaar is.

In de 2de video zal uitgelegd worden hoe je Unity installeert. De installatie kan lang duren, bekijk in tussentijd alvast de rest van de video's en/of check uit hoe het spel 'Pong' werkt via [deze link](https://www.ponggame.org/).

Tutorials specifiek voor 2D games:
- [Brick breaker](https://www.youtube.com/watch?v=RYG8UExRkhA)
- [Sprite editor en sheets](https://learn.unity.com/tutorial/introduction-to-sprite-editor-and-sheets#)
- [Sprite animations](https://learn.unity.com/tutorial/introduction-to-sprite-animations#)
- [Unity Diversen](https://learn.unity.com/tutorials)
- [Modular codebase, voor als je nog verder wil gaan](https://learn.unity.com/tutorial/65e0cfacedbc2a2351773054#65e0d032edbc2a213003ec4b)

Je gaat zelf een versie maken van Pong in Unity. Je houdt het bij 'single player' mode. Houd je aan onderstaande vereisten.

- Zorg ervoor dat bij het starten van het spel: beide paddles in het midden van hun goal staan, de bal in het midden van het scherm vertrekt, en de bal een willekeurige richting uit gaat.
- Zorg ervoor dat de bal sneller en sneller gaat elke keer deze een paddle raakt (=acceleratie). Overdrijf niet te veel zodat het speelbaar blijft. 
- De paddles mogen enkel op en neer kunnen bewegen.
- De pijltjestoetsen laten de paddle van de speler op en neer bewegen.
- Zorg ervoor dat de paddle van de computer speler altijd in de richting van de bal beweegt (enkel de y-positie). Experimenteer met snelheden tot je een tegenspeler hebt die je kan verslaan, maar het je niet té makkelijk maakt.
- Houd de score bij en toon die.
- Nadat een speler een punt maakt, laat je de bal onmiddelijk opnieuw vertrekken in het midden van het scherm, in een willekeurige richting.

- Optioneel: maak 3 opties: makkelijk, normaal en moeilijk. Je kan dit doen door de snelheid van de bal te wijzigen, die van de tegenspeler, of door de 'acceleratie' van de bal aan te passen. Probeer eerst eens door de keuze gewoon in een variabele te stoppen, als je nog tijd over hebt laat je de speler kiezen.

Maak op voorhand een korte analyse hoe je dit zal aanpakken. Begin niet van iets dat een AI heeft gemaakt om het dan aan te passen, daar verlies je meer tijd mee dan je wint. Je kan AI om hulp vragen, maar bouw je basis zelf op!

### Hoe en wat inleveren?
Maak als je klaar bent een Windows publish van je spel. Deze zet je samen met de code in je repo.

##### Wat inleveren?
- Je volledige code (veplicht om te kunnen slagen)
- Een Windows publish van je spel (om het maximum te kunnen scoren)

##### Waar inleveren?
In de repo van de GitHub Classroom assignment: commit en push je repo voor de deadline (enkel inleveringen op GitHub zullen beoordeeld worden).

## Opdracht 2 - Eigen game
Kies een spel uit de lijst met voorstellen en maak hiervan je eigen versie of maak het zo veel mogelijk na in Unity, rekening houdend met de minimum vereisten. Heb je zelf een leuk voorstel, leg het dan voor aan je docent. Beperk je tot 2D games. Probeer niet te veel tijd te verliezen met de grafische kant tot in de details af te werken, concentreer je op de werking en code.

Afbeeldingen/graphics kan je op volgende plaatsen vinden:
- Het template materiaal van Unity zelf
- Laten genereren door een AI
- 'van het internet' (Google) => Voor educatieve doeleinden mag je materiaal waar normaal een licentie op zit gewoon gebruiken. Weet wel dat je dan je spel niet mag publishen. Er zijn ook open source bibliotheken te vinden indien je graag je spel zou delen. Lees de Unity terms of service voor je een spel deelt.

Je spel bevat minstens deze principes:
- Animated 2D sprite(s) met behulp van een spritesheet
- 2D objecten die bewegen over het scherm
- Minstens 2 power-ups
- Collision detection
- Minstens 2 moeilijkheidsgraden
- Iets dat willekeurig gegenereerd wordt
- Een soort progressie (coins sparen/meerdere levels/punten/...)
- Een manier om te winnen en een manier om te verliezen
- Een 'high score' die bewaard wordt
- Drag/Drop OF iets bewegen met de pijltjestoetsen (OF beiden)
- Maak gebruik van OOP (minstens classes en objects)
- Optioneel: een easter egg
- Optioneel: extra power-ups
- Optioneel: een naam bijhouden naast de high score
- Optioneel: multiplayer mode
- Optioneel: gebruik je fantasie...

TIPS: maak regelmatig een backup wanneer je een werkbare versie hebt en aan de volgende feature gaat beginnen. Dat kan via Github, zipjes, ...

#### Voorstellen:
- Bejeweled (=> match-3 games)
- 1 van de vele 'sorting games' ([inspiratie](https://plays.org/sorting-games/))
- Tetris
- Bubble shooter
- Zuma
- 2048
- Snake
- Breakout
- Super Mario Bros (=> simpele 2D platformers/sidescrollers)
=> Maak nadat je de video tutorial gezien hebt eerst een korte analyse en beoordeel of het voor jou haalbaar is. Bij twijfel check je bij de docent.

#### Pitfalls
- Kies geen spellen die onspeelbaar worden als je alles willekeurig genereert (Sudoku, solitaire kaartspellen, ....). Als je zo'n spellen maakt heb je een bibliotheek nodig van combinaties die werkelijk uitspeelbaar zijn (al dan niet door een AI gegenereerd).
- Houd je graphics simpel. Mooie graphics kosten tijd en vereisen vaak veel talent. 
- Als je een simpel spel kiest, moet je extra zaken bij verzinnen om aan alle vereisten te voldoen. Als je niet creatief genoeg bent hiervoor, kies dan een spel waar de vereisten sowieso al in zitten.
- Als je een klassieke 2D platformer bouwt, houd je dan bij levels die je op voorhand ontwerpt. Je willekeurig gegenereerd item is dan best gewoon een willekeurige power-up of vijand.

### Hoe en wat inleveren?
Maak als je klaar bent een Windows publish van je spel. Deze zet je samen met de code in je repo.

Maak daarnaast ook een demo filmpje van je spel: 
- Deze demo moet alle funcionaliteiten laten zien. 
- Voeg via tekst en/of spraak instructies toe voor de controls, leg uit wat welke powerup juist doet, welke input waar verwacht wordt, hoe je punten scoort, hoe je naar het volgende level gaat, ...
- Gebruik hiervoor screen recording software (zoals OBS Studio), en upload je video in MP4 format op Canvas. 
- Je resolutie is bij voorkeur 720p of 1080p.
- OPTIONEEL: pak het aan alsof je een gameplay trailer maakt voor je spel en stop er wat humor in.

Als je geen headset hebt kan je eentje uitlenen op school. 

##### Wat inleveren?
- Je code (veplicht om te kunnen slagen)
- Een Windows publish van je spel (om het maximum te kunnen scoren)
- Een demo filmpje (om het maximum te kunnen scoren)

##### Waar inleveren?
- Code + build:
   - In de repo van de GitHub Classroom assignment: commit en push je repo voor de deadline (enkel inleveringen op GitHub zullen beoordeeld worden).
- Demo filmpje:
   - In de Canvas opdracht (GithHub heeft beperkingen op bestandsgrootte dus stop dit NIET in je repo). 
   - LET OP: Indien je geen code indient op GitHub zal je filmpje niet beoordeeld worden!